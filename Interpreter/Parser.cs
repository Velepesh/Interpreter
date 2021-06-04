using System;
using System.Collections.Generic;

namespace Interpreter
{
    class Parser
    {
        private List<Token> _tokens = new List<Token>();
        private int _position = 0;
        private TokenType _currentTokenType => Peek();

        public AstNode AstNode { get; private set; }

        public Parser(List<Token> tokens)
        { 
            _tokens = tokens;
        }

        public void Analysis()
        {
            AstNode = new AstNode(NodeType.language);

            while (_tokens.Count > _position)
            {
                try
                {
                    Language(AstNode);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Синтаксическая ошибка: " + _position + " " + _currentTokenType);
                    Console.WriteLine(e.StackTrace);
                    break;
                }
            }
        }

        private void Language(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.lang_expr);

            astTop.AddNode(astNode);

            if (_currentTokenType == TokenType.VAR)
            {
                if (NextTokenType() == TokenType.DOT)
                {
                    FuncExpr(astNode);
                }
                else
                {
                    AssignExpr(astNode);
                }
            }
            else if (_currentTokenType == TokenType.WHILE)
            {
                WhileExpr(astNode);
            }
            else if (_currentTokenType == TokenType.IF)
            {
                IfExpr(astNode);
            }
            else if (_currentTokenType == TokenType.FOR)
            {
                ForExpr(astNode);
            }
            else if (_currentTokenType == TokenType.DO)
            {
                DoWhileExpr(astNode);
            }
            else if (_currentTokenType == TokenType.PRINT)
            {
                PrintExpr(astNode);
            }
            else if (_currentTokenType == TokenType.LINKED_LIST)
            {
                LinkedListExpr(astNode);
            }
            else if (_currentTokenType == TokenType.HASH_SET)
            {
                HashSetExpr(astNode);
            }
            else
            {
                throw new Exception();
            }
        }

        private void AssignExpr(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.assign_expr);
            astTop.AddNode(astNode);

            astNode.AddLeaf(Check(TokenType.VAR));
            astNode.AddLeaf(Check(TokenType.ASSIGN));

            Expression(astNode);

            astNode.AddLeaf(Check(TokenType.SEMICOLON));
        }

        private void FuncExpr(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.func_expr);
            astTop.AddNode(astNext);

            if (_currentTokenType == TokenType.VAR)
            {
                astNext.AddLeaf(Check(TokenType.VAR));
                astNext.AddLeaf(Check(TokenType.DOT));
                astNext.AddLeaf(new Token(TokenType.FUNC, Check(TokenType.VAR).Value));
                astNext.AddLeaf(Check(TokenType.LEFT_PAREN));

                if (_currentTokenType != TokenType.RIGHT_PAREN)
                {
                    Expression(astNext);

                    while (_currentTokenType == TokenType.COMMA)
                    {
                        astNext.AddLeaf(Check(TokenType.COMMA));
                        Expression(astNext);
                    }
                }
                astNext.AddLeaf(Check(TokenType.RIGHT_PAREN));
                astNext.AddLeaf(Check(TokenType.SEMICOLON));
            }
            else
            {
                throw new Exception();
            }
        }  

        private void ForExpr(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.for_expr);

            astTop.AddNode(astNext);

            astNext.AddLeaf(Check(TokenType.FOR));
            astNext.AddLeaf(Check(TokenType.LEFT_PAREN));
            AssignExprFor(astNext);
            astNext.AddLeaf(Check(TokenType.SEMICOLON));
            Expression(astNext);
            astNext.AddLeaf(Check(TokenType.SEMICOLON));
            AssignExprFor(astNext);
            astNext.AddLeaf(Check(TokenType.RIGHT_PAREN));
            Block(astNext);
        }

        private void AssignExprFor(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.assign_expr);
            astTop.AddNode(astNext);

            astNext.AddLeaf(Check(TokenType.VAR));
            astNext.AddLeaf(Check(TokenType.ASSIGN));
            Expression(astNext);
        }

        private void DoWhileExpr(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.do_while_expr);
            astTop.AddNode(astNext);

            astNext.AddLeaf(Check(TokenType.DO));
            Block(astNext);
            astNext.AddLeaf(Check(TokenType.WHILE));
            astNext.AddLeaf(Check(TokenType.LEFT_PAREN));
            Expression(astNext);
            astNext.AddLeaf(Check(TokenType.RIGHT_PAREN));
            astNext.AddLeaf(Check(TokenType.SEMICOLON));
        }

        private void PrintExpr(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.print_expr);
            astTop.AddNode(astNext);

            astNext.AddLeaf(Check(TokenType.PRINT));
            Expression(astNext);
            astNext.AddLeaf(Check(TokenType.SEMICOLON));
        }

        private void LinkedListExpr(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.lincked_list_expr);
            astTop.AddNode(astNext);

            astNext.AddLeaf(Check(TokenType.LINKED_LIST));
            astNext.AddLeaf(Check(TokenType.VAR));
            astNext.AddLeaf(Check(TokenType.SEMICOLON));
        }

        private void HashSetExpr(AstNode astTop)
        {
            AstNode astNext = new AstNode(NodeType.hash_set_expr);
            astTop.AddNode(astNext);

            astNext.AddLeaf(Check(TokenType.HASH_SET));
            astNext.AddLeaf(Check(TokenType.VAR));
            astNext.AddLeaf(Check(TokenType.SEMICOLON));
        }

        private void IfExpr(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.if_expr);
            astTop.AddNode(astNode);

            astNode.AddLeaf(Check(TokenType.IF));
            astNode.AddLeaf(Check(TokenType.LEFT_PAREN));

            Expression(astNode);

            astNode.AddLeaf(Check(TokenType.RIGHT_PAREN));

            Block(astNode);

            while (_currentTokenType == TokenType.ELIF)
                ElifExpr(astNode);

            if (_currentTokenType == TokenType.ELSE)
                ElseExpr(astNode);
        }

        private void ElifExpr(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.elif_expr);
            astTop.AddNode(astNode);

            astNode.AddLeaf(Check(TokenType.ELIF));
            astNode.AddLeaf(Check(TokenType.LEFT_PAREN));

            Expression(astNode);

            astNode.AddLeaf(Check(TokenType.RIGHT_PAREN));

            Block(astNode);
        }

        private void ElseExpr(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.else_expr);

            astTop.AddNode(astNode);
            astNode.AddLeaf(Check(TokenType.ELSE));

            Block(astNode);
        }

        private void WhileExpr(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.while_expr);

            astTop.AddNode(astNode);

            astNode.AddLeaf(Check(TokenType.WHILE));
            astNode.AddLeaf(Check(TokenType.LEFT_PAREN));

            Expression(astNode);

            astNode.AddLeaf(Check(TokenType.RIGHT_PAREN));

            Block(astNode);
        }

        private void Block(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.block);

            astTop.AddNode(astNode);
            
            if (_currentTokenType == TokenType.LEFT_BRACE)
            {
                astNode.AddLeaf(Check(TokenType.LEFT_BRACE));
            
                while (_currentTokenType != TokenType.RIGTH_BRACE)
                    Language(astNode);
            
                astNode.AddLeaf(Check(TokenType.RIGTH_BRACE));
            }
            else
            {
                Language(astNode);
            }
        }

        private bool IsOperator(TokenType type)
        {
            return type == TokenType.PLUS || type == TokenType.MINUS 
                || type == TokenType.DIV || type == TokenType.MULT 
                || type == TokenType.EQUAL || type == TokenType.LESS 
                || type == TokenType.LESS_EQUAL || type == TokenType.GREATER 
                || type == TokenType.GREATER_EQUAL || type == TokenType.NOT_EQUAL;
        }       

        private void Expression(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.expression);

            astTop.AddNode(astNode);
        
            if (_currentTokenType == TokenType.NUMBER || _currentTokenType == TokenType.VAR)
            {
                if (_currentTokenType == TokenType.VAR && NextTokenType() == TokenType.DOT)
                {
                    MemberExpr(astNode);
                }
                else
                {
                    Member(astNode);
                }
            }
            else if (_currentTokenType == TokenType.LEFT_PAREN)
            {      
                BracketMember(astNode);
            }
            else
            {
                throw new Exception();
            }
            
            while (IsOperator(_currentTokenType))
            { 
                Operator(astNode);
                Expression(astNode);
            }
        }

        private void Operator(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.op);
            astTop.AddNode(astNode);

            if (IsOperator(_currentTokenType))
            {
                astNode.AddLeaf(GetNextToken());
            }
            else
            {
                throw new Exception();
            }
        }

        private void BracketMember(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.bracket_member);
            astTop.AddNode(astNode);

            astNode.AddLeaf(Check(TokenType.LEFT_PAREN));
            Expression(astNode);
            astNode.AddLeaf(Check(TokenType.RIGHT_PAREN));
        }

        private void Member(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.member);
            astTop.AddNode(astNode);

            if (_currentTokenType == TokenType.NUMBER || _currentTokenType == TokenType.VAR)
            {
                astNode.AddLeaf(GetNextToken());
            }
            else
                throw new Exception();
        }

        private void MemberExpr(AstNode astTop) 
        {
            AstNode astNext = new AstNode(NodeType.member_expr);
            astTop.AddNode(astNext);

            if (_currentTokenType == TokenType.VAR)
            {
                astNext.AddLeaf(Check(TokenType.VAR));
                astNext.AddLeaf(Check(TokenType.DOT));
                astNext.AddLeaf(new Token(TokenType.FUNC, Check(TokenType.VAR).Value));
                astNext.AddLeaf(Check(TokenType.LEFT_PAREN));

                if (_currentTokenType != TokenType.RIGHT_PAREN)
                {
                    Expression(astNext);

                    while (_currentTokenType == TokenType.COMMA)
                    {
                        astNext.AddLeaf(Check(TokenType.COMMA));
                        Expression(astNext);
                    }
                }

                astNext.AddLeaf(Check(TokenType.RIGHT_PAREN));
            }
            else 
            {
                throw new Exception();
            }
        }

        private Token Check(TokenType tokenType)
        {
            if (_currentTokenType != tokenType)
            {
                throw new Exception("Unexpected: " + tokenType + " currentTokenType " + _currentTokenType);
            }

            return _tokens[_position++];
        }
        
        private Token GetNextToken()
        {
            return _tokens[_position++];
        }

        private TokenType NextTokenType()
        {
            return _tokens[_position + 1].TokenType;
        }

        private TokenType Peek()
        {
            if (_position >= _tokens.Count)
                return _tokens[_tokens.Count - 1].TokenType;

            return _tokens[_position].TokenType;
        }
    }
}