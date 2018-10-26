using System.IO;
using ShellScript.Core.Helpers;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders
{
    public class ExpressionBuilderParams
    {
        public Context Context { get; }
        public Scope Scope { get; }
        public TextWriter MetaWriter { get; }
        public TextWriter NonInlinePartWriter { get; }

        public bool FormatString { get; set; } = true;
        public bool VoidFunctionCall { get; set; }
        
        [CanBeNull]
        public IStatement UsageContext { get; }

        public ExpressionBuilderParams(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement usageContext)
        {
            Context = context;
            Scope = scope;
            MetaWriter = metaWriter;
            NonInlinePartWriter = nonInlinePartWriter;
            UsageContext = usageContext;
        }

        public ExpressionBuilderParams(ExpressionBuilderParams p)
        {
            Context = p.Context;
            Scope = p.Scope;
            MetaWriter = p.MetaWriter;
            NonInlinePartWriter = p.NonInlinePartWriter;
            UsageContext = p.UsageContext;
        }

        public ExpressionBuilderParams(ExpressionBuilderParams p, TextWriter nonInlinePartWriter)
        {
            Context = p.Context;
            Scope = p.Scope;
            MetaWriter = p.MetaWriter;
            NonInlinePartWriter = nonInlinePartWriter;
            UsageContext = p.UsageContext;
        }
    }
}