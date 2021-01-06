using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaMpv.mpv
{
    public class MpvEventTask
    {
        public MpvEventTask(MpvWrapper wrapper, MpvStatus mpvStatus, CancellationTokenSource cancellationTokenSource)
        {
            Wrapper = wrapper;
            MpvStatus = mpvStatus;
            MpvObservables = new MpvObservables(mpvStatus);
            CancellationTokenSource = cancellationTokenSource;
        }

        private CancellationTokenSource CancellationTokenSource { get; }

        private MpvWrapper Wrapper { get; }

        private MpvStatus MpvStatus { get; }

        private MpvObservables MpvObservables { get; }

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public Task StartEventTask()
        {
            var observablesDictionary = typeof(MpvObservables).GetProperties().Select(i => new
                {
                    PropertyObserved = i.GetCustomAttribute<MpvPropertyAttribute>() is { } a ? a.Name : null,
                    Property = i
                })
                .Where(i => i.PropertyObserved != null)
                .ToDictionary(i => i.PropertyObserved, i => i.Property);

            return Task.Run(() =>
            {
                while (true)
                {
                    if (CancellationTokenSource.IsCancellationRequested)
                        break;

                    var ptr = Libmpv.mpv_wait_event(Wrapper.MpvHandle, -1);

                    if (Marshal.PtrToStructure(ptr, typeof(Libmpv.mpv_event)) is Libmpv.mpv_event mpvEvent)
                        switch (mpvEvent.event_id)
                        {
                            case Libmpv.mpv_event_id.MPV_EVENT_FILE_LOADED:
                                MpvStatus.Duration = TimeSpan.FromSeconds(Libmpv.get_property_number(Wrapper.MpvHandle, "duration"));
                                MpvStatus.Path = Libmpv.get_property_string(Wrapper.MpvHandle, "path");
                                break;
                            case Libmpv.mpv_event_id.MPV_EVENT_PROPERTY_CHANGE:
                            {
                                if (Marshal.PtrToStructure(mpvEvent.data, typeof(Libmpv.mpv_event_property)) is Libmpv.mpv_event_property eventProperty)
                                    if (observablesDictionary.TryGetValue(eventProperty.name, out var propertyInfo))
                                    {
                                        var value = eventProperty.data == IntPtr.Zero ? GetDefault(propertyInfo.PropertyType) : GetValue(eventProperty, propertyInfo.PropertyType);

                                        if (propertyInfo.CanWrite) propertyInfo.SetValue(MpvObservables, value);
                                    }
                                break;
                            }
                        }
                }
            }, CancellationTokenSource.Token);
        }

        private static object GetValue(Libmpv.mpv_event_property eventProperty, Type propertyType)
        {
            if (eventProperty.format == Libmpv.mpv_format.MPV_FORMAT_FLAG)
            {
                return Marshal.PtrToStructure<int>(eventProperty.data) == 1;
            }

            return Marshal.PtrToStructure(eventProperty.data, propertyType);
        }
    }
}