using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Data.Xml
{
    public abstract partial class ApiXml : ApiBaseClass
    {
        public const string ClassAccessName = "Xml";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class GetPath : ApiBaseFunction
        {
            public override string Name => nameof(GetPath);
            public override string Summary => "Reads text from XML using an XPath expression.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "XmlText", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "XPath", null, null)
            };
        }

        public abstract class IsValid : ApiBaseFunction
        {
            public override string Name => nameof(IsValid);
            public override string Summary => "Checks whether text is well-formed XML.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "XmlText", null, null)
            };
        }
    }
}
