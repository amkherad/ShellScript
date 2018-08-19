using ShellScript.Core.Language.Compiler.Parsing;

namespace ShellScript.Core.Language.Compiler.Statements
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