using System;

namespace AvaloniaMpv.mpv
{
    public class LibMpvException : Exception
    {
        public LibMpvException(Libmpv.mpv_error error)
        {
            Error = error;
        }

        private Libmpv.mpv_error Error { get; }

        public override string ToString()
        {
            return $"Lib MPV threw an error: {Error.GetMessage()}";
        }
    }
}