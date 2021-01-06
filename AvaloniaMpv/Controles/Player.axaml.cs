using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace AvaloniaMpv.Controles
{
    public class Player : UserControl
    {
        public static readonly DirectProperty<Player, string> MediaProperty =
            AvaloniaProperty.RegisterDirect<Player, string>(
                nameof(Media),
                o => o.Media,
                (o, v) => o.Media = v);

        private string _media;

        public string Media
        {
            get => _media;
            set
            {
                SetAndRaise(MediaProperty, ref _media, value);
                OnMediaChanged(value);
            }
        }

        private void OnMediaChanged(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                var mpvControlHost = this.Get<MpvControlHost>("ControlHost");
                mpvControlHost.Wrapper.LoadFile(path);
            }
        }

        public Player()
        {
            InitializeComponent();
            PointerEnter += OnPointerEnter;
            PointerMoved += OnPointerMoved;
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnPointerEnter(object? sender, PointerEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
