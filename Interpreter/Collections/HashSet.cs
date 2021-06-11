using System;

namespace Interpreter
{
    class HashSet
    {
        readonly private int _startNumberOfCell = 4;

        private LinkedList[] _lists;
        private int _numberOfCell;

        public HashSet()
        {
            _lists = new LinkedList[_startNumberOfCell];

            _numberOfCell = _startNumberOfCell;

            for (int index = 0; index < _lists.Length; index++)
            {
                _lists[index] = new LinkedList();
            }
        }

        private bool IsRehash()
        {
            int amount = 0;

            foreach (LinkedList list in _lists)
            {
                if (list != null && list.Size() >= 2)
                {
                    amount++;
                }
            }

            return amount / _numberOfCell >= 0.75f;
        }

        private void Rehash()
        {
            int newCount = _numberOfCell * 2;
            LinkedList[] newContent = new LinkedList[newCount];

            foreach (LinkedList list in _lists)
            {
                if (list != null)
                {
                    for (int i = 0; i < list.Size(); i++)
                    {
                        object element = list.Get(i);
                        int newIndex = element.GetHashCode() % newCount;

                        if (newContent[newIndex] == null)
                        {
                            newContent[newIndex] = new LinkedList();
                        }

                        newContent[newIndex].AddFirst(element);
                    }
                }
            }

            _lists = newContent;
            _numberOfCell = newCount;
        }

        private int GetIndex(object value)
        {
            return value.GetHashCode() % _numberOfCell;
        }

        public void Add(object value)
        {
            if (IsRehash())
            {
                Rehash();
            }

            int index = GetIndex(value);

            if (!_lists[index].Contains(value))
            {
                _lists[index].AddFirst(value);
            }
        }

        public bool Contains(object value)
        {
            int index = GetIndex(value);
            return _lists[index] != null && _lists[index].Contains(value);
        }

        public void Remove(object value)
        {
            int index = GetIndex(value);
            string objectString = value.ToString();

            LinkedList list = _lists[index];

            if (list != null && list.Contains(objectString))
            {
                for (int i = 0; i < list.Size(); i++)
                {
                    if (list.Get(i).Equals(objectString))
                    {
                        list.Remove(i);
                    }
                }
            }
        }     

        public void PrintHashSet()
        {
            string str = "[";

            foreach (LinkedList list in _lists)
            {
                if (list != null)
                {
                    for (int i = 0; i < list.Size(); i++)
                    {
                        str += list.Get(i) + ", ";
                    }
                }
            }

            if (str.Length > 1)
                str = str.Substring(0, str.Length - 2);

            str += "]";
            Console.WriteLine(str);
            //return str;

            //Console.Write("[");

            //foreach (LinkedList list in _lists)
            //{
            //    for (int i = 0; i < list.Size(); i++)
            //    {
            //        Console.Write(list.Get(i));
            //        Console.Write(" ");
            //    }
            //}

            //Console.Write("]");
        }
    }
}
