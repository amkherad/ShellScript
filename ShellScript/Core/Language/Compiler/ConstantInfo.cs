using System;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler
{
    public class ConstantInfo : IEquatable<ConstantInfo>, ILanguageObjectInfo
    {
        public TypeDescriptor TypeDescriptor { get; }
        public string ClassName { get; }

        public string Name { get; }

        public string ReName { get; }
        //public string ReName { get; }

        public string Value { get; }

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

        public ConstantInfo(TypeDescriptor typeDescriptor, string name, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value;
            TypeDescriptor = typeDescriptor;
        }

        public ConstantInfo(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConstantInfo) obj);
        }

        public bool Equals(ConstantInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }
    }
}