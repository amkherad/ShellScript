using System;
using System.IO;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashForEachStatementTranspiler : StatementTranspilerBase
    {
        public override Type StatementType => typeof(ForEachStatement);
        public override bool CanInline(Context context, Scope scope, IStatement statement) => false;

        public override bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            var loop = statement as ForEachStatement;
            if (loop == null || !loop.Iterator.GetDataType(context, scope).IsArray())
            {
                message = "The foreach iterator must be an array.";
                return false;
            }
            return base.Validate(context, scope, statement, out message);
        }

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement) => throw new NotSupportedException();

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            var loop = (ForEachStatement) statement;
            var loopScope = scope.BeginNewScope(ScopeType.Block);
            var iteratorType = loop.Iterator.GetDataType(context, scope);
            var elementType = new TypeDescriptor(iteratorType.DataType & ~DataTypes.Array);
            string variableName;

            var definition = loop.Variable as VariableDefinitionStatement;
            if (definition != null)
            {
                if (!StatementHelpers.IsAssignableFrom(context, loopScope, definition.TypeDescriptor, elementType))
                    throw new TypeMismatchCompilerException(elementType, definition.TypeDescriptor, definition.Info);
                variableName = definition.Name;
                loopScope.ReserveNewVariable(definition.TypeDescriptor, variableName);
            }
            else
            {
                var access = loop.Variable as VariableAccessStatement;
                VariableInfo info;
                if (access == null || !loopScope.TryGetVariableInfo(access, out info))
                    throw new IdentifierNotFoundCompilerException(access);
                variableName = info.AccessName;
            }

            string iteratorName;
            var iteratorAccess = loop.Iterator as VariableAccessStatement;
            if (iteratorAccess != null)
            {
                VariableInfo info;
                if (!scope.TryGetVariableInfo(iteratorAccess, out info))
                    throw new IdentifierNotFoundCompilerException(iteratorAccess);
                iteratorName = info.AccessName;
            }
            else
            {
                var functionCall = loop.Iterator as FunctionCallStatement;
                FunctionInfo functionInfo;
                if (functionCall == null || !scope.TryGetFunctionInfo(functionCall, out functionInfo))
                    throw new InvalidStatementStructureCompilerException(loop.Iterator, loop.Iterator.Info);
                context.GetTranspilerForStatement(functionCall).WriteBlock(
                    context, scope, writer, metaWriter, functionCall);
                iteratorName = context.GetLastFunctionCallStorageVariable(functionInfo.TypeDescriptor, metaWriter);
            }

            writer.Write("for ");
            writer.Write(variableName);
            writer.Write(" in \"${");
            writer.Write(iteratorName);
            writer.WriteLine("[@]}\"");
            writer.WriteLine("do");
            BashBlockStatementTranspiler.WriteBlockStatement(
                context, loopScope, writer, metaWriter, loop.Statement, ScopeType.Block, false);
            writer.WriteLine("done");
            scope.IncrementStatements();
        }
    }
}
