using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class IfElseStatement : IStatement
    {
        public bool IsBlockStatement => true;
        public ParserInfo ParserInfo { get; }

        public ConditionalBlockStatement MainIf { get; }
        public ConditionalBlockStatement[] ElseIfs { get; }
        public IStatement Else { get; }
        
        
        public IfElseStatement(ConditionalBlockStatement mainIf, ParserInfo parserInfo)
        {
            MainIf = mainIf;
            ParserInfo = parserInfo;
        }
        
        public IfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs, IStatement @else, ParserInfo parserInfo)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
            Else = @else;
            ParserInfo = parserInfo;
        }
        
        public IfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs, ParserInfo parserInfo)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
            ParserInfo = parserInfo;
        }
        
        public IfElseStatement(ConditionalBlockStatement mainIf, IStatement @else, ParserInfo parserInfo)
        {
            MainIf = mainIf;
            Else = @else;
            ParserInfo = parserInfo;
        }
    }
}