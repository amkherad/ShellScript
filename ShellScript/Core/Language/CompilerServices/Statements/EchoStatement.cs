using System.Linq;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class EchoStatement : IStatement
    {
        public bool CanBeEmbedded => true;
        public StatementInfo Info { get; }
        public IStatement[] TraversableChildren { get; }
        
        public EvaluationStatement[] Parameters { get; }

        
        public EchoStatement(
            EvaluationStatement[] parameters, StatementInfo info)
        {
            Parameters = parameters;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(parameters.Cast<IStatement>().ToArray());
        }

        public override string ToString()
        {
            if (Parameters != null && Parameters.Length > 0)
            {
                return $"echo ({string.Join(',', Parameters?.Select(p => p.ToString()))})";
            }

            return "echo CRLF";
        }
    }
}