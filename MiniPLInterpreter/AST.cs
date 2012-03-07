using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AST
{
    public interface Node { }

    public interface Assignable { }

    public class Program : Node
    {
        public List<Node> Children
        {
            get;
            private set;
        }

        public Program(List<Node> statements)
        {
            Children = statements;
        }
    }

    public class Literal : Node
    {
        public string Value
        {
            get;
            private set;
        }

        public Literal(string value)
        {
            Value = value;
        }
    }

    public class IntegerLiteral : Literal
    {
        public IntegerLiteral(string value) : base(value) { }
    }

    public class StringLiteral : Literal
    {
        public StringLiteral(string value) : base(value) { }
    }

    public class Variable : Node, Assignable
    {
        public string Name
        {
            get;
            private set;
        }
        public Variable(string name)
        {
            Name = name;
        }
    }

    public class Keyword : Node
    {
        public string Name
        {
            get;
            private set;
        }

        public Keyword(string name)
        {
            Name = name;
        }
    }

    public class BinaryOp : Node
    {
        public Node LeftOp
        {
            get;
            private set;
        }
        public Node RightOp
        {
            get;
            private set;
        }
        public string OpSymbol
        {
            get;
            private set;
        }

        public BinaryOp(string opsymbol, Node lhs, Node rhs)
        {
            OpSymbol = opsymbol;
            LeftOp = lhs;
            RightOp = rhs;
        }
    }

    public class UnaryNot : Node
    {
        public Node Operand
        {
            get;
            private set;
        }

        public UnaryNot(Node operand)
        {
            Operand = operand;
        }
    }

    public class Range : Node
    {
        public Node Begin
        {
            get;
            private set;
        }
        public Node End
        {
            get;
            private set;
        }

        public Range(Node lhs, Node rhs)
        {
            Begin = lhs;
            End = rhs;
        }
    }

    public class Assignment : Node
    {
        public Node Variable
        {
            get;
            private set;
        }
        public Node Expression
        {
            get;
            private set;
        }

        public Assignment(Assignable variable, Node expression)
        {
            Variable = (Node) variable;
            Expression = expression;
        }
    }

    public class KeywordStatement : Node
    {
        public Keyword Keyword
        {
            get;
            private set;
        }

        public KeywordStatement(Keyword keyword)
        {
            Keyword = keyword;
        }
    }

    public class ExpressionStatement : KeywordStatement
    {
        public Node Expression
        {
            get;
            private set;
        }

        public ExpressionStatement(Keyword keyword, Node expression)
            : base(keyword)
        {
            Expression = expression;
        }
    }

    public class ReadStatement : KeywordStatement
    {
        public Variable Variable
        {
            get;
            private set;
        }

        public ReadStatement(Keyword keyword, Variable variable)
            : base(keyword)
        {
            Variable = variable;
        }
    }

    public class VariableDeclaration : Node, Assignable
    {
        public string Name
        {
            get;
            private set;
        }
        public string Type
        {
            get;
            private set;
        }

        public VariableDeclaration(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }

    public class Loop : Node
    {
        public Node Variable
        {
            get;
            private set;
        }
        public Node Range
        {
            get;
            private set;
        }
        public List<Node> LoopBody
        {
            get;
            private set;
        }

        public Loop(Variable variable, Range range, List<Node> body)
        {
            Variable = variable;
            Range = range;
            LoopBody = body;
        }
    }
}
