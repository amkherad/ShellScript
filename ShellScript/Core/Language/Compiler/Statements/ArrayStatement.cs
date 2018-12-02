using System.Text;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ArrayStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }
        
        public TypeDescriptor Type { get; }
        public EvaluationStatement Length { get; }
        public EvaluationStatement[] Elements { get; }
        
        public ArrayStatement(TypeDescriptor type, EvaluationStatement length, EvaluationStatement[] elements, StatementInfo info)
        {
            Info = info;
            Type = type;
            Length = length;
            Elements = elements;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("new ");
            sb.Append(Type.DataType ^ DataTypes.Array);
            sb.Append("[");
            sb.Append(Length);
            sb.Append(']');
            if (Elements != null)
            {
                sb.Append(" {");

                bool isFirstLine = true;
                foreach (var elem in Elements)
                {
                    if (!isFirstLine)
                    {
                        sb.AppendLine(",");
                    }

                    sb.Append(elem);
                    
                    isFirstLine = false;
                }

                sb.AppendLine();
                sb.Append('}');
            }

            return sb.ToString();
        }
    }
}