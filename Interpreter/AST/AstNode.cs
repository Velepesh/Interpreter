using System;
using System.Collections.Generic;

namespace Interpreter
{
    class AstNode
    {
        private List<AstNode> _nodes = new List<AstNode>();
        private List<Token> _leafs = new List<Token>();

        public NodeType Type { get; private set; }

        public AstNode(NodeType type)
        {
            Type = type;
        }

        public AstNode(NodeType type, List<AstNode> nodes, List<Token> leafs)
        {
            Type = type;
            _nodes = nodes;
            _leafs = leafs;
        }

        public void AddLeaf(Token leaf)
        {
            _leafs.Add(leaf);
        }

        public void AddNode(AstNode node)
        {
            _nodes.Add(node);
        }

        public List<AstNode> GetNodes()
        {
            return _nodes;
        }

        public List<Token> GetLeafs()
        {
            return _leafs;
        }

        public void ShowAstTree()
        {
            Console.WriteLine("\nAST\n");
            PrintNext();
        }

        private void PrintNext(string indent = "", string nextIndent = "", string nodeIndent = "", bool isLast = true)
        {
            var marker = isLast ? nextIndent : nodeIndent;

            Console.Write(marker);

            Console.WriteLine("." + Type);

            for (int i = 0; i < _leafs.Count; i++)
            {
                if (i == _leafs.Count - 1 && _nodes.Count == 0)
                {
                    Console.Write(indent + "└──");
                    Console.WriteLine(_leafs[i].TokenType + (_leafs[i].Value == null ? "" : (" (" + _leafs[i].Value + ")")));
                }
                else
                {
                    Console.Write(indent + "├──");
                    Console.WriteLine(_leafs[i].TokenType + (_leafs[i].Value == null ? "" : (" (" + _leafs[i].Value + ")")));
                }
            }

            for (int i = 0; i < _nodes.Count; i++)
            {
                if (i == _nodes.Count - 1)
                    _nodes[i].PrintNext(indent + "   ", indent + "├──", indent + "└──", false);
                else
                    _nodes[i].PrintNext(indent + "│  ", indent + "├──", indent + "└──", true);
            }
        }
    }
}
