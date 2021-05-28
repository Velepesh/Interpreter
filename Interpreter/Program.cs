using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            string expression = "var a var b  var c = a * b$";
           // string expression = "var c = a * b$";
            //string expression = "a * b c / d 4 + 3$";

            Lexer lexer = new Lexer();

            lexer.RunLexer(expression);
        }
    }
}
