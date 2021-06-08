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

        public void Parse()
        {
            AstNode = new AstNode(NodeType.language);

            while (_tokens.Count > _position)
            {
                try
                {
                    ParseLanguage(AstNode);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Syntax error: " + _position + " " + _currentTokenType);
                    Console.WriteLine(e.StackTrace);
                    break;
                }
            }

            AstNode.ShowAstTree();
        }

        private void ParseLanguage(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.lang_expr);

            astTop.AddNode(node);

            if (_currentTokenType == TokenType.VAR)
            {
                if (GetNextTokenType() == TokenType.DOT)
                {
                    ParseMethodExpr(node);
                }
                else
                {
                    ParseAssignExpr(node);
                }
            }
            else if (_currentTokenType == TokenType.WHILE)
            {
                ParseWhileExpr(node);
            }
            else if (_currentTokenType == TokenType.IF)
            {
                ParseIfExpr(node);
            }
            else if (_currentTokenType == TokenType.FOR)
            {
                ParseForExpr(node);
            }
            else if (_currentTokenType == TokenType.DO)
            {
                ParseDoWhileExpr(node);
            }
            else if (_currentTokenType == TokenType.PRINT)
            {
                ParsePrintExpr(node);
            }
            else if (_currentTokenType == TokenType.LINKED_LIST)
            {
                ParseLinkedListExpr(node);
            }
            else if (_currentTokenType == TokenType.HASH_SET)
            {
                ParseHashSetExpr(node);
            }
            else
            {
                throw new Exception("Unexpected token: " + _currentTokenType);
            }
        }

        private void ParseAssignExpr(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.assign_expr);
            astTop.AddNode(node);

            node.AddLeaf(Check(TokenType.VAR));
            node.AddLeaf(Check(TokenType.ASSIGN));

            ParseValue(node);

            node.AddLeaf(Check(TokenType.SEMICOLON));
        }

        private void ParseForExpr(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.for_expr);
            astTop.AddNode(node);

            node.AddLeaf(Check(TokenType.FOR));
            node.AddLeaf(Check(TokenType.LEFT_PAREN));
            ParseAssignExprFor(node);
            node.AddLeaf(Check(TokenType.SEMICOLON));

            ParseValue(node);
            node.AddLeaf(Check(TokenType.SEMICOLON));

            ParseAssignExprFor(node);
            node.AddLeaf(Check(TokenType.RIGHT_PAREN));

            ParseBody(node);
        }

        private void ParseAssignExprFor(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.assign_expr);
            astTop.AddNode(node);

            node.AddLeaf(Check(TokenType.VAR));
            node.AddLeaf(Check(TokenType.ASSIGN));
            ParseValue(node);
        }

        private void ParseDoWhileExpr(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.do_while_expr);
            astTop.AddNode(node);

            node.AddLeaf(Check(TokenType.DO));
            ParseBody(node);
            node.AddLeaf(Check(TokenType.WHILE));
            node.AddLeaf(Check(TokenType.LEFT_PAREN));
            ParseValue(node);
            node.AddLeaf(Check(TokenType.RIGHT_PAREN));
            node.AddLeaf(Check(TokenType.SEMICOLON));
        }

        private void ParsePrintExpr(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.print_expr);
            astTop.AddNode(node);

            node.AddLeaf(Check(TokenType.PRINT));
            ParseValue(node);
            node.AddLeaf(Check(TokenType.SEMICOLON));
        }

        private void ParseLinkedListExpr(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.lincked_list_expr);
            astTop.AddNode(node);

            node.AddLeaf(Check(TokenType.LINKED_LIST));
            node.AddLeaf(Check(TokenType.VAR));
            node.AddLeaf(Check(TokenType.SEMICOLON));
        }

        private void ParseHashSetExpr(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.hash_set_expr);
            astTop.AddNode(node);

            node.AddLeaf(Check(TokenType.HASH_SET));
            node.AddLeaf(Check(TokenType.VAR));
            node.AddLeaf(Check(TokenType.SEMICOLON));
        }

        private void ParseIfExpr(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.if_expr);
            astTop.AddNode(node);

            node.AddLeaf(Check(TokenType.IF));
            node.AddLeaf(Check(TokenType.LEFT_PAREN));

            ParseValue(node);

            node.AddLeaf(Check(TokenType.RIGHT_PAREN));

            ParseBody(node);

            while (_currentTokenType == TokenType.ELIF)
                ParseElifExpr(node);

            if (_currentTokenType == TokenType.ELSE)
                ParseElseExpr(node);
        }

        private void ParseElifExpr(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.elif_expr);
            astTop.AddNode(node);

            node.AddLeaf(Check(TokenType.ELIF));
            node.AddLeaf(Check(TokenType.LEFT_PAREN));

            ParseValue(node);

            node.AddLeaf(Check(TokenType.RIGHT_PAREN));

            ParseBody(node);
        }

        private void ParseElseExpr(AstNode astTop)
        {
            AstNode astNode = new AstNode(NodeType.else_expr);
            astTop.AddNode(astNode);

            astNode.AddLeaf(Check(TokenType.ELSE));

            ParseBody(astNode);
        }

        private void ParseWhileExpr(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.while_expr);
            astTop.AddNode(node);

            node.AddLeaf(Check(TokenType.WHILE));
            node.AddLeaf(Check(TokenType.LEFT_PAREN));

            ParseValue(node);

            node.AddLeaf(Check(TokenType.RIGHT_PAREN));

            ParseBody(node);
        }

        private void ParseBody(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.body);

            astTop.AddNode(node);
            
            if (_currentTokenType == TokenType.LEFT_BRACE)
            {
                node.AddLeaf(Check(TokenType.LEFT_BRACE));

                while (_currentTokenType != TokenType.RIGTH_BRACE)
                {
                    ParseLanguage(node);
                }
            
                node.AddLeaf(Check(TokenType.RIGTH_BRACE));
            }
            else
            {
                ParseLanguage(node);
            }
        }

        private void ParseValue(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.value);
            astTop.AddNode(node);
        
            if (_currentTokenType == TokenType.NUMBER || _currentTokenType == TokenType.VAR)
            {
                if (_currentTokenType == TokenType.VAR && GetNextTokenType() == TokenType.DOT)
                    ParseMethodExpr(node);
                else
                    ParseMember(node);
            }
            else if (_currentTokenType == TokenType.LEFT_PAREN)
            {      
                ParseBracketMember(node);
            }
            else
            {
                throw new Exception("Unexpected token type:" + _currentTokenType);
            }
            
            while (IsOperator(_currentTokenType))
            { 
                ParseOperator(node);
                ParseValue(node);
            }
        }
        private void ParseOperator(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.op);
            astTop.AddNode(node);

            node.AddLeaf(Check(_currentTokenType));
        }

        private void ParseMember(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.member);
            astTop.AddNode(node);

            node.AddLeaf(Check(_currentTokenType));
        }

        private void ParseBracketMember(AstNode astTop)
        {
            AstNode node = new AstNode(NodeType.bracket_member);
            astTop.AddNode(node);

            node.AddLeaf(Check(TokenType.LEFT_PAREN));
            ParseValue(node);
            node.AddLeaf(Check(TokenType.RIGHT_PAREN));
        }

        private void ParseMethodExpr(AstNode astTop) 
        {
            AstNode node = new AstNode(NodeType.method);
            astTop.AddNode(node);

            if (_currentTokenType == TokenType.VAR)
            {
                node.AddLeaf(Check(TokenType.VAR));
                node.AddLeaf(Check(TokenType.DOT));
                node.AddLeaf(new Token(TokenType.METHOD, Check(TokenType.VAR).Value));
                node.AddLeaf(Check(TokenType.LEFT_PAREN));

                if (_currentTokenType != TokenType.RIGHT_PAREN)
                {
                    ParseValue(node);

                    while (_currentTokenType == TokenType.COMMA)
                    {
                        node.AddLeaf(Check(TokenType.COMMA));
                        ParseValue(node);
                    }
                }

                node.AddLeaf(Check(TokenType.RIGHT_PAREN));

                if(_currentTokenType == TokenType.SEMICOLON)
                {
                    node.AddLeaf(Check(TokenType.SEMICOLON));
                }
            }
            else 
            {
                throw new Exception("Unexpected token type:" + _currentTokenType);
            }
        }

        private bool IsOperator(TokenType tokenType)
        {
            return tokenType == TokenType.PLUS || tokenType == TokenType.MINUS 
                || tokenType == TokenType.MULT || tokenType == TokenType.DIV 
                || tokenType == TokenType.EQUAL || tokenType == TokenType.NOT_EQUAL
                || tokenType == TokenType.LESS || tokenType == TokenType.LESS_EQUAL 
                || tokenType == TokenType.GREATER || tokenType == TokenType.GREATER_EQUAL 
                || tokenType == TokenType.NOT_EQUAL;
        }       

        private Token Check(TokenType tokenType)
        {
            if (_currentTokenType != tokenType)
            {
                throw new Exception("Unexpected: " + tokenType + ", Current token type: " + _currentTokenType);
            }

            return _tokens[_position++];
        }

        private TokenType GetNextTokenType()
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