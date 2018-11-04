namespace ShellScript.Core.Language.Compiler
{
    public struct CompilerFlags
    {
        //==========================
        // Syntax
        //==========================
        public bool SemicolonRequired { get; set; }

        //==========================
        // Inlining
        //==========================
        public bool UseInlining { get; set; }
        public bool InlineCascadingFunctionCalls { get; set; }
        public bool InlineNonEvaluations { get; set; }

        //==========================
        // Environmental Features
        //==========================
        public bool UseLastFunctionCallStorageVariable { get; set; }

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

        public bool PreferRandomHelperVariableNames { get; set; }


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

                UseInlining = true,
                InlineCascadingFunctionCalls = true,
                InlineNonEvaluations = true,

                SuccessStatusCode = 0,
                FailureStatusCode = 1,

                ExplicitEchoStream = null,
                DefaultExplicitEchoStream = "/dev/tty",

                UseComments = true,
                CommentParameterInfos = true,

                UseSegments = true,

                WriteShellScriptVersion = true,
            };
        }
    }
}