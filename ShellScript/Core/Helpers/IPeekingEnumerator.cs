using System.Collections.Generic;

namespace ShellScript.Core.Helpers
{
    public interface IPeekingEnumerator<TElement> : IEnumerator<TElement>
        where TElement : class
    {
        bool TryPeek(out TElement peek);
    }
}