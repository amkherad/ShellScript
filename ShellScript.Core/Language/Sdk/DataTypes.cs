using System;

namespace ShellScript.Core.Language.Sdk
{
    [Flags]
    public enum DataTypes
    {
        Void = 0, //Should not be used in code.
        
        Array = 0x4000,
        
        Boolean = 0x2001,
        
        Decimal = 1,
        
        Float = 2,
        
        Numeric = 3,
        
        String = 16,
        
        Class = 32,
        
        Delegate = 64,
    }
}