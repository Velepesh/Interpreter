using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class MyLinkedList
    {
        private Element<object> _firstElement;
        private Element<object> _lastElement;
        private int count = 0;

        public void AddFirst(object value)
        {
            if (_firstElement == null)
            {
                _firstElement = new Element<object>(value);
                _lastElement = _firstElement;
            }
            else
            {
                Element<object> newElement = new Element<object>(value);
               
                _firstElement.SetPrevious(_lastElement);
                _lastElement.SetNext(newElement);
                _firstElement = newElement;
            }
        }

        public void AddLast(int value)
        {
            if (_lastElement == null)
            {
                _lastElement = new Element<object>(value);
                _firstElement = _lastElement;
            }
            else
            {
                Element<object> newElement = new Element<object>(value);

                _lastElement.SetNext(newElement);
                _lastElement = newElement;
            }
        }
        // Вставить элемент по индексу
        public void Add(int index, int value)
        {
            Element<object> element;

            for (element = _firstElement; element != null && index != 0; element = element.Next)
            {
                index--;
            }

            if (element == _firstElement)
            {
                AddFirst(value);
            }
            else if (element == null && _firstElement != null && index == 0)
            {
                AddLast(value);
            }
            else
            {
                Element<object> newElement = new Element<object>(value);

                element.Previous.SetNext(newElement);
                element.SetPrevious(newElement);
            }
        }

        private Element<object> GetNode(int index)
        {
            if (index >= count)
            {
                throw new IndexOutOfRangeException();
            }

            int i;
            Element<object> element;

            if (count == 1) 
            {
                element = _firstElement;
            } 
            else if (count == 2)
            {
                element = index == 0 ? _firstElement : _lastElement;
            }
            else if (index < count / 2)
            {
                i = 0;
                element = _firstElement;

                while (i < index)
                {
                    element = element.Next;
                     i++;
                }
            }
            else
            {
                i = count - 1;
                element = _lastElement;
                while (i > index)
                {
                    element = element.Previous;
                    i--;
                }
            }

            return element;
        }

        public int Peek(int index)
        {
            Element<object> element;

            for (element = _firstElement; element != _lastElement && index != 0; element = element.Next) 
            { 
                index--;
            }

            try
            {
                return element.Value;
            }
            finally { }
        }

        public object Get(int index)
        {
            if (index >= count)
            {
                throw new IndexOutOfRangeException();
            }

            return GetNode(index).Data;
        }

        public void Remove(int index)
        {
            if (index >= count) 
            {
                throw new IndexOutOfRangeException();
            }

            Element<object> element = GetNode(index);
            Element<object> prev = element.Previous;
            Element<object> next = element.Next;
                
            if (prev != null) 
            {
                prev.SetNext(next);
            } 
            else 
            {
                _firstElement = next;
            }
                
            if (next != null) 
            {
                next.SetPrevious(prev);
            } 
            else 
            {
                _lastElement = prev;
            }

            count--;
        }

        public bool Contains(object value)
        {
            for (Element<object> current = _firstElement; current != null; current = current.Next)
            {
                if (current.Data.GetHashCode()== value.GetHashCode())
                {
                    if (current.Data.Equals(value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public int Size()
        {
            return count;
        }

        public string PrintList()
        {
            if (_firstElement == null)
            {
                return "[]";
            }

            string str = "[";

            for (Element<object> current = _firstElement; current != null; current = current.Next)
            {
                str += current + ", ";
            }

            str += _lastElement.ToString() + "]";

            return str;
        }
    }
}
