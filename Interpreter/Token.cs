using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Token
    {
        public Terminal Terminal { get; private set; }
        public string Value { get; private set; }

        public Token(Terminal terminal, string value)
        {
            Terminal = terminal;
            Value = value;
        }
    }
}
