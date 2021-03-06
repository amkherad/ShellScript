namespace ShellScript.Core.Language.Compiler.Statements
{
    public class NopStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }

        public NopStatement(StatementInfo info)
        {
            Info = info;

            TraversableChildren = new IStatement[0];
        }
    }
}