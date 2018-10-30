using System.Collections.Generic;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ForStatement : ConditionalBlockStatement, IBlockWrapperStatement
    {
        public IStatement[] PreLoopAssignment { get; }
        public IStatement[] AfterLoopEvaluations { get; }
        
        public ForStatement(IStatement[] preLoopAssignment , EvaluationStatement condition, IStatement[] afterLoopEvaluations, IStatement statement, StatementInfo info)
            : base(condition, statement, info)
        {
            PreLoopAssignment = preLoopAssignment;
            AfterLoopEvaluations = afterLoopEvaluations;

            var children = new List<IStatement>();
            if (preLoopAssignment != null)
            {
                children.AddRange(preLoopAssignment);
            }
            if (condition != null)
            {
                children.Add(condition);
            }
            if (afterLoopEvaluations != null)
            {
                children.AddRange(afterLoopEvaluations);
            }
            if (statement != null)
            {
                children.Add(statement);
            }
            
            TraversableChildren = children.ToArray();
        }

        public override string ToString()
        {
            return $"for({PreLoopAssignment}; {Condition}; {AfterLoopEvaluations})";
        }
    }
}