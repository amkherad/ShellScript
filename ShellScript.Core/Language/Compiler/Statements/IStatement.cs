using ShellScript.Core.Language.Compiler.Parsing;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public interface IStatement
    {
        /// <summary>
        /// Determines whether this statement has child statements or not.
        /// </summary>
        bool IsBlockStatement { get; }
        
        ParserInfo ParserInfo { get; }
    }
}