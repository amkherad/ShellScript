namespace ShellScript.Core.Language.CompilerServices.Statements.PreProcessors
{
    public class IfElseStatement : ICompilerAnnotationStatement
    {
        public bool IsBlockStatement => true;
        
        public ConditionalBlockStatement MainIf { get; }
        public ConditionalBlockStatement[] ElseIfs { get; }
        public IStatement Else { get; }
        
        
        public IfElseStatement(ConditionalBlockStatement mainIf)
        {
            MainIf = mainIf;
        }
        
        public IfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs, IStatement @else)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
            Else = @else;
        }
        
        public IfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
        }
        
        public IfElseStatement(ConditionalBlockStatement mainIf, IStatement @else)
        {
            MainIf = mainIf;
            Else = @else;
        }
    }
}