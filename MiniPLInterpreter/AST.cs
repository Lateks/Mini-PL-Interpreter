using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AST
{
    public interface Node { }

    public interface Assignable { }

    public interface Expression : Node { }

    public interface Statement : Node { }

    public class Program : Node
    {
        public List<Statement> Children
        {
            get;
            private set;
        }

        public Program(List<Statement> statements)
        {
            Children = statements;
        }
    }

    public class Literal : Expression
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

    public class Variable : Expression, Assignable
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

    public class BinaryOp : Expression
    {
        public Expression LeftOp
        {
            get;
            private set;
        }
        public Expression RightOp
        {
            get;
            private set;
        }
        public string OpSymbol
        {
            get;
            private set;
        }

        public BinaryOp(string opsymbol, Expression lhs, Expression rhs)
        {
            OpSymbol = opsymbol;
            LeftOp = lhs;
            RightOp = rhs;
        }
    }

    public class UnaryNot : Expression
    {
        public Expression Operand
        {
            get;
            private set;
        }

        public UnaryNot(Expression operand)
        {
            Operand = operand;
        }
    }

    public class Range : Node
    {
        public Expression Begin
        {
            get;
            private set;
        }
        public Expression End
        {
            get;
            private set;
        }

        public Range(Expression lhs, Expression rhs)
        {
            Begin = lhs;
            End = rhs;
        }
    }

    public class Assignment : Statement
    {
        public Assignable Variable
        {
            get;
            private set;
        }
        public Expression Expression
        {
            get;
            private set;
        }

        public Assignment(Assignable variable, Expression expression)
        {
            Variable = variable;
            Expression = expression;
        }
    }

    public class KeywordStatement : Statement
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
        public Expression Expression
        {
            get;
            private set;
        }

        public ExpressionStatement(Keyword keyword, Expression expression)
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

    public class VariableDeclaration : Statement, Assignable
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

    public class Loop : Statement
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
        public List<Statement> LoopBody
        {
            get;
            private set;
        }

        public Loop(Variable variable, Range range, List<Statement> body)
        {
            Variable = variable;
            Range = range;
            LoopBody = body;
        }
    }
}
