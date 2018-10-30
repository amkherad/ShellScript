using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Compiler.CompilerErrors
{
    public class IdentifierNameExistsCompilerException : CompilerException
    {
        public string VariableName { get; }
        
        
        public IdentifierNameExistsCompilerException(string variableName, StatementInfo info)
            : base(CreateMessage(variableName, info), info)
        {
            VariableName = variableName;
        }
        
        public IdentifierNameExistsCompilerException(VariableAccessStatement variableAccessStatement)
            : base(CreateMessage(variableAccessStatement.VariableName, variableAccessStatement.Info), variableAccessStatement.Info)
        {
        }
        
        public IdentifierNameExistsCompilerException(FunctionCallStatement functionCallStatement)
            : base(CreateMessage(functionCallStatement.Fqn, functionCallStatement.Info), functionCallStatement.Info)
        {
        }

        public static string CreateMessage(string variableName, StatementInfo info)
        {
            return $"Identifier name '{variableName}' already exists in current scope {info}";
        }
//        public static string CreateMessage(string variableName)
//        {
//            return $"Identifier name '{variableName}' already exists in current scope.";
//        }
    }
}