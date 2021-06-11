using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Element<T>
    {
        public T Value { get; private set; }

        public Element<T> Next { get; private set; }
        public Element<T> Previous { get; private set; }

        public Element(T value)
        {
            Value = value;
        }

        public void SetNext(Element<T> next)
        {
            Next = next;
        }

        public void SetPrevious(Element<T> previous)
        {
            Previous = previous;
        }

        public override string ToString()
        {
            return Convert.ToString(Value);
        }
    }
}
