namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ThrowStatement : IStatement
    {
        public bool CanBeEmbedded => true;
        public StatementInfo Info { get; }
        public VariableAccessStatement Exception { get; }

        public IStatement[] TraversableChildren => Exception != null
            ? new IStatement[] {Exception}
            : new IStatement[0];

        public ThrowStatement(VariableAccessStatement exception, StatementInfo info)
        {
            Exception = exception;
            Info = info;
        }

        public override string ToString()
        {
            return Exception != null ? $"throw {Exception.VariableName}" : "throw";
        }
    }
}