using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class IfElseStatement : IStatement
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