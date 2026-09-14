using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.String
{
    public partial class ApiString
    {
        public abstract class ToLower : ApiBaseFunction
        {
            public override string Name => nameof(ToLower);
            public override string Summary => "Returns the string in lowercase.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {StringParameter};
        }

        public abstract class ToUpper : ApiBaseFunction
        {
            public override string Name => nameof(ToUpper);
            public override string Summary => "Returns the string in uppercase.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {StringParameter};
        }

        public abstract class Trim : ApiBaseFunction
        {
            public override string Name => nameof(Trim);
            public override string Summary => "Removes leading and trailing whitespace.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {StringParameter};
        }

        public abstract class GetBefore : ApiBaseFunction
        {
            public override string Name => nameof(GetBefore);
            public override string Summary => "Returns the substring before the first occurrence of a delimiter.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Delimiter", null, null)
            };
        }

        public abstract class GetAfter : ApiBaseFunction
        {
            public override string Name => nameof(GetAfter);
            public override string Summary => "Returns the substring after the first occurrence of a delimiter.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Delimiter", null, null)
            };
        }

        public abstract class IndexOf : ApiBaseFunction
        {
            public override string Name => nameof(IndexOf);
            public override string Summary => "Returns the index of a substring, or -1 when not found.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Value", null, null)
            };
        }

        public abstract class Substring : ApiBaseFunction
        {
            public override string Name => nameof(Substring);
            public override string Summary => "Returns a substring starting at an index.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "StartIndex", null, null)
            };
        }

        public abstract class Equals : ApiBaseFunction
        {
            public override string Name => nameof(Equals);
            public override string Summary => "Compares two strings for equality.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Other", null, null)
            };
        }

        public abstract class Replace : ApiBaseFunction
        {
            public override string Name => nameof(Replace);
            public override string Summary => "Replaces occurrences of a substring.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "OldValue", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "NewValue", null, null)
            };
        }
    }
}
