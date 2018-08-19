using ShellScript.Core.Language.Compiler.Parsing;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public abstract class EvaluationStatement : IStatement
    {
        public abstract bool IsBlockStatement { get; }
        public abstract ParserInfo ParserInfo { get; }
    }
}