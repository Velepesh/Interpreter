using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class AstNode
    {
        private NodeType type;
        private List<AstNode> nodes;
        private List<Token> leafs;

        public AstNode(NodeType type)
        {

            this.type = type;
            this.leafs = new List<Token>();
            this.nodes = new List<AstNode>();
        }

        public AstNode(NodeType type, List<AstNode> nodes, List<Token> leafs)
        {

            this.type = type;
            this.leafs = leafs;
            this.nodes = nodes;
        }

        public void addLeaf(Token leaf)
        {

            leafs.Add(leaf);
        }

        public void addNode(AstNode node)
        {

            nodes.Add(node);
        }

        public void print()
        {

            this.printNext("", "", "", true);
        }

        private void printNext(string format, string format2, string format3, bool last)
        {

            if (last)
                Console.Write(format2);
            else
                Console.Write(format3);

                 Console.WriteLine("╥" + type);

            for (int i = 0; i < leafs.Count; i++)
            {

                if (i == leafs.Count - 1 && this.nodes.Count == 0)
                {

                    Console.Write(format + "╙────");
                    Console.WriteLine(leafs[i].Terminal.TokenType + (leafs[i].Value == null ? "" : (" (" + leafs[i].Value + ")")));
                }
                else
                {

                    Console.Write(format + "╟────");
                    Console.WriteLine(leafs[i].Terminal.TokenType + (leafs[i].Value == null ? "" : (" (" + leafs[i].Value + ")")));
                }
            }

            for (int i = 0; i < nodes.Count; i++)
            {

                if (i == nodes.Count - 1)
                    nodes[i].printNext(format + "     ", format + "╟────", format + "╙────", false);
                else
                    nodes[i].printNext(format + "║    ", format + "╟────", format + "╙────", true);
            }
        }

        public NodeType getType()
        {

            return type;
        }

        public List<AstNode> getNodes()
        {

            return nodes;
        }

        public List<Token> getLeafs()
        {

            return leafs;
        }
    }
}
