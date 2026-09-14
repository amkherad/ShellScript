using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Library.IO.Directory
{
    public partial class ApiDirectory
    {
        public abstract class GetDirectories : ApiBaseFunction
        {
            public override string Name => nameof(GetDirectories);
            public override string Summary => "Lists subdirectories.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor =>
                new TypeDescriptor(DataTypes.String | DataTypes.Array);
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {DirectoryPathParameter};
        }

        public abstract class Copy : ApiBaseFunction
        {
            public override string Name => nameof(Copy);
            public override string Summary => "Recursively copies a directory.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                DirectoryPathParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "DestinationPath", null, null)
            };
        }

        public abstract class Move : ApiBaseFunction
        {
            public override string Name => nameof(Move);
            public override string Summary => "Moves or renames a directory.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                DirectoryPathParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "DestinationPath", null, null)
            };
        }
    }
}
