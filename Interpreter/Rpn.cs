﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Rpn
    {
        private AstNode _astNode;
        private LinkedList<Token> _tokens = new LinkedList<Token>();
        private List<Token> _rpn = new List<Token>();
        private HashSet<string> _variables = new HashSet<string>();
        private HashSet<string> _variablesList = new HashSet<string>();
        private HashSet<string> _variablesSet = new HashSet<string>();

        public Rpn(AstNode astNode)
        {
            _astNode = astNode;
        }

        public void Translate()
        {
            foreach (AstNode node in _astNode.GetNodes())
            {
                Language(node);
            }
        }

        private void Language(AstNode node)
        {
            AstNode nodeExpr = node.GetNodes()[0];

            switch (nodeExpr.Type)
            {
                case NodeType.assign_expr:
                    AssignExpr(nodeExpr);
                    break;
                case NodeType.if_expr:
                    IfExpr(nodeExpr);
                    break;
                case NodeType.while_expr:
                    WhileExpr(nodeExpr);
                    break;
                case NodeType.do_while_expr:
                    DoWhileExpr(nodeExpr);
                    break;
                case NodeType.for_expr:
                    ForExpr(nodeExpr);
                    break;
                case NodeType.func_expr:
                    FuncExpr(nodeExpr);
                    break;
                case NodeType.print_expr:
                    PrintExpr(nodeExpr);
                    break;
                case NodeType.lincked_list_expr:
                    LinkedListExpr(nodeExpr);
                    break;
                case NodeType.hash_set_expr:
                    HashSetExpr(nodeExpr);
                    break;
                default:
                    throw new Exception();
            }
        }

        private void WhileExpr(AstNode nodeConst)
        {
            int start = _rpn.Count;

            Expression(nodeConst.GetNodes()[0]);
            FlushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            AddOprand(point);

            AddOprand(nodeConst.GetLeafs()[0]);
            Block(nodeConst.GetNodes()[1]);

            Token endPoint = new Token(TokenType.JMP_VALUE);
            AddOprand(endPoint);
            AddOprand(new Token(TokenType.JMP));

            point.SetValue(Convert.ToString(_rpn.Count));
            endPoint.SetValue(Convert.ToString(start));
        }

        private void IfExpr(AstNode node)
        {
            Expression(node.GetNodes()[0]);
            FlushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            AddOprand(point);

            AddOprand(node.GetLeafs()[0]);
            Block(node.GetNodes()[1]);

            Token endPoint = new Token(TokenType.JMP_VALUE);
            AddOprand(endPoint);
            AddOprand(new Token(TokenType.JMP));

            point.SetValue(Convert.ToString(_rpn.Count));

            for (int i = 2; i < node.GetNodes().Count; i++)
            {
                if (node.GetNodes()[i].Type == NodeType.elif_expr)
                {
                    ElifExpr(node.GetNodes()[i], endPoint);
                }
                else if (node.GetNodes()[i].Type == NodeType.else_expr)
                {
                    ElseExpr(node.GetNodes()[i]);
                }
            }

            endPoint.SetValue(Convert.ToString(_rpn.Count));
        }

        private void ElifExpr(AstNode nodeConst, Token endPoint)
        {
            Expression(nodeConst.GetNodes()[0]);
            FlushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            AddOprand(point);

            AddOprand(nodeConst.GetLeafs()[0]);
            Block(nodeConst.GetNodes()[1]);

            AddOprand(endPoint);
            AddOprand(new Token(TokenType.JMP));

            point.SetValue(Convert.ToString(_rpn.Count));
        }

        private void ElseExpr(AstNode nodeConst)
        {
            Block(nodeConst.GetNodes()[0]);
        }

        private void DoWhileExpr(AstNode nodeConst)
        {
            int start = _rpn.Count;

            Block(nodeConst.GetNodes()[0]);

            Expression(nodeConst.GetNodes()[1]);
            FlushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            AddOprand(point);

            AddOprand(nodeConst.GetLeafs()[0]);

            point.SetValue(Convert.ToString(start));
        }

        private void ForExpr(AstNode nodeConst)
        {
            AssignExpr(nodeConst.GetNodes()[0]);

            int start = _rpn.Count;

            Expression(nodeConst.GetNodes()[1]);
            FlushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            AddOprand(point);
            AddOprand(nodeConst.GetLeafs()[0]);

            Block(nodeConst.GetNodes()[3]);
            AssignExpr(nodeConst.GetNodes()[2]);

            Token endPoint = new Token(TokenType.JMP_VALUE);
            AddOprand(endPoint);
            AddOprand(new Token(TokenType.JMP));

            point.SetValue(Convert.ToString(_rpn.Count));
            endPoint.SetValue(Convert.ToString(start));
        }

        private void PrintExpr(AstNode nodeConst)
        {
            Expression(nodeConst.GetNodes()[0]);
            FlushTexas();
            AddOprand(nodeConst.GetLeafs()[0]);
        }

        private void FuncExpr(AstNode node)
        {
            if (!_variables.Contains(node.GetLeafs()[0].Value) 
                && (!_variablesList.Contains(node.GetLeafs()[0].Value) 
                || !_variablesSet.Contains(node.GetLeafs()[0].Value)))
            {
                throw new Exception();
            }

            if (_variablesList.Contains(node.GetLeafs()[0].Value))
            {
                if (node.GetLeafs()[2].Value.Equals("Add"))
                {
                    if (node.GetNodes().Count == 2)
                    {
                        Expression(node.GetNodes()[0]);
                        Expression(node.GetNodes()[1]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (node.GetLeafs()[2].Value.Equals("Size"))
                {
                    if (node.GetNodes().Count == 0)
                    {
                        Expression(node.GetNodes()[0]);
                        Expression(node.GetNodes()[1]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (node.GetLeafs()[2].Value.Equals("PrintList"))
                {
                    if (node.GetNodes().Count != 0)
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (_variablesSet.Contains(node.GetLeafs()[0].Value))
            {
                if (node.GetLeafs()[2].Value.Equals("Add"))
                {
                    if (node.GetNodes().Count == 1)
                    {
                        Expression(node.GetNodes()[0]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (node.GetLeafs()[2].Value.Equals("Remove"))
                {
                    if (node.GetNodes().Count == 1)
                    {
                        Expression(node.GetNodes()[0]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (node.GetLeafs()[2].Value.Equals("PrintSet"))
                {
                    if (node.GetNodes().Count != 0)
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }

            AddOprand(node.GetLeafs()[0]);
            AddOprand(node.GetLeafs()[2]);
        }

        private void LinkedListExpr(AstNode nodeConst)
        {
            AddOprand(nodeConst.GetLeafs()[1]);
            AddOprand(nodeConst.GetLeafs()[0]);

            _variables.Add(nodeConst.GetLeafs()[1].Value);
            _variablesList.Add(nodeConst.GetLeafs()[1].Value);
        }

        private void HashSetExpr(AstNode nodeConst)
        {
            AddOprand(nodeConst.GetLeafs()[1]);
            AddOprand(nodeConst.GetLeafs()[0]);

            _variables.Add(nodeConst.GetLeafs()[1].Value);
            _variablesSet.Add(nodeConst.GetLeafs()[1].Value);
        }

        private void Block(AstNode topNode)
        {
            foreach (AstNode node in topNode.GetNodes())
            {
                Language(node);
            }
        }

        private void AssignExpr(AstNode nodeConst)
        {
            AddOprand(nodeConst.GetLeafs()[0]);
            AddOperator(nodeConst.GetLeafs()[1]);
            Expression(nodeConst.GetNodes()[0]);
            FlushTexas();

            _variables.Add(nodeConst.GetLeafs()[0].Value);
        }

        private void Expression(AstNode topNode)
        {
            List<AstNode> nextNodes = topNode.GetNodes();

            if (nextNodes[0].Type == NodeType.member)
            {
                Member(nextNodes[0]);
            }
            else if (nextNodes[0].Type == NodeType.bracket_member)
            {
                BracketMember(nextNodes[0]);
            }
            else if (nextNodes[0].Type == NodeType.member_expr)
            {
                MemberExpr(nextNodes[0]);
            }

            if (nextNodes.Count > 1)
            {
                Op(nextNodes[1]);
                Expression(nextNodes[2]);
            }
        }

        private void MemberExpr(AstNode node)
        {
            if(!_variables.Contains(node.GetLeafs()[0].Value) && 
                (!_variablesList.Contains(node.GetLeafs()[0].Value) || !_variablesSet.Contains(node.GetLeafs()[0].Value))) {

                throw new Exception();
            }

            if (_variablesList.Contains(node.GetLeafs()[0].Value))
            {
                if (node.GetLeafs()[2].Value.Equals("Get"))
                {
                    if (node.GetNodes().Count == 1)
                    {
                        Expression(node.GetNodes()[0]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (node.GetLeafs()[2].Value.Equals("Size"))
                {
                    if (node.GetNodes().Count == 1)
                    {
                        Expression(node.GetNodes()[0]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (node.GetLeafs()[2].Value.Equals("Contains"))
                {
                    if (node.GetNodes().Count == 1)
                    {
                        Expression(node.GetNodes()[0]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (_variablesSet.Contains(node.GetLeafs()[0].Value))
            {
                if (node.GetLeafs()[2].Value.Equals("Contains"))
                {

                    if (node.GetNodes().Count == 1)
                    {
                        Expression(node.GetNodes()[0]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }

            AddOprand(node.GetLeafs()[0]);
            AddOprand(node.GetLeafs()[2]);
        }

        private void BracketMember(AstNode node)
        {
            AddOperator(node.GetLeafs()[0]);
            Expression(node.GetNodes()[0]);
            AddOperator(node.GetLeafs()[1]);
        }

        private void Op(AstNode node)
        {
            AddOperator(node.GetLeafs()[0]);
        }

        private void Member(AstNode node)
        {
            AddOprand(node.GetLeafs()[0]);
        }

        private int OpPriority(Token op)
        {
            int priority;

            if (op.TokenType == TokenType.ASSIGN)
                priority = 0;
            else if (op.TokenType == TokenType.LESS || op.TokenType == TokenType.GREATER
              || op.TokenType == TokenType.LESS_EQUAL || op.TokenType == TokenType.GREATER_EQUAL
              || op.TokenType == TokenType.NOT_EQUAL || op.TokenType == TokenType.EQUAL)
                priority = 5;
            else if (op.TokenType == TokenType.MINUS || op.TokenType == TokenType.PLUS)
                priority = 9;
            else if (op.TokenType == TokenType.MULT || op.TokenType == TokenType.DIV)
                priority = 10;
            else
                priority = 0;

            return priority;
        }

        private void AddOprand(Token token)
        {
            _rpn.Add(token);
        }

        private void AddOperator(Token token)
        {
            if (token.TokenType == TokenType.LEFT_PAREN)
            {
                _tokens.AddFirst(token);
                return;
            }

            if (token.TokenType == TokenType.RIGHT_PAREN)
            {
                while (_tokens.First.Value.TokenType != TokenType.LEFT_PAREN)
                {
                    AddRpn();
                }

                _tokens.RemoveFirst();
                return;
            }

            while (true)
            {
                if (_tokens.First != null && OpPriority(_tokens.First.Value) > OpPriority(token))
                {
                    AddRpn();
                }
                else
                {
                    _tokens.AddFirst(token);
                    return;
                }
            }
        }

        private void FlushTexas()
        {
            int size = _tokens.Count;

            for (int i = 0; i < size; i++)
            {
                AddRpn();
            }
        }
        public void Print()
        {
            Console.WriteLine("Rpn: ");
            Console.Write($"{"№",-4} {"Name token",-20}{"Value",-20}\n");

            int i = 0;

            foreach (Token token in _rpn)
            {
                Console.Write($"{i++, -4}");
                token.Println();
            }

            Console.WriteLine();
        }

        private void AddRpn()
        {
            _rpn.Add(_tokens.First.Value);
            _tokens.RemoveFirst();
        }

        public List<Token> GetRpn()
        {
            return _rpn;
        }
    }
}