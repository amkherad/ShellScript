namespace ShellScript.Core.Language.CompilerServices.Statements
{
    /// <summary>
    /// Represents an statement.
    /// </summary>
    public interface IStatement
    {
        /// <summary>
        /// Determines whether this statement can make a block to be considered as non-empty block.
        /// </summary>
        /// <remarks>
        /// Function call is a embedded statement but assignment is not a embedded statement.
        /// To be more clear those statements that can be presented out of braces in a if statement is c#:
        /// if (XXX)
        ///     myMethod(); //VALID
        ///
        /// if (XXX)
        ///     var x = 2;  //INVALID
        /// </remarks>
        bool CanBeEmbedded { get; }
        
        StatementInfo Info { get; }
        
        IStatement[] TraversableChildren { get; }
    }
}