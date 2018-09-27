using System.Collections.Generic;
using System.Linq;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.IO.File;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Math;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform;
using ShellScript.Unix.Utilities;

namespace ShellScript.Unix.Bash.Api
{
    public class UnixBashApi : ApiBase
    {
        public override IApiVariable[] Variables => new IApiVariable[0];
        public override IApiFunc[] Functions => new IApiFunc[0];

        public override IApiClass[] Classes { get; } =
        {
            new ApiMath(),
            
            new ApiPlatform(),

            new ApiFile(),
        };

        private IThirdPartyUtility[] _utilities =
        {
            new AwkThirdPartyUtility(),
            new BcThirdPartyUtility(),
        };

        public override IDictionary<string, IThirdPartyUtility> Utilities { get; }

        public UnixBashApi()
        {
            Utilities = new Dictionary<string, IThirdPartyUtility>(_utilities.ToDictionary(key => key.Name));
        }

        public override string Name => "Unix-Bash";
    }
}