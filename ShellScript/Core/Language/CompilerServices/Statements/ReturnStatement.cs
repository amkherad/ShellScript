namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ReturnStatement : IStatement
    {
        public bool IsBlockStatement => false;
        public StatementInfo Info { get; }

        public EvaluationStatement Result { get; }
        
        public IStatement[] TraversableChildren { get; protected set; }

        
        public ReturnStatement(EvaluationStatement result, StatementInfo info)
        {
            Result = result;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(result);
        }
    }
}