using System;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices
{
    public class FunctionInfo : IEquatable<FunctionInfo>
    {
        public DataTypes DataType { get; }
        public virtual string ObjectName { get; }
        public virtual string Name { get; }
        public virtual string ReName { get; }

        public virtual string AccessName
        {
            get
            {
                if (ObjectName != null)
                {
                    return $"{ObjectName}_{ReName ?? Name}";
                }

                return ReName ?? Name;
            }
        }

        public virtual string Fqn
        {
            get
            {
                if (ObjectName != null)
                {
                    return $"{ObjectName}_{ReName ?? Name}";
                }

                return ReName ?? Name;
            }
        }

        public bool IsParams { get; }
        public bool ByPassParameterValidation { get; }
        public FunctionParameterDefinitionStatement[] Parameters { get; }

        public IStatement InlinedStatement { get; }


        public FunctionInfo(DataTypes dataType, string name, string reName, string objectName, bool isParams,
            FunctionParameterDefinitionStatement[] parameters,
            IStatement inlinedStatement, bool byPassParameterValidation = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ObjectName = objectName;
            ReName = reName;
            IsParams = isParams;
            Parameters = parameters;
            InlinedStatement = inlinedStatement;
            ByPassParameterValidation = byPassParameterValidation;
            DataType = dataType;
        }

        public FunctionInfo(string objectName, string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ObjectName = objectName;
        }

        public virtual bool Equals(FunctionInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && string.Equals(ObjectName, other.ObjectName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FunctionInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^
                       (ObjectName != null ? ObjectName.GetHashCode() : 0);
            }
        }
    }
}