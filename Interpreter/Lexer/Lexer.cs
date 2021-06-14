using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Lexer
    {
        private List<Terminal> _terminals = new List<Terminal> 
        {
            new Terminal(TokenType.PLUS, "^\\+$"),
            new Terminal(TokenType.MINUS, "^\\-$"),
            new Terminal(TokenType.MULT, "^\\*$"),
            new Terminal(TokenType.DIV, "^\\/$"),
            new Terminal(TokenType.WHILE, "^while$", 1),
            new Terminal(TokenType.FOR, "^for$", 1),
            new Terminal(TokenType.ELIF, "^elif$", 1),
            new Terminal(TokenType.IF, "^if$", 1),
            new Terminal(TokenType.ELSE, "^else$", 1),
            new Terminal(TokenType.DO, "^do$", 1),
            new Terminal(TokenType.PRINT, "^Print$"),
            new Terminal(TokenType.LINKED_LIST, "^LinkedList$"),
            new Terminal(TokenType.HASH_SET, "^HashSet$"),
            new Terminal(TokenType.NUMBER, "^0$|^[1-9][0-9]*$"),
            new Terminal(TokenType.VAR, "^[a-zA-Z0-9_]*$"),
            new Terminal(TokenType.WHITESPACE, "^\\s+$"),
            new Terminal(TokenType.SEMICOLON, "^;$"),
            new Terminal(TokenType.ASSIGN, "^=$"),
            new Terminal(TokenType.EQUAL, "^==$"),
            new Terminal(TokenType.NOT_EQUAL, "^<>$"),
            new Terminal(TokenType.GREATER, "^>$"),
            new Terminal(TokenType.LESS, "^<$"),
            new Terminal(TokenType.GREATER_EQUAL,">="),
            new Terminal(TokenType.LESS_EQUAL,"<="),
            new Terminal(TokenType.LEFT_PAREN, "^\\($"),
            new Terminal(TokenType.RIGHT_PAREN, "^\\)$"),
            new Terminal(TokenType.LEFT_BRACE, "^\\{$"),
            new Terminal(TokenType.RIGTH_BRACE, "^\\}$"),
            new Terminal(TokenType.COMMA, "^,$"),
            new Terminal(TokenType.DOT, "^.$"),
        };

        private List<Token> _tokens = new List<Token>();

        public void RunLexer(string expression)
        {
            StringBuilder input = new StringBuilder(expression);

            input.Append("$");

            while (input[0] != '$')
            {
                Token token = ExtractNextToken(input);

                if (token.TokenType != TokenType.WHITESPACE)
                    _tokens.Add(token);

                input.Remove(0, token.Value.Length);
            }

            ShowTokens(_tokens);
        }

        public List<Token> GetTokens()
        {
            return _tokens;
        }

        private Token ExtractNextToken(StringBuilder input)
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append(input[0]);

            if (AnyTerminalMatches(buffer))
            {
                while (AnyTerminalMatches(buffer) && buffer.Length != input.Length)
                {
                    buffer.Append(input[buffer.Length]);
                }

                buffer.Remove(buffer.Length - 1, 1);

                List<Terminal> terminals = LookupTerminals(buffer);

                return new Token(GetPrioritizedTerminal(terminals).TokenType, buffer.ToString());
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
                if (terminal.IsMatches(buffer.ToString()))
                {
                    terminals.Add(terminal);
                }
            }

            return terminals;
        }

        private void ShowTokens(List<Token> tokens)
        {
            Console.WriteLine("Lexer\n");

            foreach (Token token in tokens)
            {
                Console.WriteLine($"[{token.TokenType}], {token.Value}");
            }
        }
    }
}