using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class StackMachine
    {
        private List<Token> _RPN = new List<Token>();
        private Dictionary<string, int> _variables = new Dictionary<string, int>();
        private LinkedList<Token> _stack = new LinkedList<Token>();
        private int _index;

        public StackMachine(List<Token> RPN)
        {
            _RPN = RPN;
        }

        public void run()
        {
            for (_index = 0; _index < _RPN.Count; _index++)
            {
                switch (_RPN[_index].TokenType)
                {
                    case TokenType.PLUS:
                        //RemoveFromStack();////////////ЗДЕСЬ НЕ то ычи
                        _stack.AddFirst(sum(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.MINUS:
                        //RemoveFromStack();
                        _stack.AddFirst(sub(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.MULT:
                       // RemoveFromStack();
                        _stack.AddFirst(mul(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.DIV:
                        //RemoveFromStack();
                        _stack.AddFirst(div(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.ASSIGN:
                        //RemoveFromStack();
                        assign(RemoveFromStack(), RemoveFromStack());
                        break;
                    case TokenType.EQUAL:
                        //RemoveFromStack();
                        _stack.AddFirst(equal(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.NOT_EQUAL:
                        //RemoveFromStack();
                        _stack.AddFirst(notEqual(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.GREATER:
                       // RemoveFromStack();
                        _stack.AddFirst(more(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.LESS:
                        //RemoveFromStack();
                        _stack.AddFirst(less(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.GREATER_EQUAL:
                       // RemoveFromStack();
                        _stack.AddFirst(moreEqual(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.LESS_EQUAL:
                        //RemoveFromStack();
                        _stack.AddFirst(lessEqual(RemoveFromStack(), RemoveFromStack()));
                        break;
                    case TokenType.IF:
                        //RemoveFromStack();
                        ifConst(RemoveFromStack(), RemoveFromStack());
                        break;
                    case TokenType.ELIF:
                        //RemoveFromStack();
                        ifConst(RemoveFromStack(), RemoveFromStack());
                        break;
                    case TokenType.WHILE:
                        //RemoveFromStack();
                        whileConst(RemoveFromStack(), RemoveFromStack());
                        break;
                    case TokenType.JMP:
                        _stack.RemoveFirst();
                        _index = Convert.ToInt32(RemoveFromStack()) - 1;//////////
                        break;
                    default:
                        _stack.AddFirst(_RPN[_index]);
                        break;
                }
            }
        }

        private Token RemoveFromStack()////////
        {
            var token = _stack.First.Value;
            _stack.RemoveFirst();

            return token;
        }

        private void whileConst(Token jmp, Token boolean)
        {
            int jmp_value = Convert.ToInt32(jmp.Value);
            bool bool_value = Convert.ToBoolean(boolean.Value);

            if (!bool_value)
                _index = jmp_value - 1;
        }

        private Token notEqual(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value != a_value));
        }

        private Token lessEqual(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value <= a_value));
        }

        private Token moreEqual(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value >= a_value));
        }

        private Token less(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value < a_value));
        }

        private Token more(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value > a_value));
        }

        private void ifConst(Token jmp, Token boolean)
        {
            int jmp_value = Convert.ToInt32(jmp.Value);
            bool bool_value = Convert.ToBoolean(boolean.Value);

            if (!bool_value)
                _index = jmp_value - 1;
        }

        private Token equal(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value == a_value));
        }

        private Token div(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.NUMBER, Convert.ToString(b_value / a_value));
        }

        private Token mul(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.NUMBER, Convert.ToString(b_value * a_value));
        }

        private Token sub(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.NUMBER, Convert.ToString(b_value - a_value));
        }

        private void assign(Token a, Token b)
        {
            _variables.Add(b.Value, Convert.ToInt32(a.Value));////////
        }

        private Token sum(Token a, Token b)
        {
            int a_value = a.TokenType == TokenType.VAR ? _variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? _variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.NUMBER, Convert.ToString(a_value + b_value));
        }

        private bool isOp(Token token)
        {
            TokenType type = token.TokenType;

            return type == TokenType.PLUS || type == TokenType.MINUS
                || type == TokenType.DIV || type == TokenType.MULT
                || type == TokenType.EQUAL || type == TokenType.LESS
                || type == TokenType.LESS_EQUAL || type == TokenType.GREATER
                || type == TokenType.GREATER_EQUAL || type == TokenType.NOT_EQUAL;
        }

        public void print()
        {
            var keys = _variables.Keys;

            foreach (string name in keys)
            {

                Console.WriteLine(name + " " + _variables[name]);
            }
        }
    }
}
