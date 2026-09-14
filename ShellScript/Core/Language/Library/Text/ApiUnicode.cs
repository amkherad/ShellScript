using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Text
{
    public abstract partial class ApiUnicode : ApiBaseClass
    {
        public const string ClassAccessName = "Unicode";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class GetLength : ApiBaseFunction
        {
            public override string Name => nameof(GetLength);
            public override string Summary => "Returns Unicode character count (not byte length).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Text", null, null)
            };
        }
    }
}
