using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Library;

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
        private readonly HashSet<FunctionInfo> _functionPrototypes;

        private readonly Dictionary<string, string> _config;
        
        public interface IScopedConfig
        {
            string ExplicitEchoStream { get; set; }
        }

        public Scope(Context context)
        {
            Context = context;

            _identifiers = new HashSet<string>();
            _variables = new HashSet<VariableInfo>();
            _constants = new HashSet<ConstantInfo>();
            _functions = new HashSet<FunctionInfo>();
            _functionPrototypes = new HashSet<FunctionInfo>();
            
            _config = new Dictionary<string, string>();
        }

        public Scope(Context context, Scope parent)
        {
            Context = context;
            Parent = parent;

            _identifiers = new HashSet<string>();
            _variables = new HashSet<VariableInfo>();
            _constants = new HashSet<ConstantInfo>();
            _functions = new HashSet<FunctionInfo>();
            _functionPrototypes = new HashSet<FunctionInfo>();
            
            _config = new Dictionary<string, string>();
        }

        public Scope BeginNewScope()
        {
            return new Scope(Context, this);
        }

        public T GetConfig<T>(Expression<Func<IScopedConfig, T>> config, T defaultValue)
        {
            var propertyInfo = (config.Body as MemberExpression)?.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a invalid member, not a property.",
                config.ToString()));
            }
            
            if (typeof(T) != propertyInfo.PropertyType &&
                !typeof(T).IsSubclassOf(propertyInfo.PropertyType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    config.ToString(),
                    typeof(T)));

            var propName = propertyInfo.Name;
            var that = this;
            do
            {
                if (_config.TryGetValue(propName, out var value))
                {
                    return (T) Convert.ChangeType(value, typeof(T));
                }
                
            } while ((that = that.Parent) != null);

            return defaultValue;
        }
        
        public void SetConfig<T>(Expression<Func<IScopedConfig, T>> config, T value)
        {
            var propertyInfo = (config.Body as MemberExpression)?.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a invalid member, not a property.",
                config.ToString()));
            }
            
            if (typeof(T) != propertyInfo.PropertyType &&
                !typeof(T).IsSubclassOf(propertyInfo.PropertyType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    config.ToString(),
                    typeof(T)));

            var propName = propertyInfo.Name;
            if (_config.ContainsKey(propName))
            {
                _config[propName] = Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            else
            {
                _config.Add(propName, Convert.ToString(value, CultureInfo.InvariantCulture));
            }
        }

        public bool IsIdentifierExists(FunctionCallStatement functionCallStatement)
        {
            var that = this;
            do
            {
                if (that._identifiers.Contains(functionCallStatement.Fqn))
                {
                    return true;
                }
                
            } while ((that = that.Parent) != null);

            return false;
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
                
            } while ((that = that.Parent) != null);

            return false;
        }

        public void ReserveNewVariable(TypeDescriptor typeDescriptor, string name)
        {
            _identifiers.Add(name);
            _variables.Add(new VariableInfo(typeDescriptor, name, null));
        }

        public void ReserveNewParameter(TypeDescriptor typeDescriptor, string name, string rename)
        {
            _identifiers.Add(name);
            _variables.Add(new VariableInfo(typeDescriptor, name, rename));
        }

        public void ReserveNewConstant(TypeDescriptor typeDescriptor, string name, string value)
        {
            _identifiers.Add(name);
            _constants.Add(new ConstantInfo(typeDescriptor, name, value));
        }

        public void ReserveNewFunction(FunctionInfo function)
        {
            _identifiers.Add(function.Fqn);
            _functions.Add(function);
        }

        public void ReserveNewPrototype(FunctionInfo function)
        {
            _identifiers.Add(function.Fqn);
            _functionPrototypes.Add(function);
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
//        
//        public string NewVariable(DataTypes dataType)
//        {
//            var counter = 1;
//            var baseName = "v_" + _generateRandomString(12);
//            var varName = baseName;
//            while (IsIdentifierExists(varName))
//            {
//                varName = baseName + counter++;
//            }
//
//            _identifiers.Add(varName);
//            _variables.Add(new VariableInfo(dataType, varName, null));
//
//            return varName;
//        }

        public string NewHelperVariable(TypeDescriptor typeDescriptor, string nameHint)
        {
            if (string.IsNullOrWhiteSpace(nameHint) || !StringHelpers.IsValidIdentifierName(nameHint))
            {
                nameHint = _generateRandomString(12);
            }
            
            var varName = "h_" + nameHint;

            if (Context.Flags.PreferRandomHelperVariableNames)
            {
                while (IsIdentifierExists(varName))
                {
                    varName = "h_" + _generateRandomString(12);
                }
            }
            else
            {
                var index = 0;
                var bufName = varName;
                while (IsIdentifierExists(bufName))
                {
                    bufName = bufName + ++index;
                }

                varName = bufName;
            }

            _identifiers.Add(varName);
            _variables.Add(new VariableInfo(typeDescriptor, varName, null));

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

        public bool TryGetVariableInfo(VariableAccessStatement variableAccessStatement, out VariableInfo variableInfo)
        {
            var varInfo = new VariableInfo(variableAccessStatement.ClassName, variableAccessStatement.VariableName);

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
//
//        public bool TryGetConstantInfo(string constantName, out ConstantInfo constantInfo)
//        {
//            var constInfo = new ConstantInfo(constantName);
//
//            var that = this;
//            do
//            {
//                if (that._constants.TryGetValue(constInfo, out constantInfo))
//                {
//                    return true;
//                }
//                
//            } while ((that = that.Parent) != null);
//
//            return false;
//        }

        public bool TryGetConstantInfo(VariableAccessStatement variableAccessStatement, out ConstantInfo constantInfo)
        {
            var constInfo = new ConstantInfo(variableAccessStatement.VariableName);

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

        public bool TryGetPrototypeInfo(VariableAccessStatement variableAccessStatement, out FunctionInfo functionInfo)
        {
            var funcInfo = new FunctionInfo(variableAccessStatement.ClassName, variableAccessStatement.VariableName);

            var that = this;
            do
            {
                if (that._functionPrototypes.TryGetValue(funcInfo, out functionInfo))
                {
                    return true;
                }
                
            } while ((that = that.Parent) != null);

            return false;
        }

        public bool TryGetPrototypeInfo(string className, string functionName, out FunctionInfo functionInfo)
        {
            var funcInfo = new FunctionInfo(className, functionName);

            var that = this;
            do
            {
                if (that._functionPrototypes.TryGetValue(funcInfo, out functionInfo))
                {
                    return true;
                }
                
            } while ((that = that.Parent) != null);

            return false;
        }

        public bool TryGetFunctionInfo(FunctionInfo functionInfo, out FunctionInfo actualFunctionInfo)
        {
            var that = this;
            do
            {
                if (that._functions.TryGetValue(functionInfo, out actualFunctionInfo))
                {
                    return true;
                }
                
            } while ((that = that.Parent) != null);

            return false;
        }

        public bool TryGetFunctionInfo(FunctionCallStatement functionCallStatement, out FunctionInfo functionInfo)
        {
            var funcInfo = new FunctionInfo(functionCallStatement.ObjectName, functionCallStatement.FunctionName);

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

        public bool TryGetFunctionInfo(VariableAccessStatement variableAccessStatement, out FunctionInfo functionInfo)
        {
            var funcInfo = new FunctionInfo(variableAccessStatement.ClassName, variableAccessStatement.VariableName);

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

        public bool TryGetFunctionInfo(string className, string functionName, out FunctionInfo functionInfo)
        {
            var funcInfo = new FunctionInfo(className, functionName);

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
        

        public int Level
        {
            get
            {
                var level = 0;
                var that = this;
                do
                {
                    level++;
                } while ((that = that.Parent) != null);

                return level;
            }
        }
    }
}