using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaMpv.mpv
{
    public class MpvWrapper
    {
        public MpvWrapper(IntPtr systemHandle, MpvStatus mpvStatus)
        {
            SystemHandle = systemHandle;
            MpvStatus = mpvStatus;
            MpvHandle = Libmpv.mpv_create();
            Initialize();
            EventTask = new MpvEventTask(this, MpvStatus, EventTaskCancellationTokenSource).StartEventTask();
        }

        private Task EventTask { get; }

        private CancellationTokenSource EventTaskCancellationTokenSource { get; } = new CancellationTokenSource();

        public IntPtr MpvHandle { get; }

        private IntPtr SystemHandle { get; }

        private MpvStatus MpvStatus { get; }

        private void Initialize()
        {
            Libmpv.set_property_string(MpvHandle, "wid", SystemHandle.ToString());
            Libmpv.set_property_string(MpvHandle, "osc", "no");
            Libmpv.set_property_string(MpvHandle, "force-window", "no");
            Libmpv.set_property_string(MpvHandle, "no-taskbar-progress", "no");
            var err = Libmpv.mpv_initialize(MpvHandle);

            if (err != Libmpv.mpv_error.MPV_ERROR_SUCCESS) throw new LibMpvException(err);
            MpvObservables.SetupObservables(MpvHandle);
        }

        private static string GetLanguage(string id)
        {
            foreach (var ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                if (ci.ThreeLetterISOLanguageName == id)
                    return ci.EnglishName;

            return id;
        }

        public IEnumerable<MediaTrack> GetTracks()
        {
            var count = Libmpv.get_property_int(MpvHandle, "track-list/count");

            for (var i = 0; i < count; i++)
            {
                var type = Libmpv.get_property_string(MpvHandle, $"track-list/{i}/type");

                if (type == "video")
                    continue;

                yield return new MediaTrack(type == "sub" ? MediaTrackType.Subtitle : MediaTrackType.Audio,
                    GetLanguage(Libmpv.get_property_string(MpvHandle, $"track-list/{i}/lang")),
                    Libmpv.get_property_int(MpvHandle, $"track-list/{i}/id"),
                    Libmpv.get_property_string(MpvHandle, $"track-list/{i}/title"));
            }
        }

        private void WaitForEvent(Libmpv.mpv_event_id eventId)
        {
            while (true)
            {
                var ptr = Libmpv.mpv_wait_event(MpvHandle, -1);

                if (Marshal.PtrToStructure(ptr, typeof(Libmpv.mpv_event)) is Libmpv.mpv_event evt)
                {
                    if (evt.event_id == eventId)
                    {
                        break;
                    }
                }
            }
        }

        public void PlayPause()
        {
            DoCommand("set", "pause", MpvStatus.Paused ? "no" : "yes");
        }

        public void LoadFile(string path)
        {
            DoCommand("loadfile", path);
            WaitForEvent(Libmpv.mpv_event_id.MPV_EVENT_FILE_LOADED);
        }

        private void DoCommand(params string[] commands)
        {
            var err = Libmpv.ExecuteCommand(MpvHandle, commands);
            if (err != Libmpv.mpv_error.MPV_ERROR_SUCCESS) throw new LibMpvException(err);
        }

        public void Close()
        {
            Libmpv.mpv_terminate_destroy(MpvHandle);
            EventTaskCancellationTokenSource?.Cancel();
            EventTask?.Dispose();
        }
    }
}