using System.Collections.Generic;
using System.Linq;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Array;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Convert;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Locale;
using ShellScript.Unix.Bash.Api.ClassLibrary.IO.File;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Math;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.String;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.User;
using ShellScript.Unix.Bash.Api.ClassLibrary.Network.Net;
using ShellScript.Unix.Utilities;

namespace ShellScript.Unix.Bash.Api
{
    public class UnixBashApi : ApiBase
    {
        public override IApiVariable[] Variables => new IApiVariable[0];
        public override IApiFunc[] Functions => new IApiFunc[0];

        public override IApiClass[] Classes { get; } =
        {
            new BashConvert(),
            
            new BashMath(),
            new BashString(),
            new BashArray(),
            
            new BashPlatform(),
            new BashUser(),

            new BashFile(),
            
            new BashLocale(),
            
            new BashNet(),
        };

        private IThirdPartyUtility[] _utilities =
        {
            new AwkThirdPartyUtility(),
            new BcThirdPartyUtility(),
            new BcThirdPartyUtility(),
        };

        public override IDictionary<string, IThirdPartyUtility> Utilities { get; }

        public UnixBashApi()
        {
            Utilities = new Dictionary<string, IThirdPartyUtility>(_utilities.ToDictionary(key => key.Name));
        }

        public override void InitializeContext(Context context)
        {
            base.InitializeContext(context);
            
            context.GeneralScope.ReserveNewVariable(TypeDescriptor.Integer, "?");
        }

        public override string Name => "Unix-Bash";
    }
}