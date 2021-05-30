using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class TranslatorRPN
    {
        private AstNode _astNode;

        private LinkedList<Token> _tokens = new LinkedList<Token>();
        private List<Token> RPN = new List<Token>();

        public TranslatorRPN(AstNode astNode)
        {
            _astNode = astNode;
        }

        public void translate()
        {
            foreach (AstNode node in _astNode.GetNodes())
            {
                languageConst(node);
            }
        }

        private void languageConst(AstNode node)
        {

            AstNode nodeConst = node.GetNodes()[0];

            if (nodeConst.Type == NodeType.assign_expr)
            {

                assignConst(nodeConst);
            }
            else if (nodeConst.Type == NodeType.if_expr)
            {

                ifConst(nodeConst);
            }
            else if (nodeConst.Type == NodeType.while_expr)
            {

                whileConst(nodeConst);
            }
        }

        private void whileConst(AstNode nodeConst)
        {

            int start = RPN.Count;

            expression(nodeConst.GetNodes()[0]);
            flushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            addOprand(point);

            addOprand(nodeConst.GetLeafs()[0]);
            block(nodeConst.GetNodes()[1]);

            Token endPoint = new Token(TokenType.JMP_VALUE);
            addOprand(endPoint);
            addOprand(new Token(TokenType.JMP));

            point.SetValue(Convert.ToString(RPN.Count));
            endPoint.SetValue(Convert.ToString(start));
        }

        private void ifConst(AstNode nodeConst)
        {

            expression(nodeConst.GetNodes()[0]);
            flushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            addOprand(point);

            addOprand(nodeConst.GetLeafs()[0]);
            block(nodeConst.GetNodes()[1]);

            Token endPoint = new Token(TokenType.JMP_VALUE);
            addOprand(endPoint);
            addOprand(new Token(TokenType.JMP));

            point.SetValue(Convert.ToString(RPN.Count));

            for (int i = 2; i < nodeConst.GetNodes().Count; i++)
            {

                if (nodeConst.GetNodes()[i].Type == NodeType.elif_expr)
                {

                    elifConst(nodeConst.GetNodes()[i], endPoint);
                }
                else if (nodeConst.GetNodes()[i].Type == NodeType.else_expr)
                {

                    elseConst(nodeConst.GetNodes()[i]);
                }
            }

            endPoint.SetValue(Convert.ToString(RPN.Count));
        }

        private void elifConst(AstNode nodeConst, Token endPoint)
        {

            expression(nodeConst.GetNodes()[0]);
            flushTexas();

            Token point = new Token(TokenType.JMP_VALUE);
            addOprand(point);

            addOprand(nodeConst.GetLeafs()[0]);
            block(nodeConst.GetNodes()[1]);

            addOprand(endPoint);
            addOprand(new Token(TokenType.JMP));

            point.SetValue(Convert.ToString(RPN.Count));
        }

        private void elseConst(AstNode nodeConst)
        {

            block(nodeConst.GetNodes()[0]);
        }

        private void block(AstNode topNode)
        {

            foreach (AstNode node in topNode.GetNodes())
            {

                languageConst(node);
            }
        }

        private void assignConst(AstNode nodeConst)
        {

            addOprand(nodeConst.GetLeafs()[0]);
            addOperator(nodeConst.GetLeafs()[1]);
            expression(nodeConst.GetNodes()[0]);
            flushTexas();
        }

        private void expression(AstNode topNode)
        {

            List<AstNode> nextNodes = topNode.GetNodes();

            if (nextNodes[0].Type == NodeType.member)
            {

                member(nextNodes[0]);
            }
            else if (nextNodes[0].Type == NodeType.bracket_member)
            {

                bracketMember(nextNodes[0]);
            }
            if (nextNodes.Count > 1)
            {
                op(nextNodes[1]);
                expression(nextNodes[2]);
            }
        }

        private void bracketMember(AstNode node)
        {

            addOperator(node.GetLeafs()[0]);
            expression(node.GetNodes()[0]);
            addOperator(node.GetLeafs()[1]);
        }

        private void op(AstNode node)
        {

            addOperator(node.GetLeafs()[0]);
        }

        private void member(AstNode node)
        {

            addOprand(node.GetLeafs()[0]);
        }

        private int opPriority(Token op)
        {

            int priority;

            if (op.TokenType == TokenType.ASSIGN)
                priority = 0;
            else if (op.TokenType == TokenType.LESS)
                priority = 5;
            else if (op.TokenType == TokenType.GREATER)
                priority = 5;
            else if (op.TokenType == TokenType.LESS_EQUAL)
                priority = 5;
            else if (op.TokenType == TokenType.GREATER_EQUAL)
                priority = 5;
            else if (op.TokenType == TokenType.NOT_EQUAL)
                priority = 5;
            else if (op.TokenType == TokenType.EQUAL)
                priority = 5;
            else if (op.TokenType == TokenType.MINUS)
                priority = 9;
            else if (op.TokenType == TokenType.MULT)
                priority = 10;
            else if (op.TokenType == TokenType.DIV)
                priority = 10;
            else
                priority = 0;

            return priority;
        }

        private void addOprand(Token token)
        {

            RPN.Add(token);
        }

        private void addOperator(Token token)
        {

            if (token.TokenType == TokenType.LEFT_PAREN)
            {
                _tokens.AddFirst(token);
                return;
            }

            if (token.TokenType == TokenType.RIGHT_PAREN)
            {

                while (_tokens.peek().Type != TokenType.LEFT_PAREN)
                {

                    RPN.Add(_tokens.removeFirst());
                }

                _tokens.RemoveFirst();
                return;
            }

            while (true)
            {

                if (_tokens.peek() != null && opPriority(_tokens.peek()) > opPriority(token))
                    RPN.Add(_tokens.removeFirst());
                else
                {

                    _tokens.AddFirst(token);
                    return;
                }
            }
        }

        private void flushTexas()
        {

            int size = _tokens.Count;

            for (int i = 0; i < size; i++)
            {

                RPN.Add(_tokens.removeFirst());
            }
        }

        public void print()
        {

            Console.WriteLine("[RPN translator] reverse polish notation: ");
            Console.Write("%-4s%-20s%-20s\n", "№", "Name token", "Value");

            int i = 0;

            foreach (Token token in RPN)
            {
                Console.Write("%-4s", i++);
                token.Println();
            }
        }

        public List<Token> getRPN()
        {
            return RPN;
        }
    }
}
