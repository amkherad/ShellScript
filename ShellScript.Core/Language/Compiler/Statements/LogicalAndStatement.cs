namespace ShellScript.Core.Language.Compiler.Statements
{
    public class LogicalAndStatement : LogicalOperatorStatement
    {
        public LogicalStatement Left { get; }
        public LogicalStatement Right { get; }
        
        public LogicalAndStatement(LogicalStatement left, LogicalStatement right)
        {
            Left = left;
            Right = right;
        }
    }
}