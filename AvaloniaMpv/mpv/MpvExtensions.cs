using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AvaloniaMpv.mpv
{
    public static class MpvExtensions
    {
        private static string ConvertFromUtf8(IntPtr nativeUtf8)
        {
            int len = 0;

            while (Marshal.ReadByte(nativeUtf8, len) != 0)
                ++len;

            byte[] buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        public static string GetMessage(this Libmpv.mpv_error err) => ConvertFromUtf8(Libmpv.mpv_error_string(err));
    }
}