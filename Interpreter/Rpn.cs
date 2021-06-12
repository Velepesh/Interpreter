using System;
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
        private HashSet<string> _linkedListVariables = new HashSet<string>();
        private HashSet<string> _hashSetVariables = new HashSet<string>();

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
                case NodeType.method:
                    MethodExpr(nodeExpr);
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
        private void AssignExpr(AstNode node)
        {
            AddOprand(node.GetLeafs()[0]);
            AddOperator(node.GetLeafs()[1]);
            Value(node.GetNodes()[0]);
            FlushTexas();
        }

        private void WhileExpr(AstNode node)
        {
            int start = _rpn.Count;

            Value(node.GetNodes()[0]);
            FlushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            AddOprand(point);

            AddOprand(node.GetLeafs()[0]);
            Body(node.GetNodes()[1]);

            Token endPoint = new Token(TokenType.JMP_VALUE);
            AddOprand(endPoint);
            AddOprand(new Token(TokenType.JMP));

            point.SetValue(Convert.ToString(_rpn.Count));
            endPoint.SetValue(Convert.ToString(start));
        }

        private void IfExpr(AstNode node)
        {
            Value(node.GetNodes()[0]);
            FlushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            AddOprand(point);

            AddOprand(node.GetLeafs()[0]);
            Body(node.GetNodes()[1]);

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

        private void ElifExpr(AstNode node, Token endPoint)
        {
            Value(node.GetNodes()[0]);
            FlushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            AddOprand(point);

            AddOprand(node.GetLeafs()[0]);
            Body(node.GetNodes()[1]);

            AddOprand(endPoint);
            AddOprand(new Token(TokenType.JMP));

            point.SetValue(Convert.ToString(_rpn.Count));
        }

        private void ElseExpr(AstNode node)
        {
            Body(node.GetNodes()[0]);
        }

        private void DoWhileExpr(AstNode node)
        {
            int start = _rpn.Count;

            Body(node.GetNodes()[0]);

            Value(node.GetNodes()[1]);
            FlushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            AddOprand(point);

            AddOprand(node.GetLeafs()[0]);

            point.SetValue(Convert.ToString(start));
        }

        private void ForExpr(AstNode node)
        {
            AssignExpr(node.GetNodes()[0]);

            int start = _rpn.Count;

            Value(node.GetNodes()[1]);
            FlushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            AddOprand(point);
            AddOprand(node.GetLeafs()[0]);

            Body(node.GetNodes()[3]);
            AssignExpr(node.GetNodes()[2]);

            Token endPoint = new Token(TokenType.JMP_VALUE);
            AddOprand(endPoint);
            AddOprand(new Token(TokenType.JMP));

            point.SetValue(Convert.ToString(_rpn.Count));
            endPoint.SetValue(Convert.ToString(start));
        }

        private void PrintExpr(AstNode node)
        {
            Value(node.GetNodes()[0]);
            FlushTexas();
            AddOprand(node.GetLeafs()[0]);
        }

        private void MethodExpr(AstNode node)
        {
            var value = node.GetLeafs()[0].Value;

            if (_linkedListVariables.Contains(value))
                CallLinkedListMethod(node);
            else if (_hashSetVariables.Contains(value))
                CallHashSetMethod(node);
            else
                throw new Exception($"Name {value} does not exist in current context");

            AddOprand(node.GetLeafs()[0]);
            AddOprand(node.GetLeafs()[2]);
        }

        private void LinkedListExpr(AstNode node)
        {
            AddOprand(node.GetLeafs()[1]);
            AddOprand(node.GetLeafs()[0]);

            _linkedListVariables.Add(node.GetLeafs()[1].Value);
        }

        private void HashSetExpr(AstNode node)
        {
            AddOprand(node.GetLeafs()[1]);
            AddOprand(node.GetLeafs()[0]);

            _hashSetVariables.Add(node.GetLeafs()[1].Value);
        }

        private void Body(AstNode topNode)
        {
            foreach (AstNode node in topNode.GetNodes())
            {
                Language(node);
            }
        }

        private void Value(AstNode topNode)
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
            else if (nextNodes[0].Type == NodeType.method)
            {
                MethodExpr(nextNodes[0]);
            }

            if (nextNodes.Count > 1)
            {
                Op(nextNodes[1]);
                Value(nextNodes[2]);
            }
        }

        private void CallLinkedListMethod(AstNode node)
        {
            var value = node.GetLeafs()[2].Value;

            if (value.Equals("Add"))
            {
                if (node.GetNodes().Count == 2)
                {
                    Value(node.GetNodes()[0]);
                    Value(node.GetNodes()[1]);
                }
                else
                {
                    throw new Exception();
                }
            }
            if (value.Equals("AddFirst"))
            {
                if (node.GetNodes().Count == 1)
                {
                    Value(node.GetNodes()[0]);
                }
                else
                {
                    throw new Exception();
                }
            }
            if (value.Equals("AddLast"))
            {
                if (node.GetNodes().Count == 1)
                {
                    Value(node.GetNodes()[0]);
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (value.Equals("Size"))
            {
                if (node.GetNodes().Count != 0)
                {
                    throw new Exception("No overload for method 'method' takes 'number' arguments");
                }
            }
            else if (value.Equals("PrintLinkedList"))
            {
                if (node.GetNodes().Count != 0)
                {
                    throw new Exception("No overload for method 'method' takes 'number' arguments");
                }
            }
            else if (value.Equals("Get"))
            {
                if (node.GetNodes().Count == 1)
                {
                    Value(node.GetNodes()[0]);
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (value.Equals("GetFirst"))
            {
                if (node.GetNodes().Count != 0)
                {
                    throw new Exception("No overload for method 'method' takes 'number' arguments");
                }
            }
            else if (value.Equals("GetLast"))
            {
                if (node.GetNodes().Count != 0)
                {
                    throw new Exception("No overload for method 'method' takes 'number' arguments");
                }
            }
            else if (value.Equals("Contains"))
            {
                if (node.GetNodes().Count == 1)
                {
                    Value(node.GetNodes()[0]);
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (value.Equals("Remove"))
            {
                if (node.GetNodes().Count == 1)
                {
                    Value(node.GetNodes()[0]);
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private void CallHashSetMethod(AstNode node)
        {
            var value = node.GetLeafs()[2].Value;

            if (value.Equals("Add"))
            {
                if (node.GetNodes().Count == 1)
                {
                    Value(node.GetNodes()[0]);
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (value.Equals("Remove"))
            {
                if (node.GetNodes().Count == 1)
                {
                    Value(node.GetNodes()[0]);
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (value.Equals("PrintHashSet"))
            {
                if (node.GetNodes().Count != 0)
                {
                    throw new Exception("No overload for method 'method' takes 'number' arguments");
                }
            }
            if (value.Equals("Contains"))
            {

                if (node.GetNodes().Count == 1)
                {
                    Value(node.GetNodes()[0]);
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private void BracketMember(AstNode node)
        {
            AddOperator(node.GetLeafs()[0]);
            Value(node.GetNodes()[0]);
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

        private int GetOperationPriority(Token op)
        {
            int priority = 0;

            if (op.TokenType == TokenType.ASSIGN)
                priority = 0;
            else if (op.TokenType == TokenType.LESS || op.TokenType == TokenType.GREATER
              || op.TokenType == TokenType.LESS_EQUAL || op.TokenType == TokenType.GREATER_EQUAL
              || op.TokenType == TokenType.NOT_EQUAL || op.TokenType == TokenType.EQUAL)
                priority = 10;
            else if (op.TokenType == TokenType.MINUS || op.TokenType == TokenType.PLUS)
                priority = 20;
            else if (op.TokenType == TokenType.MULT || op.TokenType == TokenType.DIV)
                priority = 30;

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
                if (_tokens.First != null && GetOperationPriority(_tokens.First.Value) > GetOperationPriority(token))
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
            Console.Write($"{"№",-5} {"Token name",-15}{"Value"}\n");

            int i = 0;

            foreach (Token token in _rpn)
            {
                Console.Write($"{i++,-6}");
                token.Print();
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