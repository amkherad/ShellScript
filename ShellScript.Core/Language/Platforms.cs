using System;
using System.Collections.Generic;
using System.Linq;

namespace ShellScript.Core.Language
{
    public class Platforms
    {
        private static readonly List<IPlatform> AvailablePlatforms = new List<IPlatform>();

        public static void AddPlatform(IPlatform platform)
        {
            AvailablePlatforms.Add(platform);
        }

        public static IPlatform GetPlatformByName(string name)
        {
            return AvailablePlatforms.FirstOrDefault(x => StringComparer.CurrentCultureIgnoreCase.Equals(x.Name, name));
        }
        public static IPlatform GetSdkByName(string name)
        {
            return AvailablePlatforms.FirstOrDefault(x => x.Sdks.Any(sdk => StringComparer.CurrentCultureIgnoreCase.Equals(sdk.Name , name)) );
        }
    }
}