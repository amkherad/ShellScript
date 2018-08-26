using System;

namespace ShellScript.Core.Language.CompilerServices.CompilerErrors
{
    public class CompilerException : Exception
    {
        public PositionInfo PositionInfo { get; }
        
        public CompilerException(string message, PositionInfo positionInfo)
            : base(message)
        {
            PositionInfo = positionInfo;
        }

        public CompilerException(string message, PositionInfo positionInfo, Exception innerException)
            : base(message, innerException)
        {
            PositionInfo = positionInfo;
        }

        public CompilerException(PositionInfo positionInfo, Exception innerException)
            : base("", innerException)
        {
            PositionInfo = positionInfo;
        }
    }
}