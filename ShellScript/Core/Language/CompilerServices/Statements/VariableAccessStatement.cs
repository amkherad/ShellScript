namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class VariableAccessStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }

        public string ClassName { get; }
        public string VariableName { get; }
        
        
        public VariableAccessStatement(string className, string variableName, StatementInfo info)
        {
            ClassName = className;
            VariableName = variableName;
            Info = info;

            TraversableChildren = new IStatement[0];
        }
        
        public VariableAccessStatement(string variableName, StatementInfo info)
        {
            VariableName = variableName;
            Info = info;

            TraversableChildren = new IStatement[0];
        }

        public override string ToString()
        {
            return VariableName;
        }
    }
}