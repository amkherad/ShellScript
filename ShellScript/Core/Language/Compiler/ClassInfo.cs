using System.Collections.Generic;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler
{
    public sealed class ClassInfo
    {
        public string Name { get; }
        public IReadOnlyDictionary<string, TypeDescriptor> Fields { get; }
        public FunctionStatement Constructor { get; }

        public ClassInfo(string name, Dictionary<string, TypeDescriptor> fields, FunctionStatement constructor)
        {
            Name = name;
            Fields = fields;
            Constructor = constructor;
        }

        public bool TryGetFieldType(string fieldName, out TypeDescriptor typeDescriptor)
        {
            return Fields.TryGetValue(fieldName, out typeDescriptor);
        }
    }
}
