using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Diagnostics;
using ShellScript.Windows.PowerShell.Api.ClassLibrary.Base;

namespace ShellScript.Windows.PowerShell.Api.ClassLibrary.Diagnostics
{
    public partial class PowerShellConsole : ApiConsole
    {
        public override IApiFunc[] Functions { get; } =
        {
            new PowerShellWriteLine(),
            new PowerShellWriteError(),
            new PowerShellReadLine(),
            new PowerShellReadText(),
            new PowerShellReadKey(),
        };

        public class PowerShellWriteLine : WriteLine
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                PowerShellApiNative.Native(this, p, call, ClassAccessName, "Write-Output $args[0]");
        }

        public class PowerShellWriteError : WriteError
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                PowerShellApiNative.Native(this, p, call, ClassAccessName,
                    "[Console]::Error.WriteLine($args[0])");
        }

        public class PowerShellReadLine : ReadLine
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                PowerShellApiNative.Native(this, p, call, ClassAccessName, "[Console]::ReadLine()");
        }

        public class PowerShellReadText : ReadText
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                PowerShellApiNative.Native(this, p, call, ClassAccessName,
                    "Write-Host -NoNewline $args[0]; [Console]::ReadLine()");
        }

        public class PowerShellReadKey : ReadKey
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                PowerShellApiNative.Native(this, p, call, ClassAccessName,
                    "$k = $Host.UI.RawUI.ReadKey($(if ($args[0]) { 'IncludeKeyDown' } else { 'NoEcho' })); $k.Character");
        }
    }
}
