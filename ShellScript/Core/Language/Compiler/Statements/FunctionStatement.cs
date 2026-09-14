using System.Linq;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class FunctionStatement : IStatement, IBlockWrapperStatement
    {
        public bool CanBeEmbedded => true;
        public StatementInfo Info { get; }

        public string Name { get; }
        public string ClassName { get; }
        public bool IsInstanceMethod { get; }
        public bool IsConstructor { get; }
        public IStatement Statement { get; }
        public FunctionParameterDefinitionStatement[] Parameters { get; }

        public bool IsParams { get; }

        public TypeDescriptor TypeDescriptor { get; }

        public IStatement[] TraversableChildren { get; protected set; }


        public FunctionStatement(TypeDescriptor typeDescriptor, string name,
            FunctionParameterDefinitionStatement[] parameters, IStatement statement, StatementInfo info,
            string className = null, bool isInstanceMethod = false, bool isConstructor = false)
        {
            TypeDescriptor = typeDescriptor;
            Name = name;
            ClassName = className;
            IsInstanceMethod = isInstanceMethod;
            IsConstructor = isConstructor;
            Statement = statement;
            Info = info;
            Parameters = parameters;

            if (parameters != null)
            {
                TraversableChildren =
                    StatementHelpers.CreateChildren(new IStatement[] {statement}.Union(parameters).ToArray());
            }
            else
            {
                TraversableChildren =
                    StatementHelpers.CreateChildren(statement);
            }
        }

        public override string ToString()
        {
            if (Parameters != null && Parameters.Length > 0)
            {
                return $"{TypeDescriptor} {Name}({string.Join(',', Parameters.Select(x => x.ToString()))}){{}}";
            }

            return $"{TypeDescriptor} {Name}(){{}}";
        }
    }
}