namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public abstract class ArithmeticStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
    }
}