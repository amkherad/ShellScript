using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public class TypeCastOperator : IOperator
    {
        public bool CanBeEmbedded => false;
        public StatementInfo Info { get; }
        public OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        public int Order => 65;
        
        
        public TypeDescriptor TypeDescriptor { get; }
        
        
        public IStatement[] TraversableChildren { get; }
        
        public TypeCastOperator(TypeDescriptor typeDescriptor, StatementInfo info)
        {
            TypeDescriptor = typeDescriptor;
            Info = info;

            TraversableChildren = StatementHelpers.EmptyStatements;
        }
    }
}