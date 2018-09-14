using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class IfElseStatement : IStatement, IBranchWrapperStatement
    {
        public bool CanBeEmbedded => true;
        public StatementInfo Info { get; }

        public ConditionalBlockStatement MainIf { get; }
        public ConditionalBlockStatement[] ElseIfs { get; }
        public IStatement Else { get; }

        public IStatement[] TraversableChildren { get; protected set; }

        public IStatement[] Branches { get; }


        public IfElseStatement(ConditionalBlockStatement mainIf, StatementInfo info)
        {
            MainIf = mainIf;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(mainIf);
            Branches = new[] {mainIf.Statement};
        }

        public IfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs, IStatement @else,
            StatementInfo info)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
            Else = @else;
            Info = info;

            var children = new List<IStatement>(elseIfs.Length + 2);
            var branches = new List<IStatement>(elseIfs.Length + 2);
            if (mainIf != null)
            {
                children.Add(mainIf);
                branches.Add(mainIf.Statement);
            }

            foreach (var child in elseIfs)
            {
                if (child != null)
                {
                    children.Add(child);
                    branches.Add(child.Statement);
                }
            }

            if (@else != null)
            {
                children.Add(@else);
                branches.Add(@else);
            }

            TraversableChildren = children.ToArray();
            Branches = branches.ToArray();
        }

        public IfElseStatement(ConditionalBlockStatement mainIf, ConditionalBlockStatement[] elseIfs,
            StatementInfo info)
        {
            MainIf = mainIf;
            ElseIfs = elseIfs;
            Info = info;

            var children = new List<IStatement>(elseIfs.Length + 2);
            var branches = new List<IStatement>(elseIfs.Length + 2);
            if (mainIf != null)
            {
                children.Add(mainIf);
                branches.Add(mainIf.Statement);
            }

            foreach (var child in elseIfs)
            {
                if (child != null)
                {
                    children.Add(child);
                    branches.Add(child.Statement);
                }
            }

            TraversableChildren = children.ToArray();
            Branches = branches.ToArray();
        }

        public IfElseStatement(ConditionalBlockStatement mainIf, IStatement @else, StatementInfo info)
        {
            MainIf = mainIf;
            Else = @else;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(mainIf, @else);
            Branches = mainIf?.Statement != null
                ? new[] {mainIf.Statement, @else}
                : new[] {@else};
        }
    }
}