using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class StackMachine
    {
        private List<Token> _rpn = new List<Token>();
        private Dictionary<string, object> _variables = new Dictionary<string, object>();
        private Dictionary<string, LinkedList> _linkedListVariables = new Dictionary<string, LinkedList>();
        private Dictionary<string, HashSet> _hashSetVariables = new Dictionary<string, HashSet>();
        private LinkedList<Token> _stack = new LinkedList<Token>();
        private int _index;

        public StackMachine(List<Token> rpn)
        {
            _rpn = rpn;
        }

        public void Run()
        {
            for (_index = 0; _index < _rpn.Count; _index++)
            {
                switch (_rpn[_index].TokenType)
                {
                    case TokenType.PLUS:
                        _stack.AddFirst(Sum(GetFirstFromStack(), GetFirstFromStack()));
                        break;
                    case TokenType.MINUS:
                        _stack.AddFirst(Sub(GetFirstFromStack(), GetFirstFromStack()));
                        break;
                    case TokenType.MULT:
                        _stack.AddFirst(Mult(GetFirstFromStack(), GetFirstFromStack()));
                        break;
                    case TokenType.DIV:
                        _stack.AddFirst(Div(GetFirstFromStack(), GetFirstFromStack()));
                        break;
                    case TokenType.ASSIGN:
                        Assign(GetFirstFromStack(), GetFirstFromStack());
                        break;
                    case TokenType.EQUAL:
                        _stack.AddFirst(Equal(GetFirstFromStack(), GetFirstFromStack()));
                        break;
                    case TokenType.NOT_EQUAL:
                        _stack.AddFirst(NotEqual(GetFirstFromStack(), GetFirstFromStack()));
                        break;
                    case TokenType.GREATER:
                        _stack.AddFirst(Greater(GetFirstFromStack(), GetFirstFromStack()));
                        break;
                    case TokenType.LESS:
                        _stack.AddFirst(Less(GetFirstFromStack(), GetFirstFromStack()));
                        break;
                    case TokenType.GREATER_EQUAL:
                        _stack.AddFirst(GreaterEqual(GetFirstFromStack(), GetFirstFromStack()));
                        break;
                    case TokenType.LESS_EQUAL:
                        _stack.AddFirst(LessEqual(GetFirstFromStack(), GetFirstFromStack()));
                        break;
                    case TokenType.IF:
                        SetTransitionIndex(GetFirstFromStack(), GetFirstFromStack());
                        break;
                    case TokenType.ELIF:
                        SetTransitionIndex(GetFirstFromStack(), GetFirstFromStack());
                        break;
                    case TokenType.WHILE:
                        SetTransitionIndex(GetFirstFromStack(), GetFirstFromStack());
                        break;
                    case TokenType.DO:
                        SetTransitionIndex(GetFirstFromStack(), GetFirstFromStack());
                        break;
                    case TokenType.FOR:
                        SetTransitionIndex(GetFirstFromStack(), GetFirstFromStack());
                        break;
                    case TokenType.METHOD:
                        MethodExpr(_rpn[_index]);
                        break;
                    case TokenType.PRINT:
                        PrintExpr(GetFirstFromStack());
                        break;
                    case TokenType.LINKED_LIST:
                        DeclarateLinkedListExpr(GetFirstFromStack());
                        break;
                    case TokenType.HASH_SET:
                        DeclarateHashSetExpr(GetFirstFromStack());
                        break;
                    case TokenType.JMP:
                        _index = Convert.ToInt32(GetFirstFromStack().Value) - 1;
                        break;
                    default:
                        _stack.AddFirst(_rpn[_index]);
                        break;
                }
            }
        }

        private Token GetFirstFromStack()
        {
            var token = _stack.First.Value;
            RemoveFirstFromStack();

            return token;
        }

        private void RemoveFirstFromStack()
        {
            _stack.RemoveFirst();
        }

        private void SetTransitionIndex(Token jump, Token condition)///////???????
        {
            int jumpValue = Convert.ToInt32(jump.Value);
            bool conditionValue = Convert.ToBoolean(condition.Value);

            if (conditionValue == false)
                _index = jumpValue - 1;
        }

        private void MethodExpr(Token rpn)
        {
            Token name = GetFirstFromStack();

            if (_linkedListVariables.ContainsKey(name.Value))
            {
                CallLinkedListMethod(rpn, name.Value);
            }
            
            if (_hashSetVariables.ContainsKey(name.Value))
            {
                CallHashSetMethod(rpn, name.Value);
            }
        }

        private void CallLinkedListMethod(Token rpn, string name)
        {
            if (rpn.Value.Equals("Add"))
            {
                Token valueFromStack = GetFirstFromStack();
                Token index = GetFirstFromStack();

                int indexValue =  Convert.ToInt32(index.Value);
                var value = valueFromStack.Value;

                _linkedListVariables[name].Add(indexValue, value);
            }
            else if(rpn.Value.Equals("AddFirst"))
            {
                Token valueFromStack = GetFirstFromStack();

                var value = valueFromStack.Value;

                _linkedListVariables[name].AddFirst(value);
            }
            else if(rpn.Value.Equals("AddLast"))
            {
                Token valueFromStack = GetFirstFromStack();

                var value = valueFromStack.Value;

                _linkedListVariables[name].AddLast(value);
            }
            else if (rpn.Value.Equals("Remove"))
            {
                Token index = GetFirstFromStack();

                int indexValue = Convert.ToInt32(index.Value);

                _linkedListVariables[name].Remove(indexValue);
            }
            else if (rpn.Value.Equals("Size"))
            {
                _stack.AddFirst(new Token(TokenType.NUMBER, _linkedListVariables[name].Size()));
            }
            else if (rpn.Value.Equals("Get"))
            {
                Token index = GetFirstFromStack();

                int indexValue = Convert.ToInt32(index.Value);

                _stack.AddFirst(new Token(TokenType.VAR, Convert.ToString(_linkedListVariables[name].Get(indexValue))));
            }
            else if (rpn.Value.Equals("GetFirst"))
            {
                Console.WriteLine(_linkedListVariables[name].Size() + " dsvdssd");
                _stack.AddFirst(new Token(TokenType.VAR, Convert.ToString(_linkedListVariables[name].GetFirst())));
            }
            else if (rpn.Value.Equals("GetLast"))
            {
                _stack.AddFirst(new Token(TokenType.VAR, Convert.ToString(_linkedListVariables[name].GetLast())));
            }
            else if (rpn.Value.Equals("Contains"))
            {
                Token index = GetFirstFromStack();

                var value = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);

                _stack.AddFirst(new Token(TokenType.BOOLEAN, _linkedListVariables[name].Contains(value) ? Convert.ToString(false) : Convert.ToString(true)));
            }
            else if (rpn.Value.Equals("PrintLinkedList"))
            {
                _linkedListVariables[name].PrintLinkedList();
                Console.WriteLine();
            }
        }

        private void CallHashSetMethod(Token rpn, string name)
        {
            if (rpn.Value.Equals("Add"))
            {
                Token index = GetFirstFromStack();

                var indexValue = Convert.ToInt32(index.Value);

                _hashSetVariables[name].Add(indexValue);
            }
            else if (rpn.Value.Equals("Contains"))
            {
                Token index = GetFirstFromStack();

                var indexValue = Convert.ToInt32(index.Value);

                _stack.AddFirst(new Token(TokenType.BOOLEAN, _hashSetVariables[name].Contains(indexValue) ? Convert.ToString(false) : Convert.ToString(true)));
            }
            else if (rpn.Value.Equals("Remove"))
            {
                Token index = GetFirstFromStack();

                var indexValue =  Convert.ToInt32(index.Value);

                _hashSetVariables[name].Remove(indexValue);
            }
            else if (rpn.Value.Equals("PrintHashSet"))
            {
                _hashSetVariables[name].PrintHashSet();
                Console.WriteLine();
            }
        }

        private void PrintExpr(Token a)
        {
            var value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            Console.WriteLine(value);///////нужен ли?
        }

        private void DeclarateLinkedListExpr(Token token)
        {
            if ((_linkedListVariables.ContainsKey(token.Value) || _variables.ContainsKey(token.Value)) == false)
            {
                _linkedListVariables.Add(token.Value, new LinkedList());
            }
        }

        private void DeclarateHashSetExpr(Token token)
        {
            if ((_linkedListVariables.ContainsKey(token.Value) || _variables.ContainsKey(token.Value)) == false)
            {
                _hashSetVariables.Add(token.Value, new HashSet());
            }
        }

        private void Assign(Token a, Token b)
        {
            string assign = b.Value;
            int value = Convert.ToInt32(a.Value);

            if (_variables.ContainsKey(assign))
                _variables[assign] = value;
            else
                _variables.Add(assign, value);
        }

        private Token NotEqual(Token a, Token b)
        {
            return new Token(TokenType.BOOLEAN, Convert.ToString(GetValue(b) != GetValue(a)));
        }

        private Token LessEqual(Token a, Token b)
        {
            return new Token(TokenType.BOOLEAN, Convert.ToString(GetValue(b) <= GetValue(a)));
        }

        private Token GreaterEqual(Token a, Token b)
        {
            return new Token(TokenType.BOOLEAN, Convert.ToString(GetValue(b) >= GetValue(a)));
        }

        private Token Less(Token a, Token b)
        {
            return new Token(TokenType.BOOLEAN, Convert.ToString(GetValue(b) < GetValue(a)));
        }

        private Token Greater(Token a, Token b)
        {
            return new Token(TokenType.BOOLEAN, Convert.ToString(GetValue(b) > GetValue(a)));
        }

        private Token Equal(Token a, Token b)
        {
            return new Token(TokenType.BOOLEAN, Convert.ToString(GetValue(b) == GetValue(a)));
        }

        private Token Div(Token a, Token b)
        {
            return new Token(TokenType.NUMBER, Convert.ToString(GetValue(b) / GetValue(a)));
        }

        private Token Mult(Token a, Token b)
        {
            return new Token(TokenType.NUMBER, Convert.ToString(GetValue(b) * GetValue(a)));
        }

        private Token Sum(Token a, Token b)
        {
            return new Token(TokenType.NUMBER, Convert.ToString(GetValue(a) + GetValue(b)));
        }

        private Token Sub(Token a, Token b)
        {
            return new Token(TokenType.NUMBER, Convert.ToString(GetValue(b) - GetValue(a)));
        }

        private int GetValue(Token valueToken)
        {
            return valueToken.TokenType == TokenType.VAR ? (int)_variables[valueToken.Value] : Convert.ToInt32(valueToken.Value);
        }

        public void Print()
        {
            var keys = _variables.Keys;

            foreach (string name in keys)
            {
                Console.WriteLine(name + " " + _variables[name]);
            }
        }
    }
}
