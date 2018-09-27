using ShellScript.Core.Language.CompilerServices.Lexing;

namespace ShellScript.Core.Language.CompilerServices.PreProcessors
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