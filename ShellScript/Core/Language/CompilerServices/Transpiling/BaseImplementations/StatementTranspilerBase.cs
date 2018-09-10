using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
{
    public abstract class StatementTranspilerBase : IPlatformStatementTranspiler
    {
        public abstract Type StatementType { get; }

        public abstract bool CanInline(Context context, Scope scope, IStatement statement);

        public virtual bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            //TODO: validate all paths must return a value

            message = default;
            return true;
        }

        public abstract void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement);

        public abstract void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement);
    }
}