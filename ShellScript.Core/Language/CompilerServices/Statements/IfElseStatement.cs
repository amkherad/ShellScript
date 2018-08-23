using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class IfElseStatement : IStatement
    {
        public bool IsBlockStatement => true;
        public StatementInfo Info { get; }

        public ConditionalBlockStatement MainIf { get; }
        public ConditionalBlockStatement[] ElseIfs { get; }
        public IStatement Else { get; }
        
        
        public IfElseStatement(ConditionalBlockStatement mainIf, StatementInfo info)
        {
            MainIf = mainIf;
            Info = info;
        }
        
        public IfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs, IStatement @else, StatementInfo info)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
            Else = @else;
            Info = info;
        }
        
        public IfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs, StatementInfo info)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
            Info = info;
        }
        
        public IfElseStatement(ConditionalBlockStatement mainIf, IStatement @else, StatementInfo info)
        {
            MainIf = mainIf;
            Else = @else;
            Info = info;
        }


        public IEnumerable<IStatement> TraversableChildren
        {
            get
            {
                yield return MainIf;
                foreach (var ei in ElseIfs)
                    yield return ei;
                if (Else != null) yield return Else;
            }
        }
    }
}