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

        public TypeDescriptor TypeDescriptor { get; }
        public EvaluationStatement[] Parameters { get; }


        public FunctionCallStatement(string objectName, string functionName, TypeDescriptor typeDescriptor,
            EvaluationStatement[] parameters, StatementInfo info, IStatement parentStatement = null)
        {
            ObjectName = objectName;
            FunctionName = functionName;
            Parameters = parameters;
            Info = info;
            TypeDescriptor = typeDescriptor;
            ParentStatement = parentStatement;

            // ReSharper disable once CoVariantArrayConversion
            TraversableChildren = StatementHelpers.CreateChildren(parameters);
        }

        public override string ToString()
        {
            var name = string.IsNullOrWhiteSpace(ObjectName)
                ? FunctionName
                : $"{ObjectName}.{FunctionName}";
            
            if (Parameters != null && Parameters.Length > 0)
            {
                return $"{name}({string.Join(',', Parameters.Select(p => p.ToString()))})";
            }
            
            return $"{name}()";
        }
    }
}