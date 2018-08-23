using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionCallStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
        
        /// <summary>
        /// The name of the instance of the method or class's name itself.
        /// </summary>
        public string ObjectName { get; }
        public string FunctionName { get; }
        public EvaluationStatement[] Parameters { get; }
        
        
        public FunctionCallStatement(string objectName, string functionName, EvaluationStatement[] parameters)
        {
            ObjectName = objectName;
            FunctionName = functionName;
            Parameters = parameters;
        }


        public override IEnumerable<IStatement> TraversableChildren => Parameters;
    }
}