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
        private Dictionary<string, int> _variables = new Dictionary<string, int>();
        private Dictionary<string, MyLinkedList> _linkedListVariables = new Dictionary<string, MyLinkedList>();
        private Dictionary<string, MyHashSet> _hashSetVariables = new Dictionary<string, MyHashSet>();
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
                    case TokenType.FUNC:
                        FuncExpr(_rpn[_index]);
                        break;
                    case TokenType.PRINT:
                        PrintExpr(GetFirstFromStack());
                        break;
                    case TokenType.LINKED_LIST:
                        LinkedListExpr(GetFirstFromStack());
                        break;
                    case TokenType.HASH_SET:
                        HashSetExpr(GetFirstFromStack());
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

        private void SetTransitionIndex(Token jump, Token condition)
        {
            int jumpValue = Convert.ToInt32(jump.Value);
            bool conditionValue = Convert.ToBoolean(condition.Value);

            if (conditionValue == false)
                _index = jumpValue - 1;
        }

        private void FuncExpr(Token token)
        {
            Token name = GetFirstFromStack();

            if (_linkedListVariables.ContainsKey(name.Value))
            {
                if (token.Value.Equals("Add"))
                {
                    Token valueFromStack = GetFirstFromStack();
                    Token index = GetFirstFromStack();

                    int indexValue = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);
                    int value = valueFromStack.TokenType == TokenType.VAR ? _variables[valueFromStack.Value] : Convert.ToInt32(valueFromStack.Value);

                    _linkedListVariables[name.Value].Add(indexValue, value);
                }
                else if (token.Value.Equals("Size"))
                {
                    Token index = GetFirstFromStack();

                    int indexValue = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);

                    _stack.AddFirst(new Token(TokenType.NUMBER, _linkedListVariables[name.Value].Size()));
                }
                else if (token.Value.Equals("Get"))
                {
                    Token index = GetFirstFromStack();

                    int indexValue = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);

                    _stack.AddFirst(new Token(TokenType.NUMBER, (string)_linkedListVariables[name.Value].Get(indexValue)));
                }
                else if (token.Value.Equals("Contains"))
                {
                    Token index = GetFirstFromStack();

                    int value_value = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);

                    _stack.AddFirst(new Token(TokenType.BOOLEAN, _linkedListVariables[name.Value].Contains(value_value) ? Convert.ToString(false) : Convert.ToString(true)));
                }
                else if (token.Value.Equals("PrintList"))
                {
                    _linkedListVariables[name.Value].PrintList();
                    Console.WriteLine();
                }
            }
            else if (_hashSetVariables.ContainsKey(name.Value))
            {
                if (token.Value.Equals("Add"))
                {
                    Token value = GetFirstFromStack();

                    int value_value = value.TokenType == TokenType.VAR ? _variables[value.Value] : Convert.ToInt32(value.Value);

                    _hashSetVariables[name.Value].Add(value_value);
                }
                else if (token.Value.Equals("Contains"))
                {
                    Token index = GetFirstFromStack();

                    int value_value = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);

                    _stack.AddFirst(new Token(TokenType.BOOLEAN, _hashSetVariables[name.Value].Contains(value_value) ? Convert.ToString(false) : Convert.ToString(true)));
                }
                else if (token.Value.Equals("Remove"))
                {
                    Token index = GetFirstFromStack();

                    int value_value = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);

                    _hashSetVariables[name.Value].Remove(value_value);
                }
                else if (token.Value.Equals("PrintSet"))
                {

                    _hashSetVariables[name.Value].PrintSet();
                    Console.WriteLine();
                }
            }
        }

        private void PrintExpr(Token a)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            Console.WriteLine(a_value);///////нужен ли?
        }

        private void LinkedListExpr(Token token)
        {
            if ((_linkedListVariables.ContainsKey(token.Value) || _variables.ContainsKey(token.Value)) == false)
            {
                _linkedListVariables.Add(token.Value, new MyLinkedList());
            }
        }

        private void HashSetExpr(Token token)
        {
            if ((_linkedListVariables.ContainsKey(token.Value) || _variables.ContainsKey(token.Value)) == false)
            {
                _hashSetVariables.Add(token.Value, new MyHashSet());
            }
        }

        private void Assign(Token a, Token b)
        {
            string key = b.Value;
            int value = Convert.ToInt32(a.Value);

            if (_variables.ContainsKey(key))
                _variables[key] = value;
            else
                _variables.Add(key, value);
        }
        private Token NotEqual(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value != a_value));
        }

        private Token LessEqual(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value <= a_value));
        }

        private Token GreaterEqual(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value >= a_value));
        }

        private Token Less(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value < a_value));
        }

        private Token Greater(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value > a_value));
        }

        private Token Equal(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value == a_value));
        }

        private Token Div(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.NUMBER, Convert.ToString(b_value / a_value));
        }

        private Token Mult(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.NUMBER, Convert.ToString(b_value * a_value));
        }

        private Token Sum(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.NUMBER, Convert.ToString(a_value + b_value));
        }

        private Token Sub(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.NUMBER, Convert.ToString(b_value - a_value));
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
