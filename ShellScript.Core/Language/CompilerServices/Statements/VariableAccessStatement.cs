namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class VariableAccessStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
        public override StatementInfo Info { get; }

        public string VariableName { get; }
        
        
        public VariableAccessStatement(string variableName, StatementInfo info)
        {
            VariableName = variableName;
            Info = info;

            TraversableChildren = new IStatement[0];
        }
    }
}