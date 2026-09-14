using System.Linq;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ClassDeclarationStatement : IStatement
    {
        public bool CanBeEmbedded => false;
        public StatementInfo Info { get; }

        public string Name { get; }
        public VariableDefinitionStatement[] Fields { get; }
        public FunctionStatement[] Methods { get; }

        public IStatement[] TraversableChildren { get; }

        public ClassDeclarationStatement(string name, VariableDefinitionStatement[] fields,
            FunctionStatement[] methods, StatementInfo info)
        {
            Name = name;
            Fields = fields ?? new VariableDefinitionStatement[0];
            Methods = methods ?? new FunctionStatement[0];
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(
                fields.Cast<IStatement>().Concat(methods).ToArray());
        }
    }
}
