using System.Collections.Generic;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Unix.Bash.Sdk.ClassLibrary.IO
{
    public partial class SdkFile
    {
        public class Exists : ISdkFunc
        {
            public string Name => "Exists";

            public bool AllowDynamicParams => false;

            public IEnumerable<ISdkParameter> Parameters { get; } = new[]
            {
                new SdkParameter("FilePath", DataTypes.String), 
            };
        }
    }
}