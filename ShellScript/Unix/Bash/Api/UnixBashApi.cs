using System.Collections.Generic;
using System.Linq;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Cli;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Array;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Convert;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Environment;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Locale;
using ShellScript.Unix.Bash.Api.ClassLibrary.IO.Directory;
using ShellScript.Unix.Bash.Api.ClassLibrary.IO.File;
using ShellScript.Unix.Bash.Api.ClassLibrary.IO.Path;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Math;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.String;
using ShellScript.Unix.Bash.Api.ClassLibrary.Core.User;
using ShellScript.Unix.Bash.Api.ClassLibrary.Network.Net;
using ShellScript.Unix.Bash.Api.ClassLibrary.System.OS;
using ShellScript.Unix.Bash.Api.ClassLibrary.System.Process;
using ShellScript.Unix.Bash.Api.ClassLibrary.System.Thread;
using ShellScript.Unix.Bash.Api.ClassLibrary.Data.Ini;
using ShellScript.Unix.Bash.Api.ClassLibrary.Data.DotEnv;
using ShellScript.Unix.Bash.Api.ClassLibrary.Data.Json;
using ShellScript.Unix.Bash.Api.ClassLibrary.Data.Yaml;
using ShellScript.Unix.Bash.Api.ClassLibrary.Data.Xml;
using ShellScript.Unix.Bash.Api.ClassLibrary.Text;
using ShellScript.Unix.Bash.Api.ClassLibrary.DateTime;
using ShellScript.Unix.Bash.Api.ClassLibrary.IO.Binary;
using ShellScript.Unix.Bash.Api.ClassLibrary.Diagnostics;
using ShellScript.Unix.Bash.Api.ClassLibrary.Testing;
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
            new BashEnvironment(),
            new BashMath(),
            new BashString(),
            new BashStringBuilder(),
            new BashArray(),
            new BashPlatform(),
            new BashUser(),
            new BashFile(),
            new BashDirectory(),
            new BashPath(),
            new BashLocale(),
            new BashNet(),
            new BashOS(),
            new BashProcess(),
            new BashThread(),
            new BashIni(),
            new BashDotEnv(),
            new BashJson(),
            new BashYaml(),
            new BashXml(),
            new BashText(),
            new BashUnicode(),
            new BashDateTime(),
            new BashBinary(),
            new BashRegex(),
            new BashLog(),
            new BashConsole(),
            new BashCli(),
            new BashAssert(),
        };

        private IThirdPartyUtility[] _utilities =
        {
            new AwkThirdPartyUtility(),
            new BcThirdPartyUtility(),
            new PythonThirdPartyUtility(),
            new JqThirdPartyUtility(),
            new CurlThirdPartyUtility(),
            new YqThirdPartyUtility(),
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
