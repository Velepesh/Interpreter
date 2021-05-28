using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class Lexer
    {
        private List<Terminal> _terminals = new List<Terminal> 
        {
            //new Terminal("VAR", "^[a-zA-Z_]{1}\\w*$"),
            new Terminal("VAR", "^[a-zA-Z]*$"),
            new Terminal("NUMBER", "^0|[1-9][0-9]*$"),
            new Terminal("ASSIGN_OP", "^=$"),
            new Terminal("LOGICAL_OP", "==|>|<|!=$"),
            new Terminal("WHILE_KW", "^while$", 1),
            new Terminal("FOR_KW", "^for$", 1),
            new Terminal("IF_KW", "^if$", 1),
            new Terminal("ELSE_KW", "^else$", 1),
            new Terminal("DO_KW", "^do$", 1),
            new Terminal("L_BR", "^\\($"),
            new Terminal("R_BR", "^\\)$"),
            new Terminal("L_S_BR", "^\\{$"),
            new Terminal("R_S_BR", "^\\}$"),
            new Terminal("SEMICOLON","^;$"),
            new Terminal("VAR_TYPE", "^int|str|float$", 1),
            //new Terminal("OP", "^[+-/*]|\\+\\+|\\-\\-$"),
            new Terminal("PLUS", "^\\+$"),
            new Terminal("MINUS", "^\\-$"),
            new Terminal("MULT", "^\\*$"),
            new Terminal("DIV", "^\\/$"),
            new Terminal("WS", "^\\s+$")
        };
               

        //public static void Main(string[] args)
        //{
        //    stringBuilder input = new stringBuilder(LookupInput(args));
        //    List<Token> tokenes = new List<Token>();

        //    while (input[0] != '$')
        //    {
        //        Token token = ExtractNextLexeme(input);
        //        tokenes.Add(token);
        //        input.Remove(0, token.Value.Length);
        //    }

        //    Print(tokenes);
        //}

        public void RunLexer(string expression)
        {
            //stringBuilder input = new stringBuilder(LookupInput(args));
            StringBuilder input = new StringBuilder(expression);
            List<Token> tokenes = new List<Token>();

            while (input[0] != '$')
            {
                Token token = ExtractNextLexeme(input);
                // tokenes.Add(token);
                if (!token.Terminal.Identifier.Equals("WS")) 
                    tokenes.Add(token);

                input.Remove(0, token.Value.Length);
            }

            Print(tokenes);
            //Console.ReadKey();
        }

        private Token ExtractNextLexeme(StringBuilder input)
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append(input[0]);

            if (AnyTerminalMatches(buffer))
            {
                while (AnyTerminalMatches(buffer) && buffer.Length != input.Length)
                {
                    buffer.Append(input[buffer.Length]);
                }

                buffer.Remove(buffer.Length - 1, 1);//?

                List<Terminal> terminals = LookupTerminals(buffer);

                return new Token(GetPrioritizedTerminal(terminals), buffer.ToString());
            }
            else
            {
                throw new Exception("Unexpected symbol " + buffer);
            }
        }

        private Terminal GetPrioritizedTerminal(List<Terminal> terminals)
        {
            Terminal prioritizedTerminal = terminals[0];

            foreach (Terminal terminal in terminals)
            {
                if (terminal.Priority > prioritizedTerminal.Priority)
                {
                    prioritizedTerminal = terminal;
                }
            }

            return prioritizedTerminal;
        }

        private bool AnyTerminalMatches(StringBuilder buffer)
        {
            return LookupTerminals(buffer).Count != 0;
        }

        private List<Terminal> LookupTerminals(StringBuilder buffer)
        {
            List<Terminal> terminals = new List<Terminal>();

            foreach (Terminal terminal in _terminals)
            {
                if (terminal.Matches(buffer.ToString()))
                {
                    terminals.Add(terminal);
                }
            }

            return terminals;
        }

        //private string LookupInput(string[] args)
        //{
        //    if (args.Length == 0)
        //    {
        //        throw new ArgumentException("Input string not found");
        //    }

        //    return args[0];
        //}

        private void Print(List<Token> tokenes)
        {
            foreach (Token token in tokenes)
            {
                //Console.WriteLine("[%s, %s]%n", token.Terminal.Identifier, token.Value);
                Console.WriteLine($"[{token.Terminal.Identifier}], {token.Value}");
            }
        }
    }
}