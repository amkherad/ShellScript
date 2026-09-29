namespace ShellScript.Core.Language.Compiler
{
    public class CompilerFlags
    {
        //==========================
        // Syntax
        //==========================
        public bool SemicolonRequired { get; set; }

        //==========================
        // Constant folding (compile-time evaluation of literal expressions)
        //==========================
        public bool UseConstantFolding { get; set; }

        //==========================
        // Inlining
        //==========================
        public bool UseInlining { get; set; }
        public bool InlineCascadingFunctionCalls { get; set; }
        public bool InlineNonEvaluations { get; set; }
        public bool UseStrongInlining { get; set; }
        public int MaxInlineDepth { get; set; }
        public int StrongInliningMaxStatements { get; set; }

        //==========================
        // Other optimizations
        //==========================
        public bool UseDeadBranchElimination { get; set; }
        public bool UsePinElimination { get; set; }

        //==========================
        // Environmental Features
        //==========================
        public bool UseLastFunctionCallStorageVariable { get; set; }

        /// <summary>
        /// When true, generated code may call external utilities such as awk, bc, or python.
        /// </summary>
        public bool UseThirdPartyUtilities { get; set; }

        /// <summary>
        /// When true, utility choice is resolved once in the generated script prologue instead of on every call.
        /// </summary>
        public bool BindThirdPartyUtilitiesAtInit { get; set; }

        /// <summary>
        /// Comma- or semicolon-separated utility names to skip (e.g. "python" or "awk,python").
        /// </summary>
        public string DisabledThirdPartyUtilities { get; set; }

        /// <summary>
        /// Preferred utility order, most convenient first. Default: awk,bc,python.
        /// </summary>
        public string ThirdPartyUtilityOrder { get; set; }

        public int SuccessStatusCode { get; set; }
        public int FailureStatusCode { get; set; }

        /// <summary>
        /// only on linux.
        /// </summary>
        /// <example>
        /// /dev/tty
        /// </example>
        public string ExplicitEchoStream { get; set; }

        public string DefaultExplicitEchoStream { get; set; }

        //==========================
        // Readability
        //==========================
        public bool UseComments { get; set; }

        public bool CommentParameterInfos { get; set; }

        public bool UseSegments { get; set; }

        /// <summary>
        /// When false, merged output is written as-is (no post-processors run).
        /// </summary>
        public bool PostProcessGeneratedCode { get; set; }

        /// <summary>When true, run the platform formatter on generated code (Unix-Bash: built-in indent formatter).</summary>
        public bool FormatGeneratedCode { get; set; }

        /// <summary>Indent width in spaces when <see cref="FormatGeneratedCode"/> is true.</summary>
        public int GeneratedCodeIndentColumns { get; set; }

        public bool PreferRandomHelperVariableNames { get; set; }

        public int ArrayManipulationColumnCount { get; set; }

        //==========================
        // Info
        //==========================
        public bool WriteShellScriptVersion { get; set; }

        public string Author { get; set; }
        public string ContactInfo { get; set; }
        public string WikiUrl { get; set; }


        public static CompilerFlags CreateDefault()
        {
            return new CompilerFlags
            {
                SemicolonRequired = true,

                UseConstantFolding = true,
                UseInlining = true,
                InlineCascadingFunctionCalls = true,
                InlineNonEvaluations = true,
                UseStrongInlining = true,
                MaxInlineDepth = 8,
                StrongInliningMaxStatements = 3,
                UseDeadBranchElimination = true,
                UsePinElimination = true,

                UseThirdPartyUtilities = true,
                BindThirdPartyUtilitiesAtInit = true,
                DisabledThirdPartyUtilities = "",
                ThirdPartyUtilityOrder = ThirdPartyUtilitySettings.DefaultUtilityOrder,

                SuccessStatusCode = 0,
                FailureStatusCode = 1,

                ExplicitEchoStream = null,
                DefaultExplicitEchoStream = "/dev/tty",

                UseComments = true,
                CommentParameterInfos = true,

                UseSegments = true,

                PostProcessGeneratedCode = true,
                FormatGeneratedCode = false,
                GeneratedCodeIndentColumns = 2,

                WriteShellScriptVersion = true,
                
                ArrayManipulationColumnCount = 80,
            };
        }
    }
}