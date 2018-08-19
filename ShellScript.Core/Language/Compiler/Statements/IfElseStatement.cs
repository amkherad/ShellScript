namespace ShellScript.Core.Language.Compiler.Statements
{
    public class IfElseStatement : IStatement
    {
        public bool IsBlockStatement => true;
        
        public ConditionalBlockStatement MainIf { get; }
        public ConditionalBlockStatement[] ElseIfs { get; }
        public IStatement Else { get; }
        
        
        public IfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs, IStatement @else)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
            Else = @else;
        }
    }
}