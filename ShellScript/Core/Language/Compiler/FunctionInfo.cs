using System;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler
{
    public class FunctionInfo : IEquatable<FunctionInfo>, ILanguageObjectInfo
    {
        public TypeDescriptor TypeDescriptor { get; }
        public virtual string ClassName { get; }
        public virtual string Name { get; }
        public virtual string ReName { get; }

        public virtual string AccessName
        {
            get
            {
                if (ClassName != null)
                {
                    return $"{ClassName}_{ReName ?? Name}";
                }

                return ReName ?? Name;
            }
        }

        public virtual string Fqn
        {
            get
            {
                if (ClassName != null)
                {
                    return $"{ClassName}_{ReName ?? Name}";
                }

                return ReName ?? Name;
            }
        }

        public bool IsParams { get; }
        public bool ByPassParameterValidation { get; }
        public FunctionParameterDefinitionStatement[] Parameters { get; }

        public IStatement InlinedStatement { get; }


        public FunctionInfo(TypeDescriptor typeDescriptor, string name, string reName, string className, bool isParams,
            FunctionParameterDefinitionStatement[] parameters,
            IStatement inlinedStatement, bool byPassParameterValidation = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ClassName = className;
            ReName = reName;
            IsParams = isParams;
            Parameters = parameters;
            InlinedStatement = inlinedStatement;
            ByPassParameterValidation = byPassParameterValidation;
            TypeDescriptor = typeDescriptor;
        }

        public FunctionInfo(string className, string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ClassName = className;
        }

        public virtual bool Equals(FunctionInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && string.Equals(ClassName, other.ClassName);
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
                       (ClassName != null ? ClassName.GetHashCode() : 0);
            }
        }
    }
}