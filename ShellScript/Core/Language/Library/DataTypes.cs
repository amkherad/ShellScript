using System;

namespace ShellScript.Core.Language.Library
{
    [Flags]
    public enum DataTypes
    {
        Void = 0,

        Array = 0x4000,

        Integer = 1,

        Float = 2,

        Numeric = 3,

        Boolean = 4,

        String = 16,

        Class = 32,

        Delegate = 64,

        Lookup = 0x100, //Should not be used in code.
    }
}