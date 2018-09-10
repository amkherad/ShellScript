using System.Linq;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class SwitchCaseStatement : IStatement
    {
        public bool IsBlockStatement => true;
        public StatementInfo Info { get; }
        
        public EvaluationStatement SwitchTarget { get; }
        public ConditionalBlockStatement[] Cases { get; }
        
        public IStatement[] TraversableChildren { get; }
        
        public SwitchCaseStatement(EvaluationStatement switchTarget, ConditionalBlockStatement[] cases, StatementInfo info)
        {
            SwitchTarget = switchTarget;
            Cases = cases;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(new IStatement[] {switchTarget}.Union(cases).ToArray());
        }
        
        public SwitchCaseStatement(EvaluationStatement switchTarget, ConditionalBlockStatement[] cases, IStatement defaultCase, StatementInfo info)
        {
            SwitchTarget = switchTarget;
            Cases = cases;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(new IStatement[] {switchTarget}.Union(cases).Union(new []{defaultCase}).ToArray());
        }
    }
}