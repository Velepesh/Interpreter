using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class MyHashSet
    {
        readonly private int _startNumberOfCell = 4;

        private MyLinkedList[] _lists;
        private int _numberOfCell;

        public MyHashSet()
        {
            _lists = new MyLinkedList[_startNumberOfCell];

            _numberOfCell = _startNumberOfCell;

            for (int index = 0; index < _lists.Length; index++)
            {
                _lists[index] = new MyLinkedList();
            }
        }

        private bool IsRehash()
        {
            int amount = 0;

            foreach (MyLinkedList list in _lists)
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
            MyLinkedList[] newContent = new MyLinkedList[newCount];

            foreach (MyLinkedList list in _lists)
            {
                if (list != null)
                {
                    for (int i = 0; i < list.Size(); i++)
                    {
                        object element = list.Get(i);
                        int newIndex = element.GetHashCode() % newCount;

                        if (newContent[newIndex] == null)
                        {
                            newContent[newIndex] = new MyLinkedList();
                        }

                        newContent[newIndex].AddFirst(element);
                    }
                }
            }

            _lists = newContent;
            _numberOfCell = newCount;
        }

        private int GetKeyValue(object value)
        {
            return value.GetHashCode() % _numberOfCell;
        }

        public void Add(object value)
        {
            if (IsRehash())
            {
                Rehash();
            }

            int h = GetKeyValue(value);

            if (!_lists[h].Contains(value))
            {
                _lists[h].AddFirst(value);
            }
        }

        public bool Contains(object value)
        {
            int h = GetKeyValue(value);
            return _lists[h] != null && _lists[h].Contains(value);
        }

        public void Remove(object value)
        {
            int keyValue = GetKeyValue(value);
            string objectString = value.ToString();

            MyLinkedList list = _lists[keyValue];

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

        public string PrintSet()
        {
            string str = "{";
            
            foreach (MyLinkedList list in _lists)
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
            
            str += "}";

            return str;
        }
    }
}
