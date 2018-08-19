using System;
using ShellScript.Core.Helpers;
using ShellScript.Core.Language.Compiler.Lexing;
using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Compiler.Parsing
{
    public partial class Parser
    {
        public IStatement ReadClass(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            throw new NotImplementedException();
        }
        
        public IStatement ReadFunction(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (!enumerator.MoveNext())
                return null;
        }
        
        public IStatement ReadIf(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            
            throw new NotImplementedException();
        }
    }
}