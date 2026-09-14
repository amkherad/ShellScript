using System.Collections.Generic;

namespace ShellScript.Core.Language.Compiler.Transpiling
{
    public sealed class UtilityFunctionInitBinding
    {
        public string FunctionFqn { get; }
        public IReadOnlyList<KeyValuePair<string, string>> UtilityBodies { get; }
        public string PureBashFallbackBody { get; }

        public UtilityFunctionInitBinding(
            string functionFqn,
            IReadOnlyList<KeyValuePair<string, string>> utilityBodies,
            string pureBashFallbackBody)
        {
            FunctionFqn = functionFqn;
            UtilityBodies = utilityBodies;
            PureBashFallbackBody = pureBashFallbackBody;
        }
    }
}
