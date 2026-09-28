using System.Collections.Generic;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Library;
using ShellScript.Windows.Batch.Api.ClassLibrary.Diagnostics;

namespace ShellScript.Windows.Batch.Api
{
    public class WindowsBatchApi : ApiBase
    {
        public override string Name => "Windows-Batch";

        public override IApiVariable[] Variables => new IApiVariable[0];
        public override IApiFunc[] Functions => new IApiFunc[0];

        public override IApiClass[] Classes { get; } =
        {
            new BatchConsole(),
        };

        public override IDictionary<string, IThirdPartyUtility> Utilities { get; } =
            new Dictionary<string, IThirdPartyUtility>();
    }
}
