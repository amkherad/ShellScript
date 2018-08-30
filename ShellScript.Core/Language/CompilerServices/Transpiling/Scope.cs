using System;
using System.Collections.Generic;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Transpiling
{
    public class Scope
    {
        private static readonly Random Random = new Random();
        
        public Context Context { get; }
        public Scope Parent { get; }

        public bool IsRootScope => Parent == null;

        private readonly HashSet<string> _identifiers;
        private readonly HashSet<ConstantInfo> _constants;
        private readonly HashSet<VariableInfo> _variables;
        private readonly HashSet<FunctionInfo> _functions;


        public Scope(Context context)
        {
            Context = context;

            _identifiers = new HashSet<string>();
            _variables = new HashSet<VariableInfo>();
            _constants = new HashSet<ConstantInfo>();
            _functions = new HashSet<FunctionInfo>();
        }

        public Scope(Context context, Scope parent)
        {
            Context = context;
            Parent = parent;

            _identifiers = new HashSet<string>();
            _variables = new HashSet<VariableInfo>();
            _constants = new HashSet<ConstantInfo>();
            _functions = new HashSet<FunctionInfo>();
        }

        public Scope Fork()
        {
            return new Scope(Context, this);
        }


        public bool IsIdentifierExists(string name)
        {
            var that = this;
            do
            {
                if (that._identifiers.Contains(name))
                {
                    return true;
                }
                
                if (that._variables.Contains(new VariableInfo(name)))
                {
                    return true;
                }
                
                if (that._constants.Contains(new ConstantInfo(name)))
                {
                    return true;
                }
                
                if (that._functions.Contains(new FunctionInfo(name)))
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

        public void ReserveNewConstant(DataTypes dataType, string name, string value)
        {
            _identifiers.Add(name);
            _constants.Add(new ConstantInfo(dataType, name, value));
        }

        public void ReserveNewFunction(DataTypes dataType, string name)
        {
            _identifiers.Add(name);
            //_functions.Add(new FunctionInfo(dataType, name));
        }

        private static string _generateRandomString(int len)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[len];

            for (int i = 0; i < len; i++)
            {
                stringChars[i] = chars[Random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
        
        public string NewVariable(DataTypes dataType)
        {
            var counter = 1;
            var baseName = "v_" + _generateRandomString(12);
            var varName = baseName;
            while (IsIdentifierExists(varName))
            {
                varName = baseName + counter++;
            }

            _identifiers.Add(varName);
            _variables.Add(new VariableInfo(dataType, varName));

            return varName;
        }

        public string NewHelperVariable(DataTypes dataType, string nameHint)
        {
            if (string.IsNullOrWhiteSpace(nameHint) || !StringHelpers.IsValidIdentifierName(nameHint))
            {
                nameHint = "h_" + _generateRandomString(12);
            }
            
            var varName = nameHint;
            while (IsIdentifierExists(varName))
            {
                varName = "h_" + _generateRandomString(12);
            }

            _identifiers.Add(varName);
            _variables.Add(new VariableInfo(dataType, varName));

            return varName;
        }

        public bool TryGetVariableInfo(string variableName, out VariableInfo variableInfo)
        {
            var varInfo = new VariableInfo(variableName);

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
            var constInfo = new ConstantInfo(constantName);

            var that = this;
            do
            {
                if (that._constants.TryGetValue(constInfo, out constantInfo))
                {
                    return true;
                }
                
            } while ((that = that.Parent) != null);

            return false;
        }

        public bool TryGetFunctionInfo(string functionName, out FunctionInfo functionInfo)
        {
            var funcInfo = new FunctionInfo(functionName);

            var that = this;
            do
            {
                if (that._functions.TryGetValue(funcInfo, out functionInfo))
                {
                    return true;
                }
                
            } while ((that = that.Parent) != null);

            return false;
        }
    }
}