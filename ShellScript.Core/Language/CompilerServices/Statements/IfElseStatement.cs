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

        public IStatement[] TraversableChildren { get; protected set; }


        public IfElseStatement(ConditionalBlockStatement mainIf, StatementInfo info)
        {
            MainIf = mainIf;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(mainIf);
        }

        public IfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs, IStatement @else,
            StatementInfo info)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
            Else = @else;
            Info = info;

            var result = new List<IStatement>(elseIfs.Length + 2);
            if (mainIf != null)
                result.Add(mainIf);
            foreach (var child in elseIfs)
            {
                if (child != null)
                    result.Add(child);
            }

            if (@else != null)
                result.Add(@else);

            TraversableChildren = result.ToArray();
        }

        public IfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs,
            StatementInfo info)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
            Info = info;
            
            var result = new List<IStatement>(elseIfs.Length + 2);
            if (mainIf != null)
                result.Add(mainIf);
            foreach (var child in elseIfs)
            {
                if (child != null)
                    result.Add(child);
            }
            
            TraversableChildren = result.ToArray();
        }

        public IfElseStatement(ConditionalBlockStatement mainIf, IStatement @else, StatementInfo info)
        {
            MainIf = mainIf;
            Else = @else;
            Info = info;
            
            TraversableChildren = StatementHelpers.CreateChildren(mainIf, @else);
        }
    }
}