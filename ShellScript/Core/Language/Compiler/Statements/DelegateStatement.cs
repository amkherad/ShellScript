using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class DelegateStatement : IStatement
    {
        public bool CanBeEmbedded => false;
        public StatementInfo Info { get; }

        public string Name { get; }
        public TypeDescriptor ReturnTypeDescriptor { get; }
        public FunctionParameterDefinitionStatement[] Parameters { get; }

        public IStatement[] TraversableChildren { get; }

        public DelegateStatement(string name, TypeDescriptor returnTypeDescriptor, FunctionParameterDefinitionStatement[] parameters,
            StatementInfo info)
        {
            Name = name;
            ReturnTypeDescriptor = returnTypeDescriptor;
            Parameters = parameters;
            Info = info;

            TraversableChildren = StatementHelpers.EmptyStatements;
        }
    }
}