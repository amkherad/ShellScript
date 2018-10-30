using System.Linq;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class SwitchCaseStatement : IStatement, IBranchWrapperStatement
    {
        public bool CanBeEmbedded => true;
        public StatementInfo Info { get; }
        
        public EvaluationStatement SwitchTarget { get; }
        public ConditionalBlockStatement[] Cases { get; }
        public IStatement DefaultCase { get; }
        
        public IStatement[] TraversableChildren { get; }
        
        public IStatement[] Branches { get; }
        
        public SwitchCaseStatement(EvaluationStatement switchTarget, ConditionalBlockStatement[] cases, StatementInfo info)
        {
            SwitchTarget = switchTarget;
            Cases = cases;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(new IStatement[] {switchTarget}.Union(cases).ToArray());
            Branches = cases.Select(x => x.Statement).ToArray();
        }
        
        public SwitchCaseStatement(EvaluationStatement switchTarget, ConditionalBlockStatement[] cases, IStatement defaultCase, StatementInfo info)
        {
            SwitchTarget = switchTarget;
            Cases = cases;
            DefaultCase = defaultCase;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(new IStatement[] {switchTarget}.Union(cases).Union(new []{defaultCase}).ToArray());
            Branches = cases.Select(x => x.Statement).ToArray();
        }
    }
}