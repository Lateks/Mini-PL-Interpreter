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
        private List<Node> nodes;
        public List<Node> Children
        {
            get { return nodes; }
        }

        public Program(List<Node> statements)
        {
            this.nodes = statements;
        }
    }

    public class Literal
    {
        private string value;
        public string Value
        {
            get { return value; }
        }

        public Literal(string value)
        {
            this.value = value;
        }
    }

    public class IntegerLiteral : Literal, Node
    {
        public IntegerLiteral(string value) : base(value) { }
    }

    public class StringLiteral : Literal, Node
    {
        public StringLiteral(string value) : base(value) { }
    }

    public interface Assignable {}

    public class VariableDeclaration : Node, Assignable
    {
        private string name;
        private string type;
        public string Name
        {
            get { return name; }
        }
        public string Type
        {
            get { return type; }
        }

        public VariableDeclaration(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class Variable : Node, Assignable
    {
        private string name;
        public string Name
        {
            get { return name; }
        }
        public Variable(string name)
        {
            this.name = name;
        }
    }

    public class Keyword : Node
    {
        private string name;
        public string Name
        {
            get { return name; }
        }

        public Keyword(string name)
        {
            this.name = name;
        }
    }

    public class BinaryOp : Node
    {
        private Node lhs;
        private Node rhs;
        private string opsymbol;
        public Node LeftOp
        {
            get { return lhs; }
        }
        public Node RightOp
        {
            get { return rhs; }
        }
        public string OpSymbol
        {
            get { return opsymbol; }
        }

        public BinaryOp(string opsymbol, Node lhs, Node rhs)
        {
            this.opsymbol = opsymbol;
            this.lhs = lhs;
            this.rhs = rhs;
        }
    }

    public class UnaryNot : Node
    {
        Node operand;
        public Node Operand
        {
            get { return operand; }
        }

        public UnaryNot(Node operand)
        {
            this.operand = operand;
        }
    }

    public class Loop : Node
    {
        private Node var;
        private Node range;
        private List<Node> loop_body;
        public Node Variable
        {
            get { return var; }
        }
        public Node Range
        {
            get { return range; }
        }
        public List<Node> LoopBody
        {
            get { return loop_body; }
        }

        public Loop(Variable var, Range range, List<Node> body)
        {
            this.var = var;
            this.range = range;
            this.loop_body = body;
        }
    }

    public class Range : Node
    {
        private Node lhs;
        private Node rhs;
        public Node Begin
        {
            get { return lhs; }
        }
        public Node End
        {
            get { return rhs; }
        }

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
        public Node Variable
        {
            get { return var; }
        }
        public Node Expression
        {
            get { return expression; }
        }

        public Assignment(Assignable var, Node expression)
        {
            this.var = (Node) var;
            this.expression = expression;
        }
    }

    public class Statement : Node
    {
        private Node keyword;
        private Node expression;
        public Node Keyword
        {
            get { return keyword; }
        }
        public Node Expression
        {
            get { return expression; }
        }

        public Statement(Keyword keyword, Node expression)
        {
            this.keyword = keyword;
            this.expression = expression;
        }
    }
}
