namespace ShellScript.Core.Language.Compiler.Statements
{
    public class LambdaExpressionStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }

        public string[] Parameters { get; }
        public IStatement Statement { get; }

        public IStatement[] TraversableChildren { get; protected set; }


        public LambdaExpressionStatement(StatementInfo info, string[] parameters, IStatement statement)
        {
            Info = info;
            Parameters = parameters;
            Statement = statement;

            TraversableChildren = new[] {statement};
        }
    }
}