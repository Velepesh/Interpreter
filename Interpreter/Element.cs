using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Element<T>
    {
        public T Data { get; private set; }
        public int Value { get; private set; }
        public Element<T> Next { get; private set; }
        public Element<T> Previous { get; private set; }

        public Element(T data)
        {
            Data = data;
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
            return Convert.ToString(Data);
        }
    }
}
