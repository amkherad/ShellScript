using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Library.Core.String
{
    public partial class ApiString
    {
        public abstract class Join : ApiBaseFunction
        {
            public override string Name => nameof(Join);
            public override string Summary => "Joins string array elements with a separator.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(
                    new TypeDescriptor(DataTypes.String | DataTypes.Array), "Values", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Separator", null, null)
            };
        }

        public abstract class Split : ApiBaseFunction
        {
            public override string Name => nameof(Split);
            public override string Summary => "Splits a string by delimiter into a string array.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor =>
                new TypeDescriptor(DataTypes.String | DataTypes.Array);
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Delimiter", null, null)
            };
        }

        public abstract class Repeat : ApiBaseFunction
        {
            public override string Name => nameof(Repeat);
            public override string Summary => "Repeats a string a number of times.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "Count", null, null)
            };
        }

        public abstract class TrimStart : ApiBaseFunction
        {
            public override string Name => nameof(TrimStart);
            public override string Summary => "Removes leading whitespace.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {StringParameter};
        }

        public abstract class TrimEnd : ApiBaseFunction
        {
            public override string Name => nameof(TrimEnd);
            public override string Summary => "Removes trailing whitespace.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {StringParameter};
        }

        public abstract class PadLeft : ApiBaseFunction
        {
            public override string Name => nameof(PadLeft);
            public override string Summary => "Left-pads a string to a minimum width (space by default).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "TotalWidth", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "PadCharacter",
                    new ConstantValueStatement(TypeDescriptor.String, " ", null), null)
            };
        }

        public abstract class PadRight : ApiBaseFunction
        {
            public override string Name => nameof(PadRight);
            public override string Summary => "Right-pads a string to a minimum width (space by default).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "TotalWidth", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "PadCharacter",
                    new ConstantValueStatement(TypeDescriptor.String, " ", null), null)
            };
        }

        public abstract class LastIndexOf : ApiBaseFunction
        {
            public override string Name => nameof(LastIndexOf);
            public override string Summary => "Returns the last index of a substring, or -1 when not found.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Value", null, null)
            };
        }

        public abstract class Compare : ApiBaseFunction
        {
            public override string Name => nameof(Compare);
            public override string Summary =>
                "Compares two strings lexicographically; returns -1, 0, or 1.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Other", null, null)
            };
        }

        public abstract class CompareIgnoreCase : ApiBaseFunction
        {
            public override string Name => nameof(CompareIgnoreCase);
            public override string Summary =>
                "Compares two strings case-insensitively; returns -1, 0, or 1.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Other", null, null)
            };
        }

        public abstract class ContainsIgnoreCase : ApiBaseFunction
        {
            public override string Name => nameof(ContainsIgnoreCase);
            public override string Summary => "Checks whether a string contains a value (case-insensitive).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Search", null, null)
            };
        }
    }
}
