using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Transpiling
{
    public class Context
    {
        public Scope GeneralScope { get; }

        public IPlatform Platform { get; }
        public IPlatformStatementTranspiler[] Transpilers { get; }
        public ISdk Sdk { get; }

        private Dictionary<Type, IPlatformStatementTranspiler> _typeTranspilers;
        
        /// <summary>
        /// Meta is a separate file that will be prepended to output file.
        /// </summary>
        public TextWriter MetaWriter { get; }

        public TextWriter OutputWriter { get; }


        public Context(TextWriter metaWriter, TextWriter outputWriter, IPlatform platform)
        {
            GeneralScope = new Scope(this);

            Platform = platform;
            Transpilers = platform.Transpilers;
            Sdk = platform.Sdk;
            MetaWriter = metaWriter;
            OutputWriter = outputWriter;

            _typeTranspilers = Transpilers.ToDictionary(key => key.StatementType);
        }


        public IPlatformStatementTranspiler GetTranspilerForStatement(IStatement statement)
        {
            if (_typeTranspilers.TryGetValue(statement.GetType(), out var value))
            {
                return value;
            }

            throw new InvalidOperationException();
        }
        public IPlatformEvaluationStatementTranspiler GetEvaluationTranspilerForStatement(EvaluationStatement statement)
        {
            if (_typeTranspilers.TryGetValue(statement.GetType(), out var value))
            {
                return value as IPlatformEvaluationStatementTranspiler;
            }

            throw new InvalidOperationException();
        }
        

        public TTranspiler GetTranspiler<TTranspiler, TStatement>()
            where TTranspiler : class, IPlatformStatementTranspiler
        {
            if (_typeTranspilers.TryGetValue(typeof(TStatement), out var value))
            {
                return value as TTranspiler;
            }
            
            throw new InvalidOperationException();
        }
        

        public bool IsSdkFunctionExists(string objectName, string functionName)
            => IsSdkFunctionExists(objectName, functionName, -1);

        public bool IsSdkFunctionExists(string objectName, string functionName, int numberOfParameters)
        {
            if (!Sdk.TryGetClass(objectName, out var sdkClass))
            {
                return false;
            }

            if (!sdkClass.TryGetFunction(functionName, out var sdkFunction))
            {
                return false;
            }

            if (numberOfParameters != -1)
            {
                return sdkFunction.Parameters.Length == numberOfParameters;
            }

            return true;
        }


        public bool IsSdkFunctionExists(string functionName)
            => IsSdkFunctionExists(functionName, -1);


        public bool IsSdkFunctionExists(string functionName, int numberOfParameters)
        {
            if (!Sdk.TryGetGeneralFunction(functionName, out var sdkFunction))
            {
                return false;
            }

            if (numberOfParameters != -1)
            {
                return sdkFunction.Parameters.Length == numberOfParameters;
            }

            return true;
        }
    }
}