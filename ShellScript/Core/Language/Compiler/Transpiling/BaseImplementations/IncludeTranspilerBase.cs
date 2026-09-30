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
            if (!(statement is IncludeStatement includeStatement))
            {
                message = "Invalid include statement.";
                return false;
            }

            if (!scope.IsRootScope)
            {
                message = "Include is only allowed at the root scope of a source file.";
                return false;
            }

            if (!(includeStatement.Target is ConstantValueStatement constant) || !constant.IsString())
            {
                message = "Include path must be a string literal.";
                return false;
            }

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
            var resolvedPath = ResolveIncludePath(context, fileName, includeStatement.Info);

            if (context.CompletedIncludeSources.Contains(resolvedPath))
            {
                return;
            }

            Compiler.CompileFromSource(context, metaWriter, writer, false, resolvedPath);
        }

        internal static string ResolveIncludePath(Context context, string fileName, StatementInfo errorInfo)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new CompilerException("Include path cannot be empty.", errorInfo);
            }

            if (Path.IsPathRooted(fileName) && File.Exists(fileName))
            {
                return Path.GetFullPath(fileName);
            }

            var searched = new List<string>();

            foreach (var dir in context.IncludeDirectoryStack)
            {
                if (string.IsNullOrEmpty(dir))
                {
                    continue;
                }

                var candidate = Path.Combine(dir, fileName);
                searched.Add(candidate);
                if (File.Exists(candidate))
                {
                    return Path.GetFullPath(candidate);
                }
            }

            foreach (var dir in context.Includes)
            {
                if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                {
                    continue;
                }

                var candidate = Path.Combine(dir, fileName);
                if (searched.Contains(candidate))
                {
                    continue;
                }

                searched.Add(candidate);
                if (File.Exists(candidate))
                {
                    return Path.GetFullPath(candidate);
                }
            }

            if (File.Exists(fileName))
            {
                return Path.GetFullPath(fileName);
            }

            searched.Add(fileName);

            var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(assemblyDir))
            {
                var candidate = Path.Combine(assemblyDir, fileName);
                searched.Add(candidate);
                if (File.Exists(candidate))
                {
                    return Path.GetFullPath(candidate);
                }
            }

            throw new CompilerException(
                "Include file not found: '" + fileName + "'. Searched: " + string.Join(", ", searched),
                errorInfo);
        }
    }
}
