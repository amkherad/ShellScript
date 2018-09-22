using System.Linq;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionCallStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => true;
        public override StatementInfo Info { get; }

        /// <summary>
        /// The name of the instance of the method or class's name itself.
        /// </summary>
        public string ObjectName { get; }

        public string FunctionName { get; }

        public virtual string Fqn
        {
            get
            {
                if (ObjectName != null)
                {
                    return $"{ObjectName}_{FunctionName}";
                }

                return FunctionName;
            }
        }

        public DataTypes DataType { get; }
        public EvaluationStatement[] Parameters { get; }


        public FunctionCallStatement(string objectName, string functionName, DataTypes dataType,
            EvaluationStatement[] parameters, StatementInfo info)
        {
            ObjectName = objectName;
            FunctionName = functionName;
            Parameters = parameters;
            Info = info;
            DataType = dataType;

            TraversableChildren = StatementHelpers.CreateChildren(parameters.Cast<IStatement>().ToArray());
        }
    }
}