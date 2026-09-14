using System.Collections.Generic;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Base
{
    public static class BashMathUtilityBodies
    {
        public static Dictionary<string, string> Unary(string awkExpression, string bcExpression, string pythonExpression)
        {
            return new Dictionary<string, string>
            {
                {
                    BashFunction.AwkUtilityName,
                    $"awk -v a=\"$1\" 'BEGIN {{ {awkExpression} }}'"
                },
                {
                    BashFunction.BcUtilityName,
                    $"echo \"scale=10; {bcExpression}\" | bc -l"
                },
                {
                    BashFunction.PythonUtilityName,
                    $"python3 -c 'import sys,math; a=float(sys.argv[1]); {pythonExpression}' \"$1\""
                },
            };
        }

        public static Dictionary<string, string> Binary(string awkExpression, string bcExpression,
            string pythonExpression)
        {
            return new Dictionary<string, string>
            {
                {
                    BashFunction.AwkUtilityName,
                    $"awk -v a=\"$1\" -v b=\"$2\" 'BEGIN {{ {awkExpression} }}'"
                },
                {
                    BashFunction.BcUtilityName,
                    $"echo \"scale=10; {bcExpression}\" | bc -l"
                },
                {
                    BashFunction.PythonUtilityName,
                    $"python3 -c 'import sys,math; a=float(sys.argv[1]); b=float(sys.argv[2]); {pythonExpression}' \"$1\" \"$2\""
                },
            };
        }
    }
}
