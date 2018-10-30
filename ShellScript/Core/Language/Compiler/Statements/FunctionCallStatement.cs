using System.Linq;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class FunctionCallStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => true;
        public override StatementInfo Info { get; }

        /// <summary>
        /// The name of the instance of the method or class's name itself.
        /// </summary>
        public string ClassName { get; }

        public string FunctionName { get; }

        public virtual string Fqn
        {
            get
            {
                if (ClassName != null)
                {
                    return $"{ClassName}_{FunctionName}";
                }

                return FunctionName;
            }
        }

        public TypeDescriptor TypeDescriptor { get; }
        public EvaluationStatement[] Parameters { get; }


        public FunctionCallStatement(string className, string functionName, TypeDescriptor typeDescriptor,
            EvaluationStatement[] parameters, StatementInfo info, IStatement parentStatement = null)
        {
            ClassName = className;
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
            var name = string.IsNullOrWhiteSpace(ClassName)
                ? FunctionName
                : $"{ClassName}.{FunctionName}";
            
            if (Parameters != null && Parameters.Length > 0)
            {
                return $"{name}({string.Join(',', Parameters.Select(p => p.ToString()))})";
            }
            
            return $"{name}()";
        }
    }
}