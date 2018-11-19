namespace ShellScript.Core.Language.Library
{
    public readonly struct TypeDescriptor
    {
        public readonly struct LookupInfo
        {
            public string ClassName { get; }
            public string Name { get; }
            
            public LookupInfo(string className, string name)
            {
                ClassName = className;
                Name = name;
            }

            public override string ToString()
            {
                return ClassName == null
                    ? Name
                    : $"{ClassName}.{Name}";
            }
        }
        
        public DataTypes DataType { get; }
        public LookupInfo? Lookup { get; }

        public TypeDescriptor(DataTypes dataType, LookupInfo? lookup)
        {
            DataType = dataType;
            Lookup = lookup;
        }

        public TypeDescriptor(DataTypes dataType)
        {
            DataType = dataType;
            Lookup = null;
        }


        public override string ToString()
        {
            if (DataType == DataTypes.Lookup)
            {
                return Lookup?.ToString();
            }
            
            return DataType.ToString();
        }

        public bool Equals(TypeDescriptor other)
        {
            return DataType == other.DataType && string.Equals(Lookup.ToString(), other.Lookup.ToString());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TypeDescriptor other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) DataType * 397) ^ (Lookup != null ? Lookup.GetHashCode() : 0);
            }
        }

        public static bool operator ==(TypeDescriptor a, TypeDescriptor b)
        {
            if (a.DataType == DataTypes.Lookup)
            {
                return b.DataType == DataTypes.Lookup &&
                       a.Lookup.ToString() == b.Lookup.ToString();
            }

            return a.DataType == b.DataType;
        }

        public static bool operator !=(TypeDescriptor a, TypeDescriptor b)
        {
            return !(a == b);
        }

        public static implicit operator TypeDescriptor(DataTypes dataType)
        {
            return new TypeDescriptor(dataType);
        }

        public static implicit operator DataTypes(TypeDescriptor typeDescriptor)
        {
            return typeDescriptor.DataType;
        }


        public static TypeDescriptor Any => new TypeDescriptor(DataTypes.Any);
        public static TypeDescriptor Void => new TypeDescriptor(DataTypes.Void);
        public static TypeDescriptor Boolean => new TypeDescriptor(DataTypes.Boolean);
        public static TypeDescriptor Integer => new TypeDescriptor(DataTypes.Integer);
        public static TypeDescriptor Float => new TypeDescriptor(DataTypes.Float);
        public static TypeDescriptor Numeric => new TypeDescriptor(DataTypes.Numeric);
        public static TypeDescriptor String => new TypeDescriptor(DataTypes.String);
        //public static TypeDescriptor Class => new TypeDescriptor(DataTypes.Class);
        //public static TypeDescriptor Delegate => new TypeDescriptor(DataTypes.Delegate);
    }
}