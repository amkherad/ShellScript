namespace ShellScript.Core.Language.CompilerServices.Statements.PreProcessors
{
    public class PreProcessorIfElseStatement : IfElseStatement
    {
        public PreProcessorIfElseStatement(ConditionalBlockStatement mainIf, StatementInfo info) : base(mainIf, info)
        {
        }

        public PreProcessorIfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs,
            IStatement @else, StatementInfo info) : base(mainIf, elseIfs, @else, info)
        {
        }

        public PreProcessorIfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs,
            StatementInfo info) : base(mainIf, elseIfs, info)
        {
        }

        public PreProcessorIfElseStatement(ConditionalBlockStatement mainIf, IStatement @else, StatementInfo info) :
            base(mainIf, @else, info)
        {
        }
    }
}