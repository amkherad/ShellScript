using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using ShellScript.Core.Language.Compiler;
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

        /// <summary>Directories of sources currently being compiled (top = innermost). Used to resolve relative includes.</summary>
        public Stack<string> IncludeDirectoryStack { get; } = new Stack<string>();

        /// <summary>Source files on the include stack (detect circular includes).</summary>
        public Stack<string> ActiveIncludeSources { get; } = new Stack<string>();

        /// <summary>Sources whose top-level statements were already merged (include-once per compilation).</summary>
        public HashSet<string> CompletedIncludeSources { get; } =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private readonly List<UtilityFunctionInitBinding> _utilityFunctionInitBindings =
            new List<UtilityFunctionInitBinding>();

        
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
            {typeof(ObjectCreationStatement), typeof(EvaluationStatement)},
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
        
        public string GetLastFunctionCallStorageVariable(TypeDescriptor typeDescriptor, TextWriter metaTextWriter)
        {
            const string VariableName = "LastFunctionCall";
            
            var existed = GeneralScope.ReserveOrUpdateNewVariable(typeDescriptor, VariableName);

            if (!existed)
            {
                BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(this, GeneralScope, metaTextWriter,
                    VariableName, "0");
            }
            
            return VariableName;
        }

        public void RegisterUtilityFunctionInitBinding(UtilityFunctionInitBinding binding)
        {
            _utilityFunctionInitBindings.Add(binding);
        }

        public void WriteUtilityFunctionInitSection(TextWriter metaWriter)
        {
            if (!Flags.BindThirdPartyUtilitiesAtInit || _utilityFunctionInitBindings.Count == 0)
            {
                return;
            }

            if (Flags.UseSegments)
            {
                GetMetaInfoTranspiler().WriteSeparator(this, metaWriter);
            }

            if (Flags.UseComments)
            {
                GetMetaInfoTranspiler().WriteComment(this, metaWriter,
                    "Resolve third-party utility backends once (keeps hot paths branch-free).");
            }

            foreach (var binding in _utilityFunctionInitBindings)
            {
                var branches = new List<KeyValuePair<string, string>>();
                foreach (var implementation in binding.UtilityBodies)
                {
                    if (!Api.Utilities.TryGetValue(implementation.Key, out var utility))
                    {
                        continue;
                    }

                    var condition = ApiBaseFunction.GetUtilityLookupTestVariableName(this, metaWriter, utility);
                    branches.Add(new KeyValuePair<string, string>(condition, implementation.Value));
                }

                var isFirst = true;
                var wroteBranch = false;

                foreach (var branch in branches)
                {
                    metaWriter.Write(isFirst ? "if [ " : "elif [ ");
                    isFirst = false;
                    wroteBranch = true;
                    metaWriter.Write(branch.Key);
                    metaWriter.WriteLine(" ]");
                    metaWriter.WriteLine("then");
                    metaWriter.Write("function ");
                    metaWriter.Write(binding.FunctionFqn);
                    metaWriter.WriteLine("() {");
                    metaWriter.WriteLine(branch.Value);
                    metaWriter.WriteLine("}");
                }

                if (!string.IsNullOrWhiteSpace(binding.PureBashFallbackBody))
                {
                    if (wroteBranch)
                    {
                        metaWriter.WriteLine("else");
                    }
                    else
                    {
                        metaWriter.WriteLine("if true");
                        metaWriter.WriteLine("then");
                    }
                    metaWriter.Write("function ");
                    metaWriter.Write(binding.FunctionFqn);
                    metaWriter.WriteLine("() {");
                    metaWriter.WriteLine(binding.PureBashFallbackBody);
                    metaWriter.WriteLine("}");
                    wroteBranch = true;
                }

                if (wroteBranch)
                {
                    metaWriter.WriteLine("fi");
                }

                metaWriter.WriteLine();
            }
        }
    }
}