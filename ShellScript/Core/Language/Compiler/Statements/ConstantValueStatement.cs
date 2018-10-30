using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ConstantValueStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }

        public TypeDescriptor TypeDescriptor { get; }
        public string Value { get; }


        public ConstantValueStatement(TypeDescriptor typeDescriptor, string value,
            StatementInfo info)
        {
            TypeDescriptor = typeDescriptor;
            Value = value;
            Info = info;

            TraversableChildren = new IStatement[0];
        }

        public override string ToString()
        {
            return Value;
        }
    }
}