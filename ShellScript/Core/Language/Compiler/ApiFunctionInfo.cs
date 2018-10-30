using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler
{
    public class ApiFunctionInfo : FunctionInfo
    {
        public IApiFunc Function { get; }
        
        public ApiFunctionInfo(TypeDescriptor typeDescriptor, string name, string reName, string className, IApiFunc function, bool isParams,
            FunctionParameterDefinitionStatement[] parameters, bool byPassParameterValidation = false)
            : base(typeDescriptor, name, reName, className, isParams, parameters, null, byPassParameterValidation)
        {
            Function = function;
        }
    }
}