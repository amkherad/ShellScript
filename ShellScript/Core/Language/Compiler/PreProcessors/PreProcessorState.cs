using ShellScript.Core.Language.Compiler.Lexing;

namespace ShellScript.Core.Language.Compiler.PreProcessors
{
    public class PreProcessorState
    {
        public Token FirstToken { get; }
        
        public bool ConditionTaken { get; set; }
        
        public PreProcessorState(Token firstToken)
        {
            FirstToken = firstToken;
        }
    }
}