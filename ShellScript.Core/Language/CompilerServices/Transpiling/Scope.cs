using System.Collections.Generic;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Transpiling
{
    public class Scope
    {
        public Context Context { get; }
        public Scope Parent { get; }

        public bool IsRootScope => Parent == null;

        private readonly HashSet<string> _identifiers;
        private readonly HashSet<ConstantInfo> _constants;
        private readonly HashSet<VariableInfo> _variables;


        public Scope(Context context)
        {
            Context = context;

            _identifiers = new HashSet<string>();
            _variables = new HashSet<VariableInfo>();
            _constants = new HashSet<ConstantInfo>();
        }

        public Scope(Context context, Scope parent)
        {
            Context = context;
            Parent = parent;

            _identifiers = new HashSet<string>();
            _variables = new HashSet<VariableInfo>();
            _constants = new HashSet<ConstantInfo>();
        }

        public Scope Fork()
        {
            return new Scope(Context, this);
        }


        public bool IsVariableExists(string name)
        {
            var that = this;
            do
            {
                if (that._identifiers.Contains(name))
                {
                    return true;
                }
                
                if (that._variables.Contains(new VariableInfo(DataTypes.Void, name)))
                {
                    return true;
                }
                
                if (that._constants.Contains(new ConstantInfo(DataTypes.Void, name)))
                {
                    return true;
                }
                
            } while ((that = that.Parent) != null);

            return false;
        }

        public void ReserveNewVariable(DataTypes dataType, string name)
        {
            _identifiers.Add(name);
            _variables.Add(new VariableInfo(dataType, name));
        }

        public void ReserveNewConstant(DataTypes dataType, string name)
        {
            _identifiers.Add(name);
            _constants.Add(new ConstantInfo(dataType, name));
        }

        public void ReserveNewFunction(DataTypes dataType, string name)
        {
            _identifiers.Add(name);
            //_functions.Add(new FunctionInfo(dataType, name));
        }

        public string NewVariable(DataTypes dataType, string nameHint)
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

        public string NewVariableIfNotExists(DataTypes dataType, string nameHint)
        {
            if (IsVariableExists(nameHint))
            {
                return nameHint;
            }

            _identifiers.Add(nameHint);
            _variables.Add(new VariableInfo(dataType, nameHint));

            return nameHint;
        }

        public bool TryGetVariableInfo(string variableName, out VariableInfo variableInfo)
        {
            var varInfo = new VariableInfo(DataTypes.String, variableName);

            var that = this;
            do
            {
                if (that._variables.TryGetValue(varInfo, out variableInfo))
                {
                    return true;
                }
                
            } while ((that = that.Parent) != null);

            return false;
        }

        public bool TryGetConstantInfo(string constantName, out ConstantInfo constantInfo)
        {
            var varInfo = new ConstantInfo(DataTypes.String, constantName);

            var that = this;
            do
            {
                if (that._constants.TryGetValue(varInfo, out constantInfo))
                {
                    return true;
                }
                
            } while ((that = that.Parent) != null);

            return false;
        }
    }
}