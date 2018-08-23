using System.Collections.Generic;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Transpiling
{
    public class Scope
    {
        public Context Context { get; }
        public Scope Parent { get; }


        public HashSet<string> _identifiers;
        public HashSet<VariableInfo> _variables;


        public Scope(Context context)
        {
            Context = context;

            _identifiers = new HashSet<string>();
            _variables = new HashSet<VariableInfo>();
        }

        public Scope(Context context, Scope parent)
        {
            Context = context;
            Parent = parent;

            _identifiers = new HashSet<string>();
            _variables = new HashSet<VariableInfo>();
        }

        public Scope Clone()
        {
            return new Scope(Context, this);
        }

        public bool IsVariableExists(string name)
        {
            if (_variables.Contains(new VariableInfo(DataTypes.Variant, name)))
            {
                return true;
            }
            
            if (_identifiers.Contains(name))
            {
                return true;
            }

            return Parent?.IsVariableExists(name) ?? false;
        }

        public string ReserveNewVariable(DataTypes dataType, string nameHint)
        {
            var counter = 1;
            var varName = nameHint;
            while (IsVariableExists(varName))
            {
                varName = nameHint + counter++;
            }

            _identifiers.Add(varName);
            _variables.Add(new VariableInfo(dataType, varName));

            return varName;
        }

        public string ReserveNewVariableIfNotExists(DataTypes dataType, string nameHint)
        {
            if (IsVariableExists(nameHint))
            {
                return nameHint;
            }

            _identifiers.Add(nameHint);
            _variables.Add(new VariableInfo(dataType, nameHint));

            return nameHint;
        }
    }
}