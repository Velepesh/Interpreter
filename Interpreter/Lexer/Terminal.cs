using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Interpreter
{
    class Terminal
    {
        private string _pattern;

        public TokenType TokenType { get; private set; }
        public int Priority { get; private set; }

        public Terminal(TokenType tokenType, string pattern, int priority = 0)
        {
            TokenType = tokenType;
            _pattern = pattern;
             Priority = priority;
        }

        public bool IsMatches(string charSequence)
        {
            return Regex.IsMatch(charSequence, _pattern);
        }
    }
}
