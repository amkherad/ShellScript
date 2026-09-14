using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler
{
    public static class ObjectModelHelpers
    {
        public const string ThisKeyword = "this";

        public static TypeDescriptor UserClass(string className)
        {
            return new TypeDescriptor(DataTypes.Class, new TypeDescriptor.LookupInfo(null, className));
        }

        public static bool IsUserClassType(TypeDescriptor typeDescriptor)
        {
            return typeDescriptor.DataType == DataTypes.Class &&
                   typeDescriptor.Lookup != null &&
                   !string.IsNullOrEmpty(typeDescriptor.Lookup.Value.Name);
        }

        public static string GetUserClassName(TypeDescriptor typeDescriptor)
        {
            if (!IsUserClassType(typeDescriptor))
            {
                return null;
            }

            return typeDescriptor.Lookup.Value.Name;
        }

        public static bool TryResolveInstanceVariable(Scope scope, string receiverName, out VariableInfo variableInfo)
        {
            if (receiverName == ThisKeyword)
            {
                return scope.TryGetVariableInfo(ThisKeyword, out variableInfo);
            }

            return scope.TryGetVariableInfo(receiverName, out variableInfo);
        }

        public static bool TryResolveInstanceFieldAccess(Scope scope, VariableAccessStatement access,
            out VariableInfo instanceInfo, out TypeDescriptor fieldType)
        {
            instanceInfo = null;
            fieldType = default;

            if (string.IsNullOrEmpty(access.ClassName))
            {
                return false;
            }

            if (!TryResolveInstanceVariable(scope, access.ClassName, out instanceInfo))
            {
                return false;
            }

            if (!IsUserClassType(instanceInfo.TypeDescriptor))
            {
                return false;
            }

            var className = GetUserClassName(instanceInfo.TypeDescriptor);
            if (!scope.TryGetUserClass(className, out var classInfo))
            {
                return false;
            }

            if (!classInfo.TryGetFieldType(access.VariableName, out fieldType))
            {
                return false;
            }

            return true;
        }
    }
}
