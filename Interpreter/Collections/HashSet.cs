using System;

namespace Interpreter
{
    class HashSet
    {
        readonly private int _initialCapacity = 4;
        readonly private float _loadFactor = 0.75f;

        private LinkedList[] _lists;
        private int _capacity;

        public HashSet()
        {
            _lists = new LinkedList[_initialCapacity];

            _capacity = _initialCapacity;

            for (int index = 0; index < _lists.Length; index++)
            {
                _lists[index] = new LinkedList();
            }
        }

        public void Add(int item)
        {
            if (IsChangeCapacity())
            {
                ChangeCapacity();
            }

            int index = GetIndex(item);

            if (_lists[index].Contains(item) == false)
            {
                _lists[index].AddLast(item);
            }
        }

        public bool Contains(int item)
        {
            int index = GetIndex(item);
            LinkedList list = _lists[index];

            return list != null && list.Contains(item);
        }

        public void Remove(int item)
        {
            int index = GetIndex(item);

            LinkedList list = _lists[index];

            if (list != null && list.Contains(item))
            {
                for (int i = 0; i < list.Size(); i++)
                {
                    if (list.Get(i).Equals(item.ToString()))
                    {
                        list.Remove(i);
                    }
                }
            }
        }

        public void PrintHashSet()
        {
            string expression = "[";

            foreach (LinkedList list in _lists)
            {
                if (list != null)
                {
                    for (int i = 0; i < list.Size(); i++)
                    {
                        expression += list.Get(i) + ", ";
                    }
                }
            }

            if (expression.Length > 1)
                expression = expression.Substring(0, expression.Length - 2);

            expression += "]";

            Console.WriteLine(expression);
        }

        private bool IsChangeCapacity()
        {
            int amount = 0;

            foreach (LinkedList list in _lists)
            {
                if (list != null && list.Size() >= 2)
                {
                    amount++;
                }
            }

            return amount / _capacity >= _loadFactor;
        }

        private void ChangeCapacity()
        {
            int newCapacity = _capacity * 2;
            LinkedList[] newContent = new LinkedList[newCapacity];

            foreach (LinkedList list in _lists)
            {
                if (list != null)
                {
                    for (int i = 0; i < list.Size(); i++)
                    {
                        int item = Convert.ToInt32(list.Get(i));
                        int newIndex = GetIndex(item);

                        if (newContent[newIndex] == null)
                        {
                            newContent[newIndex] = new LinkedList();
                        }

                        newContent[newIndex].AddFirst(item);
                    }
                }
            }

            _lists = newContent;
            _capacity = newCapacity;
        }

        private int GetIndex(int item)
        {
            return item.GetHashCode() % _capacity;
        }
    }
}