using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Shared.Api
{
    public class NativeResourceFunction : ApiBaseFunction
    {
        public override string Name { get; }
        public override string Summary { get; }
        public override string ClassName { get; }
        public override bool IsStatic { get; }
        public override bool AllowDynamicParams { get; }
        public override FunctionParameterDefinitionStatement[] Parameters { get; }

        public string ResourceName { get; set; }

        private FunctionInfo _functionInfo;

        public override TypeDescriptor TypeDescriptor { get; }


        public NativeResourceFunction(
            string className,
            string name,
            string resourceName,
            string summary,
            TypeDescriptor typeDescriptor,
            bool isParams,
            bool isStatic,
            params FunctionParameterDefinitionStatement[] parameters)
        {
            Name = name;
            ClassName = className;
            ResourceName = resourceName;
            TypeDescriptor = typeDescriptor;
            Parameters = parameters;
            Summary = summary;
            IsStatic = isStatic;
            AllowDynamicParams = isParams;

            _functionInfo = new FunctionInfo(typeDescriptor, name, null, className, isParams, parameters, null);
        }
        
        
        public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement)
        {
            return UseNativeResourceMethod(this, p, ResourceName, _functionInfo, functionCallStatement.Parameters,
                functionCallStatement.Info);
        }
    }
}