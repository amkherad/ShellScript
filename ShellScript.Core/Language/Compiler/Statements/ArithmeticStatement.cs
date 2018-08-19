namespace ShellScript.Core.Language.Compiler.Statements
{
    public abstract class ArithmeticStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
    }
}