using System;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices
{
    public class VariableInfo : IEquatable<VariableInfo>
    {
        public TypeDescriptor TypeDescriptor { get; }
        
        public string ClassName { get; }
        public string Name { get; }
        public string ReName { get; }

        public string AccessName => ReName ?? Name;
        
        
        public bool IsOriginal { get; set; }
        
        
        public VariableInfo(TypeDescriptor typeDescriptor, string className, string name, string reName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ClassName = className;
            TypeDescriptor = typeDescriptor;
            ReName = reName;
            IsOriginal = true;
        }
        
        public VariableInfo(TypeDescriptor typeDescriptor, string name, string reName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TypeDescriptor = typeDescriptor;
            ReName = reName;
            IsOriginal = true;
        }

        public VariableInfo(string className, string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ClassName = className;
        }
        
        public VariableInfo(string name)
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
            return Equals((VariableInfo) obj);
        }

        public bool Equals(VariableInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }
    }
}