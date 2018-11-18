using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class TypeCastStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }
        
        public TypeDescriptor TypeDescriptor { get; }
        public EvaluationStatement Target { get; }
        
        
        public TypeCastStatement(TypeDescriptor typeDescriptor, EvaluationStatement target, StatementInfo info)
        {
            TypeDescriptor = typeDescriptor;
            Target = target;
            Info = info;
        }
        
        public TypeCastStatement(TypeDescriptor typeDescriptor, EvaluationStatement target, StatementInfo info, IStatement parentStatement)
        {
            TypeDescriptor = typeDescriptor;
            Target = target;
            Info = info;

            ParentStatement = parentStatement;
        }

        public override string ToString()
        {
            return $"({TypeDescriptor})({Target})";
        }
    }
}