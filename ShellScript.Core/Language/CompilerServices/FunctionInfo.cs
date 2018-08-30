using System;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices
{
    public class FunctionInfo : IEquatable<FunctionInfo>
    {
        public DataTypes DataType { get; }
        public string Name { get; }

        public bool IsParams { get; }
        public VariableInfo[] Parameters { get; }
        public string ObjectName { get; }

        public IStatement InlinedStatement { get; }

        
        public FunctionInfo(DataTypes dataType, string name, bool isParams, VariableInfo[] parameters,
            string objectName, IStatement inlinedStatement)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsParams = isParams;
            Parameters = parameters;
            ObjectName = objectName;
            InlinedStatement = inlinedStatement;
            DataType = dataType;
        }

        public FunctionInfo(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FunctionInfo) obj);
        }

        public bool Equals(FunctionInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }


        public static IStatement UnWrapInlinedStatement(Context context, Scope scope, FunctionInfo functionInfo)
        {
            var inlined = functionInfo.InlinedStatement;
            var result = inlined;
            while(inlined != null && inlined is FunctionCallStatement funcCallStt)
            {
                if (!scope.TryGetFunctionInfo(funcCallStt.FunctionName, out functionInfo))
                {
                    return result;
                }
                
                inlined = functionInfo.InlinedStatement;
            }

            return result;
        }
    }
}