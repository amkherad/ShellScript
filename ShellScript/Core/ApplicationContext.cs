using System;
using System.Collections.Generic;
using System.Reflection;
using ShellScript.CommandLine;

namespace ShellScript.Core
{
    public class ApplicationContext
    {
        public const string Url = "https://github.com/amkherad/ShellScript";
        
        
        public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;
    }
}