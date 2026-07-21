using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Locale
{
    public partial class ApiLocale
    {
        public abstract class GetCurrentLocale : ApiBaseFunction
        {
            public override string Name => nameof(GetCurrentLocale);
            public override string Summary => "Returns the current process locale.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }
    }
}