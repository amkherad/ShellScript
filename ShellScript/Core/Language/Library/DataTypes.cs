using System;

namespace ShellScript.Core.Language.Library
{
    [Flags]
    public enum DataTypes
    {
        Void = 0,

        Array = 0x4000,

        Integer = 0x1,

        Float = 0x2,

        Numeric = 0x3,

        Boolean = 0x4,

        String = 0x10,

        Class = 0x20,

        Delegate = 0x30,
        
        Any = 0x80,

        Lookup = 0x1000, //Should not be used in code.
    }
}