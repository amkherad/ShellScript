using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Library.IO.Directory
{
    public partial class ApiDirectory
    {
        public abstract class Exists : ApiBaseFunction
        {
            public override string Name => nameof(Exists);
            public override string Summary => "Checks whether a directory exists.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {DirectoryPathParameter};
        }

        public abstract class Create : ApiBaseFunction
        {
            public override string Name => nameof(Create);
            public override string Summary => "Creates a directory and any parent directories.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {DirectoryPathParameter};
        }

        public abstract class Delete : ApiBaseFunction
        {
            public override string Name => nameof(Delete);
            public override string Summary => "Deletes an empty directory.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {DirectoryPathParameter};
        }

        public abstract class DeleteRecursive : ApiBaseFunction
        {
            public override string Name => nameof(DeleteRecursive);
            public override string Summary => "Recursively deletes a directory and its contents.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {DirectoryPathParameter};
        }

        public abstract class GetFiles : ApiBaseFunction
        {
            public override string Name => nameof(GetFiles);
            public override string Summary => "Lists files in a directory (non-recursive).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor =>
                new TypeDescriptor(DataTypes.String | DataTypes.Array);
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {DirectoryPathParameter};
        }
    }
}
