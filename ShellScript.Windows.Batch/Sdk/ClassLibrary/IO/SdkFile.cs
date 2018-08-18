using System.Collections.Generic;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Windows.Batch.Sdk.ClassLibrary.IO
{
    public partial class SdkFile : ISdkClass
    {
        public string Name => "File";

        public IEnumerable<ISdkVariable> Variables { get; }
        public IEnumerable<ISdkFunc> Functions { get; }
    }
}