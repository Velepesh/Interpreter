using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Parser
    {
        private List<Token> _tokens = new List<Token>();
        private int _position;

        public AstNode AstNode { get; private set; }

        public Parser(List<Token> tokens)
        { 
            _tokens = tokens;
            _position = 0;
        }

        public void analysis()
        {
            //long time_analysis = System.nanoTime();

            AstNode = new AstNode(NodeType.Language);

            //while (currentTokenType() != TokenType.END)
            while (currentTokenType() != TokenType.END)
            {
                try
                {
                    languageConst(AstNode);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Синтаксическая ошибка: " + _position + " " + currentTokenType());
                    //e.printStackTrace();
                    //e.StackTrace;
                    Console.WriteLine(e.StackTrace);
                    break;
                }
            }

           // Console.WriteLine("[Parser] time analysis: " + (System.nanoTime() - time_analysis) / 1_000_000.0 + "ms");
        }

        private void languageConst(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.LanguageConst);
            astTop.addNode(astNext);

            if (currentTokenType() == TokenType.VAR) 
            {
                assignConst(astNext);
            } 
            else if (currentTokenType() == TokenType.WHILE) 
            {
                whileConst(astNext);
            }
            else if (currentTokenType() == TokenType.IF)
            {
                ifConst(astNext);
            }
            else
            {
                throw new Exception();
            }
        }

        private void ifConst(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.IfConst);
            astTop.addNode(astNext);

            astNext.addLeaf(LexCheck(TokenType.IF));
            astNext.addLeaf(LexCheck(TokenType.LEFT_PAREN));
            expression(astNext);
            astNext.addLeaf(LexCheck(TokenType.RIGHT_PAREN));
            block(astNext);

            while (currentTokenType() == TokenType.ELIF)
                elifConst(astNext);

            if (currentTokenType() == TokenType.ELSE)
                elseConst(astNext);
        }

        private void elifConst(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.ElifConst);
            astTop.addNode(astNext);

            astNext.addLeaf(LexCheck(TokenType.ELIF));
            astNext.addLeaf(LexCheck(TokenType.LEFT_PAREN));
            expression(astNext);
            astNext.addLeaf(LexCheck(TokenType.RIGHT_PAREN));
            block(astNext);
        }

        private void elseConst(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.ElseConst);
            astTop.addNode(astNext);

            astNext.addLeaf(LexCheck(TokenType.ELSE));
            block(astNext);
        }

        private void whileConst(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.WhileConst);
            astTop.addNode(astNext);

            astNext.addLeaf(LexCheck(TokenType.WHILE));
            astNext.addLeaf(LexCheck(TokenType.LEFT_PAREN));
            expression(astNext);
            astNext.addLeaf(LexCheck(TokenType.RIGHT_PAREN));
            block(astNext);
        }

        private void block(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.Block);
            astTop.addNode(astNext);
            
            if (currentTokenType() == TokenType.LEFT_BRACE)
            {
                astNext.addLeaf(LexCheck(TokenType.LEFT_BRACE));
            
                while (currentTokenType() != TokenType.RIGTH_BRACE)
                    languageConst(astNext);
            
                astNext.addLeaf(LexCheck(TokenType.RIGTH_BRACE));
            }
            else
            {
                languageConst(astNext);
            }
        }

        private void assignConst(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.AssignConst);
            astTop.addNode(astNext);
            
            astNext.addLeaf(LexCheck(TokenType.VAR));
            astNext.addLeaf(LexCheck(TokenType.ASSIGN));
            expression(astNext);
            astNext.addLeaf(LexCheck(TokenType.SEMICOLON));
        }

        private bool isOp(TokenType type)
        {
            return type == TokenType.PLUS || type == TokenType.MINUS 
                || type == TokenType.DIV || type == TokenType.MULT 
                || type == TokenType.EQUAL || type == TokenType.LESS 
                || type == TokenType.LESS_EQUAL || type == TokenType.GREATER 
                || type == TokenType.GREATER_EQUAL || type == TokenType.NOT_EQUAL;
        }       

        private void expression(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.Expression);
            astTop.addNode(astNext);
        
            if (currentTokenType() == TokenType.NUMBER || currentTokenType() == TokenType.VAR)
            {
            
                member(astNext);
            }
            else if (currentTokenType() == TokenType.LEFT_PAREN)
            {
            
                memberBracket(astNext);
            }
            else
            {
                throw new Exception();
            }
            
            while (isOp(currentTokenType()))
            {
            
                op(astNext);
                expression(astNext);
            }
        }

        private void op(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.Op);
            astTop.addNode(astNext);

            if (isOp(currentTokenType()))
                astNext.addLeaf(getNextToken());
            else
                throw new Exception();
        }

        private void memberBracket(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.BracketMember);
            astTop.addNode(astNext);

            astNext.addLeaf(LexCheck(TokenType.LEFT_PAREN));
            expression(astNext);
            astNext.addLeaf(LexCheck(TokenType.RIGHT_PAREN));
        }

        private void member(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.Member);
            astTop.addNode(astNext);
        
            if (currentTokenType() == TokenType.NUMBER || currentTokenType() == TokenType.VAR)
                astNext.addLeaf(getNextToken());
            else
                throw new Exception();
        }

        private Token LexCheck(TokenType type)
        {
            if (currentTokenType() != type)
                throw new Exception();
            else
                return _tokens[_position++];
        }
        
        private Token getNextToken()
        {
            return _tokens[_position++];
        }

        private TokenType currentTokenType()
        {
            return _tokens[_position].Terminal.TokenType;
        }

        public AstNode getTree()
        {
            return AstNode;
        }
    }
}