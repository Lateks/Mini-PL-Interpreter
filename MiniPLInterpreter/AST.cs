using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AST
{
    public interface Node
    {
    }

    public class Literal
    {
        private string value;
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

    public class Variable : Node
    {
        private string name;
        private string type;

        public Variable(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class Keyword : Node
    {
        private string name;

        public Keyword(string name)
        {
            this.name = name;
        }
    }

    public class BinaryOp : Node
    {
        private Node lhs;
        private Node rhs;

        public BinaryOp(Node lhs, Node rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }
    }

    public class UnaryOp : Node
    {
        private Node operand;

        public UnaryOp(Node operand)
        {
            this.operand = operand;
        }
    }

    public class Loop : Node
    {
        private Node var;
        private Node loop_body;
        private Node range;

        public Loop(Variable var, Range range, Node body)
        {
            this.var = var;
            this.loop_body = body;
            this.range = range;
        }
    }

    public class Range : BinaryOp
    {
        public Range(Node start, Node end) : base(start, end) { }
    }

    public class Statement : Node
    {
        private Node keyword;
        private Node expression;

        public Statement(Keyword keyword, Node expression)
        {
            this.keyword = keyword;
            this.expression = expression;
        }
    }
}
