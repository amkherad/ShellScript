namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ForEachStatement : IStatement
    {
        public bool IsBlockStatement => true;
        
        public IStatement Variable { get; }
        
        public VariableAccessStatement Iterator { get; }
        
        
        public ForEachStatement(IStatement variable, VariableAccessStatement iterator)
        {
            Variable = variable;
            Iterator = iterator;
        }
    }
}