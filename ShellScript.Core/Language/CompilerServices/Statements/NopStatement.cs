namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class NopStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
    }
}