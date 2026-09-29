using System;
using System.Collections.Generic;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api;
using ShellScript.Core.Language.Compiler.PostProcessing;
using ShellScript.Unix.Bash.PlatformTranspiler;
using ShellScript.Unix.Bash.PostProcessing;

namespace ShellScript.Unix.Bash
{
    public class UnixBashPlatform : IPlatform
    {
        public const string LastStatusCodeStoreVariableName = "?";
        public const string PlatformName = "Unix-Bash";

        private static readonly IGeneratedCodePostProcessor[] PostProcessors =
        {
            new FormatBashGeneratedCodePostProcessor(),
        };

        public string Name => PlatformName;

        public string ScriptExtension => ".bash";

        public ValueTuple<TypeDescriptor, string, string>[] CompilerConstants { get; } =
        {
            (TypeDescriptor.Boolean, "Unix", "true"),
            (TypeDescriptor.Boolean, "Bash", "true"),
        };

        public IApi Api { get; } = new UnixBashApi();

        public IPlatformMetaInfoTranspiler MetaInfoWriter { get; } = new BashPlatformMetaInfoTranspiler();

        public IPlatformStatementTranspiler[] Transpilers { get; } =
        {
            new BashAssignmentStatementTranspiler(),
            new BashBlockStatementTranspiler(),
            new BashFunctionCallStatementTranspiler(),
            new BashEchoStatementTranspiler(),
            new BashReturnStatementTranspiler(),
            new BashThrowStatementTranspiler(),
            new BashIfElseStatementTranspiler(),
            new BashSwitchCaseStatementTranspiler(),
            new BashWhileStatementTranspiler(),
            new BashDoWhileStatementTranspiler(),
            new BashForStatementTranspiler(),
            new BashForEachStatementTranspiler(),
            new BashVariableDefinitionStatementTranspiler(),
            new BashEvaluationStatementTranspiler(),
            new BashFunctionStatementTranspiler(),
            new BashClassDeclarationStatementTranspiler(),
            new BashDelegateStatementTranspiler(),
            
            new IncludeTranspilerBase()
        };
        
        public CompilerFlags ReviseFlags(CompilerFlags flags)
        {
            return flags;
        }

        public IReadOnlyList<IGeneratedCodePostProcessor> GeneratedCodePostProcessors => PostProcessors;

        public string GetDefaultValue(DataTypes dataType)
        {
            if (dataType.IsArray())
            {
                return "()";
            }
            
            switch (dataType)
            {
                case DataTypes.Void:
                {
                    throw new CompilerException("Void should not be used.", null);
                }
                case DataTypes.Boolean:
                {
                    return "0";
                }
                case DataTypes.Integer:
                case DataTypes.Float:
                case DataTypes.Numeric:
                {
                    return "0";
                }
                case DataTypes.String:
                {
                    return "\"\"";
                }
                case DataTypes.Class:
                {
                    return "0";
                }
                case DataTypes.Delegate:
                {
                    return "0";
                }
                case DataTypes.Lookup:
                {
                    return "0";
                }
                default:
                {
                    if (dataType.IsArray())
                    {
                        return null;
                    }

                    throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
                }
            }
        }
    }
}