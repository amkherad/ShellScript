namespace ShellScript.Core.Language.Compiler.Statements
{
    public class LambdaExpression : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }
        
        
        public LambdaExpression(StatementInfo info)
        {
            Info = info;
        }
    }
}