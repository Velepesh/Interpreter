using System;
using System.IO;

namespace Interpreter
{
    public class Program
    {
       public static void Main(string[] args)
       {
            string expression = "";

            string path = @"..\..\Example.txt";

            try
            {
                using (StreamReader streamReader = new StreamReader(path))
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
            
            Parser parser = new Parser(lexer.GetTokens());
            parser.Parse();

            Rpn rpn = new Rpn(parser.AstNode);
            rpn.Translate();
            rpn.Print();

            StackMachine stackMachine = new StackMachine(rpn.GetRpn());
            stackMachine.Calculate();
            stackMachine.Print();
        }
    }
}
