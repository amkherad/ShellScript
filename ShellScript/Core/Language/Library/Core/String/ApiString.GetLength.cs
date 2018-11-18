using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Core.Language.Library.Core.String
{
    public partial class ApiString
    {
        public abstract class GetLength : ApiBaseFunction
        {
            public override string Name => nameof(GetLength);
            public override string Summary => "Returns the length of the string. (may vary depending on current locale)";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter
            };
        }
    }
}