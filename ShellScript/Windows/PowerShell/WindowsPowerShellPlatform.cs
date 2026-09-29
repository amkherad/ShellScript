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
using ShellScript.Windows.PowerShell.Api;
using ShellScript.Windows.PowerShell.PlatformTranspiler;

namespace ShellScript.Windows.PowerShell
{
    public class WindowsPowerShellPlatform : IPlatform
    {
        public const string LastStatusCodeStoreVariableName = "LASTEXITCODE";

        public string Name => "Windows-PowerShell";

        public string ScriptExtension => ".ps1";

        public ValueTuple<TypeDescriptor, string, string>[] CompilerConstants { get; } =
        {
            (TypeDescriptor.Boolean, "Windows", "true"),
            (TypeDescriptor.Boolean, "PowerShell", "true"),
        };

        public IApi Api { get; } = new WindowsPowerShellApi();

        public IPlatformMetaInfoTranspiler MetaInfoWriter { get; } = new PowerShellPlatformMetaInfoTranspiler();

        public IPlatformStatementTranspiler[] Transpilers { get; } =
        {
            new PowerShellAssignmentStatementTranspiler(),
            new PowerShellBlockStatementTranspiler(),
            new PowerShellFunctionCallStatementTranspiler(),
            new PowerShellEchoStatementTranspiler(),
            new PowerShellReturnStatementTranspiler(),
            new PowerShellThrowStatementTranspiler(),
            new PowerShellIfElseStatementTranspiler(),
            new PowerShellSwitchCaseStatementTranspiler(),
            new PowerShellWhileStatementTranspiler(),
            new PowerShellDoWhileStatementTranspiler(),
            new PowerShellForStatementTranspiler(),
            new PowerShellForEachStatementTranspiler(),
            new PowerShellVariableDefinitionStatementTranspiler(),
            new PowerShellEvaluationStatementTranspiler(),
            new PowerShellFunctionStatementTranspiler(),
            new PowerShellClassDeclarationStatementTranspiler(),
            new PowerShellDelegateStatementTranspiler(),
            new IncludeTranspilerBase(),
        };

        public CompilerFlags ReviseFlags(CompilerFlags flags)
        {
            flags.UseThirdPartyUtilities = false;
            flags.BindThirdPartyUtilitiesAtInit = false;
            flags.ExplicitEchoStream = null;
            flags.DefaultExplicitEchoStream = null;
            return flags;
        }

        public IReadOnlyList<IGeneratedCodePostProcessor> GeneratedCodePostProcessors { get; } =
            Array.Empty<IGeneratedCodePostProcessor>();

        public string GetDefaultValue(DataTypes dataType)
        {
            if (dataType.IsArray())
            {
                return "@()";
            }

            switch (dataType)
            {
                case DataTypes.Void:
                    throw new CompilerException("Void should not be used.", null);
                case DataTypes.Boolean:
                    return "$false";
                case DataTypes.Integer:
                case DataTypes.Float:
                case DataTypes.Numeric:
                    return "0";
                case DataTypes.String:
                    return "''";
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
