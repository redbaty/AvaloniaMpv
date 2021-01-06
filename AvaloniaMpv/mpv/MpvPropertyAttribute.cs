using System;

namespace AvaloniaMpv.mpv
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class MpvPropertyAttribute : Attribute
    {
        public MpvPropertyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}