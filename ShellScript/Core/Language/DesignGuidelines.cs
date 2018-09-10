using System;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language
{
    public class DesignGuidelines
    {
        public const string ErrorOutputHead = "!-";


        public static string GetDefaultValue(DataTypes dataTypes)
        {
            switch (dataTypes)
            {
                case DataTypes.Void:
                    throw new CompilerException("Void should not be used.", null);
                case DataTypes.Array:
                    return "null";
                case DataTypes.Boolean:
                    return "false";
                case DataTypes.Decimal:
                case DataTypes.Float:
                case DataTypes.Numeric:
                    return "0";
                case DataTypes.String:
                    return "\"\"";
                case DataTypes.Class:
                    return "null";
                case DataTypes.Delegate:
                    return "null";
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataTypes), dataTypes, null);
            }
        }
    }
}