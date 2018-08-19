using System;

namespace ShellScript.Core.Language.Sdk
{
    [Flags]
    public enum DataTypes
    {
        Array = 0x4000,
        
        Variant = 0,
        
        Boolean = 1,
        
        Numeric = 2,
        
        Decimal = 3,
        
        Float = 4,
        
        String = 16,
        
        Class = 32,
        
        Delegate = 64,
    }
}