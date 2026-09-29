using System;
using System.Collections.Generic;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler.PostProcessing;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Library;
using ShellScript.Windows.Batch.Api;
using ShellScript.Windows.Batch.PlatformTranspiler;

namespace ShellScript.Windows.Batch
{
    public class WindowsBatchPlatform : IPlatform
    {
        public const string LastStatusCodeStoreVariableName = "ERRORLEVEL";

        public string Name => "Windows-Batch";

        public string ScriptExtension => ".cmd";

        public ValueTuple<TypeDescriptor, string, string>[] CompilerConstants { get; } =
        {
            (TypeDescriptor.Boolean, "Windows", "true"),
            (TypeDescriptor.Boolean, "Batch", "true"),
        };

        public IApi Api { get; } = new WindowsBatchApi();

        public IPlatformMetaInfoTranspiler MetaInfoWriter { get; } = new BatchPlatformMetaInfoTranspiler();

        public IPlatformStatementTranspiler[] Transpilers { get; } =
        {
            new BatchAssignmentStatementTranspiler(),
            new BatchBlockStatementTranspiler(),
            new BatchFunctionCallStatementTranspiler(),
            new BatchEchoStatementTranspiler(),
            new BatchReturnStatementTranspiler(),
            new BatchThrowStatementTranspiler(),
            new BatchIfElseStatementTranspiler(),
            new BatchSwitchCaseStatementTranspiler(),
            new BatchWhileStatementTranspiler(),
            new BatchDoWhileStatementTranspiler(),
            new BatchForStatementTranspiler(),
            new BatchForEachStatementTranspiler(),
            new BatchVariableDefinitionStatementTranspiler(),
            new BatchEvaluationStatementTranspiler(),
            new BatchFunctionStatementTranspiler(),
            new BatchClassDeclarationStatementTranspiler(),
            new BatchDelegateStatementTranspiler(),
            new IncludeTranspilerBase(),
        };

        public CompilerFlags ReviseFlags(CompilerFlags flags)
        {
            flags.UseThirdPartyUtilities = false;
            flags.BindThirdPartyUtilitiesAtInit = false;
            flags.ExplicitEchoStream = null;
            flags.DefaultExplicitEchoStream = null;
            flags.UsePinElimination = true;
            return flags;
        }

        public IReadOnlyList<IGeneratedCodePostProcessor> GeneratedCodePostProcessors { get; } =
            Array.Empty<IGeneratedCodePostProcessor>();

        public string GetDefaultValue(DataTypes dataType)
        {
            if (dataType.IsArray())
            {
                return "()";
            }

            switch (dataType)
            {
                case DataTypes.Void:
                    throw new CompilerException("Void should not be used.", null);
                case DataTypes.Boolean:
                    return "0";
                case DataTypes.Integer:
                case DataTypes.Float:
                case DataTypes.Numeric:
                    return "0";
                case DataTypes.String:
                    return "\"\"";
                case DataTypes.Class:
                case DataTypes.Delegate:
                case DataTypes.Lookup:
                    return "0";
                default:
                    if (dataType.IsArray())
                    {
                        return null;
                    }

                    throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
            }
        }
    }
}
