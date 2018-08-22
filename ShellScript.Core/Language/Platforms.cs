using System;
using System.Collections.Generic;
using System.Linq;

namespace ShellScript.Core.Language
{
    public class Platforms
    {
        private static readonly List<IPlatform> AllPlatforms = new List<IPlatform>();

        public static IPlatform[] AvailablePlatforms => AllPlatforms.ToArray();
        
        public static void AddPlatform(IPlatform platform)
        {
            AllPlatforms.Add(platform);
        }

        public static IPlatform GetPlatformByName(string name)
        {
            return AllPlatforms.FirstOrDefault(x => StringComparer.CurrentCultureIgnoreCase.Equals(x.Name, name));
        }
    }
}