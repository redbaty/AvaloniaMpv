using System;
using ReactiveUI;

namespace AvaloniaMpv.mpv
{
    public class MpvStatus : ReactiveObject
    {
        private string _path;
        private TimeSpan _duration;
        private bool _paused;
        private TimeSpan _position;

        public string Path
        {
            get => _path;
            set => this.RaiseAndSetIfChanged(ref _path, value, nameof(Path));
        }

        public TimeSpan Duration
        {
            get => _duration;
            set => this.RaiseAndSetIfChanged(ref _duration, value, nameof(Duration));
        }

        public TimeSpan Position
        {
            get => _position;
            set => this.RaiseAndSetIfChanged(ref _position, value, nameof(Position));
        }
        
        public string PausedText => Paused ? "▶" : "⏸";

        public bool Paused
        {
            get => _paused;
            set
            {
                if (value != _paused)
                {
                    _paused = value;
                    this.RaisePropertyChanged(nameof(Paused));
                    this.RaisePropertyChanged(nameof(PausedText));
                }

            }
        }
    }
}