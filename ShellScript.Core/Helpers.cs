using System;
using System.Reflection;

namespace ShellScript.Core
{
    public static class Helpers
    {
        public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;
    }
}