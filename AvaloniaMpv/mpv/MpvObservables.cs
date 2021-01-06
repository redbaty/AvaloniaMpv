using System;
using System.Reflection;

namespace AvaloniaMpv.mpv
{
    internal class MpvObservables
    {
        private double _position;
        private bool _paused;

        public MpvObservables(MpvStatus status)
        {
            Status = status;
        }

        private MpvStatus Status { get; }

        [MpvProperty("time-pos")]
        public double Position
        {
            get => _position;
            set
            {
                _position = value;
                if (Status != null) Status.Position = TimeSpan.FromSeconds(value);
            }
        }

        [MpvProperty("pause")]
        public bool Paused
        {
            get => _paused;
            set
            {
                _paused = value;
                if (Status != null) Status.Paused = value;
            }
        }

        public static void SetupObservables(IntPtr handle)
        {
            foreach (var propertyInfo in typeof(MpvObservables).GetProperties())
                if (propertyInfo.GetCustomAttribute<MpvPropertyAttribute>() is { } mpvPropertyAttribute)
                    Libmpv.mpv_observe_property(handle, 0, mpvPropertyAttribute.Name, GetFormat(propertyInfo.PropertyType));
        }

        private static Libmpv.mpv_format GetFormat(Type type)
        {
            if (type == typeof(double))
                return Libmpv.mpv_format.MPV_FORMAT_DOUBLE;

            if (type == typeof(bool))
                return Libmpv.mpv_format.MPV_FORMAT_FLAG;

            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}