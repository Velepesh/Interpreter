using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Token
    {
        public TokenType TokenType { get; private set; }
        public string Value { get; private set; }

        public Token(TokenType tokenType, string value = null)
        {
            TokenType = tokenType;
            Value = value;
        }
        public Token(TokenType tokenType, int value)
        {
            TokenType = tokenType;
            Value = Convert.ToString(value);
        }

        public void SetValue(string value)
        {
            Value = value;
        }

        public void Print()
        {
            string value = Value == null ? " " : Value;

            Console.WriteLine($"{TokenType, -15}{value}");
        }
    }
}
