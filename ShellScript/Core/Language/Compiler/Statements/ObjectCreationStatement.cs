using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ObjectCreationStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => true;
        public override StatementInfo Info { get; }

        public TypeDescriptor ClassType { get; }
        public EvaluationStatement[] Arguments { get; }

        public ObjectCreationStatement(TypeDescriptor classType, EvaluationStatement[] arguments, StatementInfo info)
        {
            ClassType = classType;
            Arguments = arguments;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(arguments);
        }

        public string ClassName => ClassType.Lookup?.Name;
    }
}
