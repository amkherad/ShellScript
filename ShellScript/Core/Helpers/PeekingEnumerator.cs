using System.Collections;
using System.Collections.Generic;

namespace ShellScript.Core.Helpers
{
    public class PeekingEnumerator<TElement> : IEnumerator<TElement>
        where TElement : class
    {
        private IEnumerator<TElement> _enumerator;

        private TElement _next;
        private TElement _current;

        public PeekingEnumerator(IEnumerator<TElement> enumerator)
        {
            _enumerator = enumerator;
        }


        public bool MoveNext()
        {
            if (ReferenceEquals(_next, default))
            {
                var result = _enumerator.MoveNext();
                if (result)
                {
                    _current = _enumerator.Current;
                }

                return result;
            }

            _current = _next;
            _next = default;
            return true;
        }

        public bool TryPeek(out TElement peek)
        {
            if (ReferenceEquals(_next, default))
            {
                var result = _enumerator.MoveNext();
                if (result)
                {
                    _next = _enumerator.Current;
                    peek = _next;
                }
                else
                {
                    peek = default;
                }

                return result;
            }

            peek = _next;
            return true;
        }


        public void Reset()
        {
            _enumerator.Reset();

            _next = null;
            _current = null;
        }

        public TElement Current => _current;

        object IEnumerator.Current => Current;

        public void Dispose() => _enumerator.Dispose();
    }
}