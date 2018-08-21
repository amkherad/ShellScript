namespace ShellScript.Core.Language.CompilerServices.Statements.PreProcessors
{
    public class PreProcessorIfElseStatement : ICompilerAnnotationStatement
    {
        public bool IsBlockStatement => true;
        
        public ConditionalBlockStatement MainIf { get; }
        public ConditionalBlockStatement[] ElseIfs { get; }
        public IStatement Else { get; }
        
        
        public PreProcessorIfElseStatement(ConditionalBlockStatement mainIf)
        {
            MainIf = mainIf;
        }
        
        public PreProcessorIfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs, IStatement @else)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
            Else = @else;
        }
        
        public PreProcessorIfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
        }
        
        public PreProcessorIfElseStatement(ConditionalBlockStatement mainIf, IStatement @else)
        {
            MainIf = mainIf;
            Else = @else;
        }
    }
}