using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace AvaloniaMpv.Controles
{
    public class PlayerControls : UserControl
    {
        public static readonly DirectProperty<PlayerControls, MpvControlHost> MpvControlProperty =
            AvaloniaProperty.RegisterDirect<PlayerControls, MpvControlHost>(
                nameof(MpvControl),
                o => o.MpvControl,
                (o, v) => o.MpvControl = v);

        private MpvControlHost _mpvControl;

        public MpvControlHost MpvControl
        {
            get => _mpvControl;
            set => SetAndRaise(MpvControlProperty, ref _mpvControl, value);
        }

        public void PlayPause(object sender, RoutedEventArgs e)
        {
            MpvControl?.Wrapper?.PlayPause();
        }

        public PlayerControls()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}