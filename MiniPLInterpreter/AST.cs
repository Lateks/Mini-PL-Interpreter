using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniPLInterpreter;

namespace AST
{
    public interface Node
    {
        void accept(NodeVisitor visitor);
    }

    public abstract class SyntaxElement : Node
    {
        public int Row
        {
            get;
            private set;
        }

        public SyntaxElement(int row)
        {
            Row = row;
        }

        public abstract void accept(NodeVisitor visitor);
    }

    public abstract class Expression : SyntaxElement
    {
        public Expression(int row) : base(row) { }
    }

    public abstract class Statement : SyntaxElement
    {
        public Statement(int row) : base(row) { }
    }

    public interface Variable : Node
    {
        string Name
        {
            get;
        }
        int Row
        {
            get;
        }
    }

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

        public void accept(NodeVisitor visitor)
        {
            foreach (Statement child in Children)
            {
                child.accept(visitor);
            }
            visitor.visit(this);
        }
    }

    public abstract class Literal : Expression
    {
        public string Value
        {
            get;
            private set;
        }

        public Literal(string value, int row)
            : base(row)
        {
            Value = value;
        }
    }

    public class IntegerLiteral : Literal
    {
        public IntegerLiteral(string value, int row)
            : base(value, row) { }

        public override void accept(NodeVisitor visitor)
        {
            visitor.visit(this);
        }
    }

    public class StringLiteral : Literal
    {
        public StringLiteral(string value, int row)
            : base(value, row) { }

        public override void accept(NodeVisitor visitor)
        {
            visitor.visit(this);
        }
    }

    public class VariableReference : Expression, Variable
    {
        public string Name
        {
            get;
            set;
        }

        public VariableReference(string name, int row)
            : base(row)
        {
            Name = name;
        }

        public override void accept(NodeVisitor visitor)
        {
            visitor.visit(this);
        }
    }

    public class VariableDeclaration : Statement, Variable
    {
        public string Type
        {
            get;
            private set;
        }
        public string Name
        {
            get;
            private set;
        }

        public VariableDeclaration(string name, string type, int row)
            : base(row)
        {
            Name = name;
            Type = type;
        }

        public override void accept(NodeVisitor visitor)
        {
            visitor.visit(this);
        }
    }

    public abstract class BinaryOp : Expression
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

        public BinaryOp(string opsymbol, Expression lhs, Expression rhs, int row)
            : base(row)
        {
            OpSymbol = opsymbol;
            LeftOp = lhs;
            RightOp = rhs;
        }
    }

    public class ArithmeticOp : BinaryOp
    {
        public ArithmeticOp(string opsymbol, Expression lhs, Expression rhs, int row)
            : base(opsymbol, lhs, rhs, row) { }

        public override void accept(NodeVisitor visitor)
        {
            LeftOp.accept(visitor);
            RightOp.accept(visitor);
            visitor.visit(this);
        }
    }

    public class LogicalOp : BinaryOp
    {
        public LogicalOp(string opsymbol, Expression lhs, Expression rhs, int row)
            : base(opsymbol, lhs, rhs, row) { }

        public override void accept(NodeVisitor visitor)
        {
            LeftOp.accept(visitor);
            RightOp.accept(visitor);
            visitor.visit(this);
        }
    }

    public class UnaryNot : Expression
    {
        public Expression Operand
        {
            get;
            private set;
        }

        public UnaryNot(Expression operand, int row)
            : base(row)
        {
            Operand = operand;
        }

        public override void accept(NodeVisitor visitor)
        {
            Operand.accept(visitor);
            visitor.visit(this);
        }
    }

    public class Range : SyntaxElement
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

        public Range(Expression lhs, Expression rhs, int row)
            : base(row)
        {
            Begin = lhs;
            End = rhs;
        }

        public override void accept(NodeVisitor visitor)
        {
            Begin.accept(visitor);
            End.accept(visitor);
            visitor.visit(this);
        }
    }

    public class Assignment : Statement
    {
        public Variable Variable
        {
            get;
            private set;
        }
        public string VarName
        {
            get { return Variable.Name; }
        }
        public Expression Expression
        {
            get;
            private set;
        }

        public Assignment(Variable variable, Expression expression, int row)
            : base(row)
        {
            Variable = variable;
            Expression = expression;
        }

        public override void accept(NodeVisitor visitor)
        {
            Variable.accept(visitor);
            Expression.accept(visitor);
            visitor.visit(this);
        }
    }

    public class ExpressionStatement : Statement
    {
        public string Keyword
        {
            get;
            private set;
        }
        public Expression Expression
        {
            get;
            private set;
        }

        public ExpressionStatement(string keyword, Expression expression, int row)
            : base(row)
        {
            Keyword = keyword;
            Expression = expression;
        }

        public override void accept(NodeVisitor visitor)
        {
            Expression.accept(visitor);
            visitor.visit(this);
        }
    }

    public class ReadStatement : Statement
    {
        public VariableReference Variable
        {
            get;
            private set;
        }
        public string VarName
        {
            get { return Variable.Name; }
        }

        public ReadStatement(VariableReference variable, int row)
            : base(row)
        {
            Variable = variable;
        }

        public override void accept(NodeVisitor visitor)
        {
            Variable.accept(visitor);
            visitor.visit(this);
        }
    }

    public class Loop : Statement
    {
        public VariableReference Variable
        {
            get;
            private set;
        }
        public string VarName
        {
            get { return Variable.Name; }
        }
        public Range Range
        {
            get;
            private set;
        }
        public List<Statement> LoopBody
        {
            get;
            private set;
        }

        public Loop(VariableReference variable, Range range, List<Statement> body, int row)
            : base(row)
        {
            Variable = variable;
            Range = range;
            LoopBody = body;
        }

        public override void accept(NodeVisitor visitor)
        {
            Variable.accept(visitor);
            Range.accept(visitor);
            visitor.visit(this);
        }
    }
}