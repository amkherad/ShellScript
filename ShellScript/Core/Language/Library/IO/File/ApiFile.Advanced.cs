using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Library.IO.File
{
    public partial class ApiFile
    {
        public abstract class ReadAllLines : ApiBaseFunction
        {
            public override string Name => nameof(ReadAllLines);
            public override string Summary => "Reads all lines of a file.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor =>
                new TypeDescriptor(DataTypes.String | DataTypes.Array);
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {FilePathParameter};
        }

        public abstract class WriteAllLines : ApiBaseFunction
        {
            public override string Name => nameof(WriteAllLines);
            public override string Summary => "Writes lines to a file.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                FilePathParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.Void, "Lines", null, null, true)
            };
        }

        public abstract class ReadAllBytesBase64 : ApiBaseFunction
        {
            public override string Name => nameof(ReadAllBytesBase64);
            public override string Summary => "Reads a file and returns base64-encoded contents.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {FilePathParameter};
        }

        public abstract class WriteAllBytesBase64 : ApiBaseFunction
        {
            public override string Name => nameof(WriteAllBytesBase64);
            public override string Summary => "Writes base64-encoded binary data to a file.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                FilePathParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Base64Data", null, null)
            };
        }

        public abstract class GetLastWriteTime : ApiBaseFunction
        {
            public override string Name => nameof(GetLastWriteTime);
            public override string Summary => "Returns last modification time as Unix seconds.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {FilePathParameter};
        }

        public abstract class CreateSymbolicLink : ApiBaseFunction
        {
            public override string Name => nameof(CreateSymbolicLink);
            public override string Summary => "Creates a symbolic link.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "LinkPath", null, null),
                FilePathParameter
            };
        }
    }
}
