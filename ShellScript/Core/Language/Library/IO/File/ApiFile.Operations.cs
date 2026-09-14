using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.File
{
    public partial class ApiFile
    {
        public abstract class ReadAllText : ApiBaseFunction
        {
            public override string Name => nameof(ReadAllText);
            public override string Summary => "Reads the entire contents of a file.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {FilePathParameter};
        }

        public abstract class WriteAllText : ApiBaseFunction
        {
            public override string Name => nameof(WriteAllText);
            public override string Summary => "Creates or overwrites a file with the given text.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                FilePathParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Contents", null, null)
            };
        }

        public abstract class AppendAllText : ApiBaseFunction
        {
            public override string Name => nameof(AppendAllText);
            public override string Summary => "Appends text to a file.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                FilePathParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Contents", null, null)
            };
        }

        public abstract class Delete : ApiBaseFunction
        {
            public override string Name => nameof(Delete);
            public override string Summary => "Deletes a file.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {FilePathParameter};
        }

        public abstract class Copy : ApiBaseFunction
        {
            public override string Name => nameof(Copy);
            public override string Summary => "Copies a file to a new location.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                FilePathParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "DestinationPath", null, null)
            };
        }

        public abstract class Move : ApiBaseFunction
        {
            public override string Name => nameof(Move);
            public override string Summary => "Moves or renames a file.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                FilePathParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "DestinationPath", null, null)
            };
        }

        public abstract class GetLength : ApiBaseFunction
        {
            public override string Name => nameof(GetLength);
            public override string Summary => "Returns the size of a file in bytes.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {FilePathParameter};
        }
    }
}
