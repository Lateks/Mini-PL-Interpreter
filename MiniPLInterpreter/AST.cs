using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AST
{
    public interface Node
    {
    }

    public class Program : Node
    {
        List<Node> nodes;

        public Program()
        {
            this.nodes = new List<Node>();
        }

        public void AddChild(Node child)
        {
            nodes.Add(child);
        }

        public void AddChildren(List<Node> children)
        {
            nodes.AddRange(children);
        }
    }

    public class Literal
    {
        private string value;
        public Literal(string value)
        {
            this.value = value;
        }
    }

    public class IntegerLiteralNode : Literal, Node
    {
        public IntegerLiteralNode(string value) : base(value) { }
    }

    public class StringLiteralNode : Literal, Node
    {
        public StringLiteralNode(string value) : base(value) { }
    }

    public interface Assignable {}

    public class VariableDeclaration : Node, Assignable
    {
        private string name;
        private string type;

        public VariableDeclaration(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class Variable : Node, Assignable
    {
        private string name;
        public Variable(string name)
        {
            this.name = name;
        }
    }

    public class KeywordNode : Node
    {
        private string name;

        public KeywordNode(string name)
        {
            this.name = name;
        }
    }

    public class BinaryOp : Node
    {
        private Node lhs;
        private Node rhs;
        private string opsymbol;

        public BinaryOp(string opsymbol)
        {
            this.opsymbol = opsymbol;
        }

        public void AddChildren(Node lhs, Node rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }
    }

    public class UnaryOpNot : Node
    {
        Node operand;

        public UnaryOpNot() { }

        public void AddChild(Node child)
        {
            this.operand = child;
        }
    }

    public class Loop : Node
    {
        private Node var;
        private List<Node> loop_body;
        private Node range;

        public Loop(Variable var, Range range, List<Node> body)
        {
            this.var = var;
            this.loop_body = body;
            this.range = range;
        }
    }

    public class Range : Node
    {
        private Node lhs;
        private Node rhs;

        public Range(Node lhs, Node rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }
    }

    public class Assignment : Node
    {
        Node var;
        Node expression;

        public Assignment() { }
        
        public void AddChildren(Assignable var, Node expression)
        {
            this.var = (Node) var;
            this.expression = expression;
        }
    }

    public class Statement : Node
    {
        private Node keyword;
        private Node expression;

        public Statement(KeywordNode keyword)
        {
            this.keyword = keyword;
            this.expression = null;
        }

        public void AddChild(Node expression)
        {
            this.expression = expression;
        }
    }
}
