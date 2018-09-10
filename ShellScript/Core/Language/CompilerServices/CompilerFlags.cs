namespace ShellScript.Core.Language.CompilerServices
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
        
        /// <summary>
        /// only on linux.
        /// </summary>
        /// <example>
        /// /dev/tty
        /// </example>
        public string ExplicitEchoStream { get; set; }
        public string DefaultExplicitEchoStream { get; set; }

        public const string ExplicitEchoStreamSwitch = "explicit-echo-dev";

        public const string DefaultExplicitEchoStreamSwitch = "default-explicit-echo-dev";
        

        public static CompilerFlags CreateDefault()
        {
            return new CompilerFlags
            {
                SemicolonRequired = true,
                
                UseInlining = true,
                InlineCascadingFunctionCalls = true,
                InlineNonEvaluations = true,
                
                ExplicitEchoStream = null,
                DefaultExplicitEchoStream = "/dev/tty"
            };
        }
    }
}