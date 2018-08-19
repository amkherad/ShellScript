using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public abstract class EvaluationStatement : IStatement
    {
        public abstract bool IsBlockStatement { get; }
        public abstract ParserInfo ParserInfo { get; }
    }
}