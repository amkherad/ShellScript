namespace ShellScript.Core.Language.Compiler.Statements
{
    public abstract class LogicalStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
    }
}