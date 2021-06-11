using System;
using System.Collections.Generic;

namespace Interpreter
{
    class LinkedList
    {
        private Element<object> _firstElement;
        private Element<object> _lastElement;
        private int _count = 0;

        public void AddFirst(object value)
        {
            Element<object> newElement = new Element<object>(value);

            if (_firstElement == null)
            {
                _firstElement = newElement;
                _lastElement = _firstElement;
            }
            else
            {
                newElement.SetNext(_firstElement);
                newElement.SetPrevious(null);

                _firstElement.SetPrevious(newElement);
                _firstElement = newElement;
            }

            IncreaseСounter();
        }

        public void AddLast(object value)
        {
            Element<object> newElement = new Element<object>(value);

            if (_lastElement == null)
            {
                _lastElement = newElement;
                _firstElement = _lastElement;
            }
            else
            {
                newElement.SetNext(null);
                newElement.SetPrevious(_lastElement);

                _lastElement.SetNext(newElement);
                _lastElement = newElement;
            }

            IncreaseСounter();
        }

        public void Add(int index, object value)
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

                newElement.SetNext(element);
                newElement.SetPrevious(element.Previous);

                element.Previous.SetNext(newElement);
                element.SetPrevious(newElement);

                IncreaseСounter();
            }
        }

        public object Get(int index)
        {
            if (index >= Size())
            {
                throw new IndexOutOfRangeException("Index was outside the bounds of the array");
            }

            return GetNode(index).ToString();
        }

        public object GetFirst()
        {
            if (_firstElement == null)
            {
                throw new IndexOutOfRangeException("Index was outside the bounds of the array");
            }

            return _firstElement.ToString();
        }

        public object GetLast()
        {
            if (_lastElement == null)
            {
                throw new IndexOutOfRangeException("Index was outside the bounds of the array");
            }

            return _lastElement.ToString();
        }

        public void Remove(int index)
        {
            if (index >= Size()) 
            {
                throw new IndexOutOfRangeException("Index was outside the bounds of the array");
            }

            Element<object> element = GetNode(index);
            Element<object> previous = element.Previous;
            Element<object> next = element.Next;
                
            if (previous != null) 
            {
                previous.SetNext(next);
            } 
            else 
            {
                _firstElement = next;
            }
                
            if (next != null) 
            {
                next.SetPrevious(previous);
            } 
            else 
            {
                _lastElement = previous;
            }

            DecreaseCounter();
        }

        public bool Contains(object value)
        {
            for (Element<object> current = _firstElement; current != null; current = current.Next)
            {
                if (current.Value.GetHashCode() == value.GetHashCode())
                {
                    if (current.Value.Equals(value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public int Size()
        {
            return _count;
        }

        public void PrintLinkedList()
        {
            Console.Write("[");

            for (Element<object> current = _firstElement; current != null; current = current.Next)
            {
                Console.Write(current.ToString());

                if (current.Next != null)
                    Console.Write(", ");
                else
                    Console.Write("]\n");
            }
        }

        private void IncreaseСounter()
        {
            _count++;
        }
        
        private void DecreaseCounter()
        {
            _count--;
        }

        private Element<object> GetNode(int index)
        {
            Element<object> element;

            for (element = _firstElement; element != _lastElement && index > 0; element = element.Next)
            {
                index--;    
            }

            if (element == _firstElement)
            {
                GetFirst();
            }
            else if (element == _lastElement && index == 0)
            {
                GetLast();
            }
            else
            {
                element.Previous.SetNext(element.Previous);
                element.Next.SetPrevious(element.Next);
            }

            return element;
        }
    }
}