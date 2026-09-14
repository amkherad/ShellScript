using System;
using System.Collections.Generic;
using System.Linq;

namespace ShellScript.Core.Language.Compiler
{
    public static class ThirdPartyUtilitySettings
    {
        public const string DefaultUtilityOrder = "awk,bc,python";

        public static IReadOnlyList<string> GetOrderedUtilityNames(CompilerFlags flags)
        {
            if (!flags.UseThirdPartyUtilities)
            {
                return Array.Empty<string>();
            }

            var order = string.IsNullOrWhiteSpace(flags.ThirdPartyUtilityOrder)
                ? DefaultUtilityOrder
                : flags.ThirdPartyUtilityOrder;

            var disabled = ParseNameList(flags.DisabledThirdPartyUtilities);
            var disabledSet = new HashSet<string>(disabled, StringComparer.Ordinal);

            return order.Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0 && !disabledSet.Contains(x))
                .ToArray();
        }

        public static IEnumerable<KeyValuePair<string, string>> OrderImplementations(
            CompilerFlags flags,
            IDictionary<string, string> implementationsByUtility)
        {
            foreach (var utilityName in GetOrderedUtilityNames(flags))
            {
                if (implementationsByUtility.TryGetValue(utilityName, out var body))
                {
                    yield return new KeyValuePair<string, string>(utilityName, body);
                }
            }
        }

        private static IEnumerable<string> ParseNameList(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Array.Empty<string>();
            }

            return value.Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0);
        }
    }
}
