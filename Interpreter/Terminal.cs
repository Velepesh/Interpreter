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
        //public string Identifier { get; private set; }
        public TokenType TokenType { get; private set; }
        private string _pattern;
        public int Priority { get; private set; }

        //public Terminal(string identifier, string pattern)
        //{
        //    this(identifier, pattern, 0);
        //}

        public Terminal(TokenType tokenType, string pattern, int priority = 0)
        {
            TokenType = tokenType;
            _pattern = pattern;
             Priority = priority;
        }

        public bool Matches(string charSequence)
        {
            return Regex.IsMatch(charSequence, _pattern);
            //return _pattern.matcher(charSequence).matches();
        }
    }
}
