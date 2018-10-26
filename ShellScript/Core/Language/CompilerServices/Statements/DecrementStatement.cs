//namespace ShellScript.Core.Language.CompilerServices.Statements
//{
//    public class DecrementStatement : EvaluationStatement
//    {
//        public override bool CanBeEmbedded => true;
//        public override StatementInfo Info { get; }
//
//        public VariableAccessStatement Variable { get; }
//
//        public bool IsPostfix { get; }
//
//        public DecrementStatement(VariableAccessStatement variable, bool isPostfix, StatementInfo info)
//        {
//            Variable = variable;
//            IsPostfix = isPostfix;
//            Info = info;
//
//            TraversableChildren = StatementHelpers.CreateChildren(variable);
//        }
//
//        public override string ToString()
//        {
//            return IsPostfix
//                ? $"{Variable}--"
//                : $"--{Variable}";
//        }
//    }
//}