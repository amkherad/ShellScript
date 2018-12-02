using System;
using System.Collections.Generic;
using System.IO;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations
{
    public class IncludeTranspilerBase : IPlatformStatementTranspiler
    {
        public Type StatementType => typeof(IncludeStatement);

        public bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return false;
        }

        public bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            message = null;
            return true;
        }

        public void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotImplementedException();
        }

        public void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is IncludeStatement includeStatement))
            {
                throw new InvalidStatementStructureCompilerException(statement, statement.Info);
            }

            if (!scope.IsRootScope)
            {
                throw new CompilerException("Invalid use of include statement in inner scopes.", statement.Info);
            }

            var transpiler = context.GetEvaluationTranspilerForStatement(includeStatement.Target);
            var result = transpiler.GetExpression(context, scope, metaWriter, writer, null, includeStatement.Target);
            if (!(result.Template is ConstantValueStatement constantValueStatement))
            {
                throw new InvalidStatementStructureCompilerException(includeStatement.Target,
                    includeStatement.Target.Info);
            }

            var fileName = constantValueStatement.Value;

            if (!Path.IsPathRooted(fileName))
            {
                if (!File.Exists(fileName))
                {
                    var directories = new List<string>
                    {
                        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase),
                    };
                    directories.AddRange(context.Includes);

                    foreach (var dir in directories)
                    {
                        if (File.Exists(Path.Combine(dir, fileName)))
                        {
                            fileName = Path.Combine(dir, fileName);
                            break;
                        }
                    }

                    if (!File.Exists(fileName))
                    {
                        throw new FileNotFoundException();
                    }
                }
            }

            Compiler.CompileFromSource(context, metaWriter, writer, false, fileName);
        }
    }
}