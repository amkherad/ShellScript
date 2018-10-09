using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Shared.Api
{
    public class ShellScriptResourceFunction : ApiBaseFunction
    {
        public override string Name { get; }
        public override string Summary { get; }
        public override string ClassName { get; }
        public override bool IsStatic { get; }
        public override bool AllowDynamicParams { get; }
        public override FunctionParameterDefinitionStatement[] Parameters { get; }

        public string ResourceName { get; set; }

        private FunctionInfo _functionInfo;

        public override DataTypes DataType { get; }


        public ShellScriptResourceFunction(
            string className,
            string name,
            string resourceName,
            string summary,
            DataTypes dataType,
            bool isParams,
            bool isStatic,
            FunctionParameterDefinitionStatement[] parameters)
        {
            Name = name;
            ClassName = className;
            ResourceName = resourceName;
            DataType = dataType;
            Parameters = parameters;
            Summary = summary;
            IsStatic = isStatic;
            AllowDynamicParams = isParams;

            _functionInfo = new FunctionInfo(dataType, name, null, className, isParams, parameters, null);
        }
        
        
        public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement)
        {
            return RecursiveCompileResource(this, p, ResourceName, _functionInfo, functionCallStatement.Parameters,
                functionCallStatement.Info);
        }
    }
}