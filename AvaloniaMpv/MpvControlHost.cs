using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using AvaloniaMpv.mpv;

namespace AvaloniaMpv
{
    public class MpvControlHost : NativeControlHost
    {
        public MpvWrapper Wrapper { get; private set; }

        public MpvStatus Status { get; } = new MpvStatus();

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            parent = base.CreateNativeControlCore(parent);
            Wrapper = new MpvWrapper(parent.Handle, Status);
            return parent;
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            Wrapper?.Close();
            Wrapper = null;
            base.DestroyNativeControlCore(control);
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
        }
    }
}