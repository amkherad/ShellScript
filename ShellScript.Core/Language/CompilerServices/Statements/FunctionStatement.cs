using System;
using System.Collections.Generic;
using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionStatement : IStatement
    {
        public bool IsBlockStatement => true;
        public ParserInfo ParserInfo { get; }

        public string Name { get; }
        public BlockStatement Block { get; }
        public FunctionParameterDefinitionStatement[] Parameters { get; }
        
        
        public FunctionStatement(string name, FunctionParameterDefinitionStatement[] parameters, BlockStatement block, ParserInfo parserInfo)
        {
            Name = name;
            Block = block;
            ParserInfo = parserInfo;
            Parameters = parameters ?? new FunctionParameterDefinitionStatement[0];
        }

        public override string ToString()
        {
            return $"function {Name}({string.Join(',', (IEnumerable<FunctionParameterDefinitionStatement>) Parameters)}) {{{Environment.NewLine}{Block}{Environment.NewLine}}}";
        }
    }
}