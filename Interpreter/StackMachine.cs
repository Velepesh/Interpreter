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
        private Dictionary<string, MyLinkedList> _variablesLinkedList = new Dictionary<string, MyLinkedList>();
        private Dictionary<string, MyHashSet> _variablesHashSet = new Dictionary<string, MyHashSet>();
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
                        _stack.AddFirst(Sum(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.MINUS:
                        _stack.AddFirst(Sub(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.MULT:
                        _stack.AddFirst(Mult(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.DIV:
                        _stack.AddFirst(Div(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.ASSIGN:
                        Assign(RemoveFromStack(), RemoveFromStack());
                        break;
                    case TokenType.EQUAL:
                        _stack.AddFirst(Equal(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.NOT_EQUAL:
                        _stack.AddFirst(NotEqual(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.GREATER:
                        _stack.AddFirst(Greater(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.LESS:
                        _stack.AddFirst(Less(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.GREATER_EQUAL:
                        _stack.AddFirst(GreaterEqual(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.LESS_EQUAL:
                        _stack.AddFirst(LessEqual(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.IF:
                        IfExpr(RemoveFromStack(), RemoveFromStack());
                        break;
                    case TokenType.ELIF:
                        IfExpr(RemoveFromStack(), RemoveFromStack());
                        break;
                    case TokenType.WHILE:
                        WhileExpr(RemoveFromStack(), RemoveFromStack());
                        break;
                    case TokenType.DO:
                        DoWhileExpr(RemoveFromStack(), RemoveFromStack());
                        break;
                    case TokenType.FOR:
                        ForExpr(RemoveFromStack(), RemoveFromStack());
                        break;
                    case TokenType.FUNC:
                        FuncExpr(_rpn[_index]);
                        break;
                    case TokenType.PRINT:
                        PrintExpr(RemoveFromStack());
                        break;
                    case TokenType.LINKED_LIST:
                        LinkedListExpr(RemoveFromStack());
                        break;
                    case TokenType.HASH_SET:
                        HashSetExpr(RemoveFromStack());
                        break;
                    case TokenType.JMP:
                        _index = Convert.ToInt32(RemoveFromStack().Value) - 1;
                        break;
                    default:
                        _stack.AddFirst(_rpn[_index]);
                        break;
                }
            }
        }

        private Token RemoveFromStack()
        {
            var token = _stack.First.Value;
            _stack.RemoveFirst();

            return token;
        }

        private void WhileExpr(Token jmp, Token boolean)
        {
            int jmpValue = Convert.ToInt32(jmp.Value);
            bool boolValue = Convert.ToBoolean(boolean.Value);

            if (!boolValue)
                _index = jmpValue - 1;
        }

        private void IfExpr(Token jmp, Token boolean)
        {
            int jmpValue = Convert.ToInt32(jmp.Value);
            bool boolValue = Convert.ToBoolean(boolean.Value);

            if (!boolValue)
                _index = jmpValue - 1;
        }

        private void DoWhileExpr(Token jmp, Token boolean)
        {
            int jmpValue = Convert.ToInt32(jmp.Value);
            bool boolValue = Convert.ToBoolean(boolean.Value);

        if (boolValue)
            _index = jmpValue - 1;
        }

        private void ForExpr(Token jmp, Token boolean)
        {
            int jmpValue = Convert.ToInt32(jmp.Value);
            bool boolValue = Convert.ToBoolean(boolean.Value);

            if (!boolValue)
                _index = jmpValue - 1;
        }

        private void FuncExpr(Token token)
        {
            Token name = RemoveFromStack();

            if (_variablesLinkedList.ContainsKey(name.Value))
            {
                if (token.Value.Equals("Add"))
                {
                    Token valueFromStack = RemoveFromStack();
                    Token index = RemoveFromStack();

                    int indexValue = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);
                    int value = valueFromStack.TokenType == TokenType.VAR ? _variables[valueFromStack.Value] : Convert.ToInt32(valueFromStack.Value);

                    _variablesLinkedList[name.Value].Add(indexValue, value);
                }
                else if (token.Value.Equals("Size"))
                {
                    Token index = RemoveFromStack();

                    int indexValue = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);

                    _stack.AddFirst(new Token(TokenType.NUMBER, _variablesLinkedList[name.Value].Peek(indexValue)));
                }
                else if (token.Value.Equals("Get"))
                {
                    Token index = RemoveFromStack();

                    int indexValue = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);

                    _stack.AddFirst(new Token(TokenType.NUMBER, (string)_variablesLinkedList[name.Value].Get(indexValue)));
                }
                else if (token.Value.Equals("Contains"))
                {
                    Token index = RemoveFromStack();

                    int value_value = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);

                    _stack.AddFirst(new Token(TokenType.BOOLEAN, _variablesLinkedList[name.Value].Contains(value_value) ? Convert.ToString(false) : Convert.ToString(true)));
                }
                else if (token.Value.Equals("PrintList"))
                {
                    _variablesLinkedList[name.Value].PrintList();
                    Console.WriteLine();
                }
            }
            else if (_variablesHashSet.ContainsKey(name.Value))
            {
                if (token.Value.Equals("Add"))
                {
                    Token value = RemoveFromStack();

                    int value_value = value.TokenType == TokenType.VAR ? _variables[value.Value] : Convert.ToInt32(value.Value);

                    _variablesHashSet[name.Value].Add(value_value);
                }
                else if (token.Value.Equals("Contains"))
                {
                    Token index = RemoveFromStack();

                    int value_value = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);

                    _stack.AddFirst(new Token(TokenType.BOOLEAN, _variablesHashSet[name.Value].Contains(value_value) ? Convert.ToString(false) : Convert.ToString(true)));
                }
                else if (token.Value.Equals("Remove"))
                {
                    Token index = RemoveFromStack();

                    int value_value = index.TokenType == TokenType.VAR ? _variables[index.Value] : Convert.ToInt32(index.Value);

                    _variablesHashSet[name.Value].Remove(value_value);
                }
                else if (token.Value.Equals("PrintSet"))
                {

                    _variablesHashSet[name.Value].PrintSet();
                    Console.WriteLine();
                }
            }
        }

        private void PrintExpr(Token a)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            Console.WriteLine(a_value);
        }

        private void LinkedListExpr(Token token)
        {
            if ((_variablesLinkedList.ContainsKey(token.Value) || _variables.ContainsKey(token.Value)) == false)
            {
                _variablesLinkedList.Add(token.Value, new MyLinkedList());
            }
        }

        private void HashSetExpr(Token token)
        {
            if ((_variablesLinkedList.ContainsKey(token.Value) || _variables.ContainsKey(token.Value)) == false)
            {
                _variablesHashSet.Add(token.Value, new MyHashSet());
            }
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

        private void Assign(Token a, Token b)
        {
            _variables.Add(b.Value, Convert.ToInt32(a.Value));
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
