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

        public void SetValue(string value)
        {
            Value = value;
        }

        public void Println()
        {
            //Console.WriteLine(String.Format("Х20s%-20s\n", TokenType, Value == null ? " " : Value));
            string value = Value == null ? " " : Value;
            Console.WriteLine($"{TokenType, -20}{value, -20}");
        }
    }
}
