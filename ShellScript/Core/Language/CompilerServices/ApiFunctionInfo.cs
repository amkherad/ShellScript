using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices
{
    public class ApiFunctionInfo : FunctionInfo
    {
        public IApiFunc Function { get; }
        
        public ApiFunctionInfo(DataTypes dataType, string name, string reName, string objectName, IApiFunc function, bool isParams,
            FunctionParameterDefinitionStatement[] parameters, bool byPassParameterValidation = false)
            : base(dataType, name, reName, objectName, isParams, parameters, null, byPassParameterValidation)
        {
            Function = function;
        }
    }
}