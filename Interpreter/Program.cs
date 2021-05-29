using System;
using System.IO;

namespace Interpreter
{
    public class Program
    {
       public static void Main(string[] args)
       {
            string expression = "";
            // string expression = "var c = a * b$";
            //string expression = "a * b c / d 4 + 3$";

            string writePath = @"..\..\Exemple.txt";

            try
            {
                using (StreamReader streamReader = new StreamReader(writePath))
                {
                    expression = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Lexer lexer = new Lexer();
            lexer.RunLexer(expression);
            //Console.WriteLine(" ++++++          " + lexer.GetTokens()[lexer.GetTokens().Count-1].Terminal.TokenType);
            Parser parser = new Parser(lexer.GetTokens());
            parser.analysis();
            parser.AstNode.print();
        }
    }
}
