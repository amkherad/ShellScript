using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Core.Language.Compiler.Transpiling
{
    public class Context
    {
        public Scope GeneralScope { get; }

        public IPlatform Platform { get; }
        public IPlatformStatementTranspiler[] Transpilers { get; }
        public IApi Api { get; }

        public CultureInfo CultureInfo { get; }
        public StringComparer StringComparer { get; }

        public CompilerFlags Flags { get; }
        
        public Compiler Compiler { get; }
        
        public TextWriter ErrorWriter { get; }
        public TextWriter WarningWriter { get; }
        public TextWriter LogWriter { get; }
        public HashSet<string> Includes { get; set; }


        private int _lastFunctionCallReferenceCount = 0;
        
        
        private readonly Dictionary<Type, IPlatformStatementTranspiler> _typeTranspilers;

        private readonly Dictionary<Type, Type> _fallbackType = new Dictionary<Type, Type>
        {
            {typeof(ConstantValueStatement), typeof(EvaluationStatement)},
            {typeof(ArithmeticEvaluationStatement), typeof(EvaluationStatement)},
            //{typeof(AssignmentStatement), typeof(EvaluationStatement)},
            {typeof(BitwiseEvaluationStatement), typeof(EvaluationStatement)},
            //{typeof(DecrementStatement), typeof(EvaluationStatement)},
            //{typeof(DoWhileStatement), typeof(ConditionalBlockStatement)},
            //{typeof(ForStatement), typeof(ConditionalBlockStatement)},
            {typeof(FunctionCallStatement), typeof(EvaluationStatement)},
            //{typeof(FunctionParameterDefinitionStatement), typeof(DefinitionStatement)},
            //{typeof(IncrementStatement), typeof(EvaluationStatement)},
            {typeof(LogicalEvaluationStatement), typeof(EvaluationStatement)},
            {typeof(NopStatement), typeof(EvaluationStatement)},
            {typeof(VariableAccessStatement), typeof(EvaluationStatement)},
            
            {typeof(TypeCastStatement), typeof(EvaluationStatement)},
            //{typeof(VariableDefinitionStatement), typeof(DefinitionStatement)},
            //{typeof(WhileStatement), typeof(ConditionalBlockStatement)},
            
            {typeof(IndexerAccessStatement), typeof(EvaluationStatement)},
            {typeof(ArrayStatement), typeof(EvaluationStatement)},
        };


        public Context(Compiler compiler, IPlatform platform, CompilerFlags flags,
            TextWriter errorWriter, TextWriter warningWriter, TextWriter logWriter)
        {
            GeneralScope = new Scope(this);

            Compiler = compiler;
            
            Platform = platform;
            Transpilers = platform.Transpilers;
            Api = platform.Api;
            Flags = flags;
            ErrorWriter = errorWriter;
            WarningWriter = warningWriter;
            LogWriter = logWriter;

            _typeTranspilers = Transpilers.ToDictionary(key => key.StatementType);
            
            CultureInfo = CultureInfo.CurrentCulture;
            StringComparer = StringComparer.CurrentCulture;

            Includes = new HashSet<string>();
            
            InitializeContext();
        }

        public void InitializeContext()
        {
            foreach (var (dataType, name, value) in Platform.CompilerConstants)
            {
                GeneralScope.ReserveNewConstant(dataType, name, value);
            }
        }
        
        public IPlatformMetaInfoTranspiler GetMetaInfoTranspiler()
        {
            return Platform.MetaInfoWriter;
        }

        
        public IPlatformStatementTranspiler GetTranspilerForStatement(IStatement statement)
        {
            var sttType = statement.GetType();
            if (_typeTranspilers.TryGetValue(sttType, out var value))
            {
                return value;
            }

            if (_fallbackType.TryGetValue(sttType, out sttType))
            {
                if (_typeTranspilers.TryGetValue(sttType, out value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException();
        }

        
        public IPlatformEvaluationStatementTranspiler GetEvaluationTranspilerForStatement(EvaluationStatement statement)
        {
            var sttType = statement.GetType();
            if (_typeTranspilers.TryGetValue(statement.GetType(), out var value))
            {
                return value as IPlatformEvaluationStatementTranspiler;
            }

            if (_fallbackType.TryGetValue(sttType, out sttType))
            {
                if (_typeTranspilers.TryGetValue(sttType, out value))
                {
                    return value as IPlatformEvaluationStatementTranspiler;
                }
            }

            throw new InvalidOperationException();
        }


        public TTranspiler GetTranspiler<TTranspiler, TStatement>()
            where TTranspiler : IPlatformStatementTranspiler
        {
            var sttType = typeof(TStatement);
            if (_typeTranspilers.TryGetValue(sttType, out var value))
            {
                return (TTranspiler) value;
            }

            if (_fallbackType.TryGetValue(sttType, out sttType))
            {
                if (_typeTranspilers.TryGetValue(sttType, out value))
                {
                    return (TTranspiler) value;
                }
            }

            throw new InvalidOperationException();
        }

        
        public TTranspiler GetTranspiler<TTranspiler>()
            where TTranspiler : IPlatformStatementTranspiler
        {
            return Transpilers.OfType<TTranspiler>().FirstOrDefault();
        }
        
        public string GetLastFunctionCallStorageVariable(TextWriter metaTextWriter)
        {
            const string VariableName = "LastFunctionCall";
            
            if (Interlocked.Increment(ref _lastFunctionCallReferenceCount) == 1)
            {
                BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(this, GeneralScope, metaTextWriter,
                    VariableName, "0");
            }

            return VariableName;
        }
    }
}