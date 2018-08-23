using System;
using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionStatement : IStatement
    {
        public bool IsBlockStatement => true;

        public string Name { get; }
        public BlockStatement Block { get; }
        public FunctionParameterDefinitionStatement[] Parameters { get; }


        public FunctionStatement(string name, FunctionParameterDefinitionStatement[] parameters, BlockStatement block)
        {
            Name = name;
            Block = block;
            Parameters = parameters ?? new FunctionParameterDefinitionStatement[0];
        }

        public override string ToString()
        {
            return
                $"function {Name}({string.Join(',', (IEnumerable<FunctionParameterDefinitionStatement>) Parameters)}) {{{Environment.NewLine}{Block}{Environment.NewLine}}}";
        }


        public IEnumerable<IStatement> TraversableChildren
        {
            get
            {
                yield return Block;
                foreach (var p in Parameters)
                    yield return p;
            }
        }
    }
}