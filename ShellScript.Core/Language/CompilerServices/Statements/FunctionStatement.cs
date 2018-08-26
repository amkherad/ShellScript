using System;
using System.Collections.Generic;
using System.Linq;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionStatement : IStatement
    {
        public bool IsBlockStatement => true;
        public StatementInfo Info { get; }

        public string Name { get; }
        public BlockStatement Block { get; }
        public FunctionParameterDefinitionStatement[] Parameters { get; }

        public IStatement[] TraversableChildren { get; protected set; }


        public FunctionStatement(string name, FunctionParameterDefinitionStatement[] parameters, BlockStatement block, StatementInfo info)
        {
            Name = name;
            Block = block;
            Info = info;
            Parameters = parameters;

            if (parameters != null)
            {
                TraversableChildren =
                    StatementHelpers.CreateChildren(new IStatement[] {block}.Union(parameters).ToArray());
            }
            else
            {
                TraversableChildren =
                    StatementHelpers.CreateChildren(block);
            }
        }

        public override string ToString()
        {
            return
                $"function {Name}({string.Join(',', (IEnumerable<FunctionParameterDefinitionStatement>) Parameters)}) {{{Environment.NewLine}{Block}{Environment.NewLine}}}";
        }
    }
}