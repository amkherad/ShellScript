using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Library.Text
{
    public abstract partial class ApiText : ApiBaseClass
    {
        public const string ClassAccessName = "Text";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class NormalizeWhitespace : ApiBaseFunction
        {
            public override string Name => nameof(NormalizeWhitespace);
            public override string Summary => "Collapses whitespace runs to single spaces and trims.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Text", null, null)
            };
        }

        public abstract class Split : ApiBaseFunction
        {
            public override string Name => nameof(Split);
            public override string Summary => "Splits text by delimiter into an array.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor =>
                new TypeDescriptor(DataTypes.String | DataTypes.Array);
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Text", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Delimiter", null, null)
            };
        }
    }
}
