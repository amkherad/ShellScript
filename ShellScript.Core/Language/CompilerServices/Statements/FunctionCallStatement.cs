using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionCallStatement : IStatement
    {
        public bool IsBlockStatement => false;
        public ParserInfo ParserInfo { get; }
        
        public string SdkFunctionName { get; }
        public IStatement[] Parameters { get; }
        
        
        public FunctionCallStatement(string sdkFunctionName, IStatement[] parameters, ParserInfo parserInfo)
        {
            SdkFunctionName = sdkFunctionName;
            Parameters = parameters;
            ParserInfo = parserInfo;
        }
    }
}