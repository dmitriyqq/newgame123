using System.Collections;
using System.Collections.Generic;
using OpenTK.Graphics.ES11;

namespace GameRenderer
{
    public class WrapObjectEnumerable<T> : IEnumerable<T> where T : class
    {
        private T O;
        
        public WrapObjectEnumerable(T o)
        {
            O = o;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new WrapObjectEnumerator<T>(O);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new WrapObjectEnumerator<T>(O);
        }
    }

    public class WrapObjectEnumerator<T> : IEnumerator<T> where T : class
    {
        private T O;
        public WrapObjectEnumerator(T o)
        {
            O = o;
        }
        
        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
            // Do nothing
        }

        object IEnumerator.Current => O;

        public void Dispose()
        {
            // Do nothing
        }

        public T Current => O;
    }
}