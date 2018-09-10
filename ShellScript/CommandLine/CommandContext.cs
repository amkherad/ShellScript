using System;
using System.Linq;

namespace ShellScript.CommandLine
{
    public class CommandContext
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] Tokens { get; }

        /// <summary>
        /// Switches presented in the command text.
        /// </summary>
        public Switch[] Switches { get; }

        /// <summary>
        /// The lower-cased command -if presented- (e.g. shellscript command blah blah) 
        /// </summary>
        public string Command { get; }

        public bool NoCommand => string.IsNullOrWhiteSpace(Command);

        public bool IsEmpty => Tokens.Length == 0;

        private StringComparer _commandComparer;
        private StringComparer _switchNameComparer;


        public CommandContext(string[] tokens, Switch[] switches, string command)
        {
            Tokens = tokens;
            Switches = switches;
            Command = command;

            _commandComparer = StringComparer.CurrentCultureIgnoreCase;
            _switchNameComparer = _commandComparer;
        }

        public bool IsCommand(string command)
        {
            var com = Command;
            if (com == null)
            {
                return false;
            }

            return _commandComparer.Equals(com, command);
        }

        public bool IsCommand(params string[] commands)
        {
            var com = Command;
            if (com == null)
            {
                return false;
            }

            foreach (var command in commands)
            {
                if (_commandComparer.Equals(com, command))
                    return true;
            }

            return false;
        }

        public bool AnySwitch(params string[] names)
        {
            return names.Any(name => Switches.Any(s => _switchNameComparer.Equals(s.Name, name)));
        }

        public Switch GetSwitch(string name)
        {
            return Switches.FirstOrDefault(s => _switchNameComparer.Equals(s.Name, name));
        }


        public static CommandContext Parse(string[] commands)
        {
            return new CommandContext(commands,
                commands.Where(c => c.StartsWith("-")).Select(Switch.Parse).ToArray(),
                commands.Length > 0 ? commands[0] : null);
        }

        public string GetToken(int index)
        {
            if (index >= Tokens.Length - 1)
            {
                return null;
            }

            return Tokens[index + 1];
        }
    }
}