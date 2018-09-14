using System;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices
{
    public class VariableInfo : IEquatable<VariableInfo>
    {
        public DataTypes DataType { get; }
        public string Name { get; }
        public string ReName { get; }

        public string AccessName => ReName ?? Name;
        
        
        public bool IsOriginal { get; set; }
        
        
        public VariableInfo(DataTypes dataType, string name, string reName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DataType = dataType;
            ReName = reName;
            IsOriginal = true;
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