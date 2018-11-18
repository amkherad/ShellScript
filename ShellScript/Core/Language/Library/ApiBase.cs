using System;
using System.Collections.Generic;
using System.Linq;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Transpiling;

namespace ShellScript.Core.Language.Library
{
    public abstract class ApiBase : IApi
    {
        public abstract IApiVariable[] Variables { get; }
        public abstract IApiFunc[] Functions { get; }
        public abstract IApiClass[] Classes { get; }
        public abstract IDictionary<string, IThirdPartyUtility> Utilities { get; }

        public abstract string Name { get; }

        private readonly string _myNamespace;
        private readonly int _myNamespaceLength;
        
        private Dictionary<Type, IApiClass> _flattenClasses;
        private Dictionary<Type, IApiFunc> _flattenFunctions;
        private Dictionary<Type, IApiVariable> _flattenVariables;

        protected ApiBase()
        {
            _myNamespace = typeof(ApiBase).Namespace;
            _myNamespaceLength = _myNamespace.Length;
        }
        
        
        public virtual void InitializeContext(Context context)
        {
            _flattenClasses = new Dictionary<Type, IApiClass>();
            _flattenFunctions = new Dictionary<Type, IApiFunc>();
            _flattenVariables = new Dictionary<Type, IApiVariable>();

            var scope = context.GeneralScope;
            
            foreach (var cls in Classes)
            {
                {
                    var type = cls.GetType();
                    _flattenClasses.Add(type, cls);

                    var baseType = _getStdLibraryBaseType(type);
                    if (baseType != null)
                    {
                        _flattenClasses.Add(baseType, cls);
                    }
                }

                foreach (var function in cls.Functions)
                {
                    var funcInfo = new ApiFunctionInfo(function.TypeDescriptor, function.Name, null, cls.Name, function,
                        false, function.Parameters);

                    scope.ReserveNewFunction(funcInfo);
                    
                    var type = function.GetType();
                    _flattenFunctions.Add(type, function);
                    
                    var baseType = _getStdLibraryBaseType(type);
                    if (baseType != null)
                    {
                        _flattenFunctions.Add(baseType, function);
                    }
                }
            }
        }

        private Type _getStdLibraryBaseType(Type type)
        {
            while ((type = type.BaseType) != null)
            {
                var ns = type.Namespace;
                if (ns.StartsWith(_myNamespace) && ns.Length > _myNamespaceLength)
                {
                    return type;
                }
            }

            return null;
        }

        public bool TryGetClass(string className, out IApiClass result)
        {
            result = Classes.FirstOrDefault(x => x.Name == className);
            return result != null;
        }

        public bool TryGetClass(Type apiClassType, out IApiClass result)
        {
            if (!_flattenClasses.TryGetValue(apiClassType, out var value))
            {
                result = null;
                return false;
            }

            result = value;
            return true;
        }

        public bool TryGetFunction(string functionName, out IApiFunc result)
        {
            result = Functions.FirstOrDefault(x => x.Name == functionName);
            return result != null;
        }

        public bool TryGetFunction(Type apiFunctionType, out IApiFunc result)
        {
            if (!_flattenFunctions.TryGetValue(apiFunctionType, out var value))
            {
                result = null;
                return false;
            }

            result = value;
            return true;
        }

        public bool TryGetVariable(string variableName, out IApiVariable result)
        {
            result = Variables.FirstOrDefault(x => x.Name == variableName);
            return result != null;
        }

        public bool TryGetVariable(Type apiVariableType, out IApiVariable result)
        {
            if (!_flattenVariables.TryGetValue(apiVariableType, out var value))
            {
                result = null;
                return false;
            }

            result = value;
            return true;
        }
    }
}