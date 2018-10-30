using System;

namespace ShellScript.Core.Language.Compiler.Transpiling
{
    [Flags]
    public enum ScopeType
    {
        Block = 0x1,
        
        MethodRoot = 0x2,
        
        ClassRoot = 0x4,
        
        IfMainBlock = Block,
        IfIfElseBlock = Block,
        IfElseBlock = Block,
    }
}