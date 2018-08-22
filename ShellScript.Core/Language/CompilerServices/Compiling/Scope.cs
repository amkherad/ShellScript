namespace ShellScript.Core.Language.CompilerServices.Compiling
{
    public class Scope
    {
        public Context Context { get; }
        public Scope Parent { get; }
        
        
        public Scope(Context context)
        {
            Context = context;
        }

        public Scope(Context context, Scope parent)
        {
            Context = context;
            Parent = parent;
        }


        public Scope Clone()
        {
            return new Scope(Context, this);
        }
    }
}