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
        private int index;

        private Dictionary<string, int> variables = new Dictionary<string, int>();
        private LinkedList<Token> stack = new LinkedList<Token>();

        public StackMachine(List<Token> RPN)
        {
            _RPN = RPN;
        }

        public void run()
        {

            for (index = 0; index < _RPN.Count; index++)
            {
                switch (_RPN[index].TokenType)
                {
                    case TokenType.PLUS:
                        stack.AddFirst(sum(stack.removeFirst(), stack.removeFirst()));
                        break;
                    case TokenType.MINUS:
                        stack.AddFirst(sub(stack.removeFirst(), stack.removeFirst()));
                        break;
                    case TokenType.MULT:
                        stack.AddFirst(mul(stack.removeFirst(), stack.removeFirst()));
                        break;
                    case TokenType.DIV:
                        stack.AddFirst(div(stack.removeFirst(), stack.removeFirst()));
                        break;
                    case TokenType.ASSIGN:
                        assign(stack.removeFirst(), stack.removeFirst());
                        break;
                    case TokenType.EQUAL:
                        stack.AddFirst(equal(stack.removeFirst(), stack.removeFirst()));
                        break;
                    case TokenType.NOT_EQUAL:
                        stack.AddFirst(notEqual(stack.removeFirst(), stack.removeFirst()));
                        break;
                    case TokenType.GREATER:
                        stack.AddFirst(more(stack.removeFirst(), stack.removeFirst()));
                        break;
                    case TokenType.LESS:
                        stack.AddFirst(less(stack.removeFirst(), stack.removeFirst()));
                        break;
                    case TokenType.GREATER_EQUAL:
                        stack.AddFirst(moreEqual(stack.removeFirst(), stack.removeFirst()));
                        break;
                    case TokenType.LESS_EQUAL:
                        stack.AddFirst(lessEqual(stack.removeFirst(), stack.removeFirst()));
                        break;
                    case TokenType.IF:
                        ifConst(stack.removeFirst(), stack.removeFirst());
                        break;
                    case TokenType.ELIF:
                        ifConst(stack.removeFirst(), stack.removeFirst());
                        break;
                    case TokenType.WHILE:
                        whileConst(stack.removeFirst(), stack.removeFirst());
                        break;
                    case TokenType.JMP:
                        index = Convert.ToInt32(stack.removeFirst().Value) - 1;
                        break;
                    default:
                        stack.AddFirst(_RPN[index]);
                }
            }
        }

        private void whileConst(Token jmp, Token boolean)
        {

            int jmp_value = Convert.ToInt32(jmp.Value);
            bool bool_value = Convert.ToBoolean(boolean.Value);

            if (!bool_value)
                index = jmp_value - 1;
        }

        private Token notEqual(Token a, Token b)
        {

            int a_value = a.TokenType == TokenType.VAR ? variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value != a_value));
        }

        private Token lessEqual(Token a, Token b)
        {

            int a_value = a.TokenType == TokenType.VAR ? variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value <= a_value));
        }

        private Token moreEqual(Token a, Token b)
        {

            int a_value = a.TokenType == TokenType.VAR ? variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value >= a_value));
        }

        private Token less(Token a, Token b)
        {

            int a_value = a.TokenType == TokenType.VAR ? variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value < a_value));
        }

        private Token more(Token a, Token b)
        {

            int a_value = a.TokenType == TokenType.VAR ? variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value > a_value));
        }

        private void ifConst(Token jmp, Token boolean)
        {

            int jmp_value = Convert.ToInt32(jmp.Value);
            bool bool_value = Convert.ToBoolean(boolean.Value);

            if (!bool_value)
                index = jmp_value - 1;
        }

        private Token equal(Token a, Token b)
        {

            int a_value = a.TokenType == TokenType.VAR ? variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.BOOLEAN, Convert.ToString(b_value == a_value));
        }

        private Token div(Token a, Token b)
        {

            int a_value = a.TokenType == TokenType.VAR ? variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.NUMBER, Convert.ToString(b_value / a_value));
        }

        private Token mul(Token a, Token b)
        {

            int a_value = a.TokenType == TokenType.VAR ? variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.NUMBER, Convert.ToString(b_value * a_value));
        }

        private Token sub(Token a, Token b)
        {

            int a_value = a.TokenType == TokenType.VAR ? variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? variables[b.Value] : Convert.ToInt32(b.Value);

            return new Token(TokenType.NUMBER, Convert.ToString(b_value - a_value));
        }

        private void assign(Token a, Token b)
        {

            variables.put(b.Value, Convert.ToInt32(a.Value));
        }

        private Token sum(Token a, Token b)
        {

            int a_value = a.TokenType == TokenType.VAR ? variables[a.Value] : Convert.ToInt32(a.Value);
            int b_value = b.TokenType == TokenType.VAR ? variables[b.Value] : Convert.ToInt32(b.Value);

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
            Set<string> keys = variables.keySet();

            foreach (String name in keys)
            {

                Console.WriteLine(name + " " + variables[name]);
            }
        }
    }
}
