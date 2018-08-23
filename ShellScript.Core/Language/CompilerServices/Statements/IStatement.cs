using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public interface IStatement
    {
        /// <summary>
        /// Determines whether this statement has child statements or not.
        /// </summary>
        bool IsBlockStatement { get; }
        
        
        IEnumerable<IStatement> TraversableChildren { get; }
    }
}