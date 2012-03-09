﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniPlInterpreter;

namespace AST
{
    public interface Node
    {
        void accept(NodeVisitor visitor);
    }

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

        public void accept(NodeVisitor visitor)
        {
            foreach (Statement child in Children)
            {
                child.accept(visitor);
                visitor.visit(this);
            }
        }
    }

    public abstract class Literal : Expression
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

        public abstract void accept(NodeVisitor visitor);
    }

    public class IntegerLiteral : Literal
    {
        public IntegerLiteral(string value) : base(value) { }

        public override void accept(NodeVisitor visitor)
        {
            visitor.visit(this);
        }
    }

    public class StringLiteral : Literal
    {
        public StringLiteral(string value) : base(value) { }

        public override void accept(NodeVisitor visitor)
        {
            visitor.visit(this);
        }
    }

    public abstract class Variable : Node
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

        public abstract void accept(NodeVisitor visitor);
    }

    public class VariableReference : Variable, Expression
    {

        public VariableReference(string name)
            : base(name) { }

        public override void accept(NodeVisitor visitor)
        {
            visitor.visit(this);
        }
    }

    public class VariableDeclaration : Variable, Statement
    {
        public string Type
        {
            get;
            private set;
        }

        public VariableDeclaration(string name, string type)
            : base(name)
        {
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

        public BinaryOp(string opsymbol, Expression lhs, Expression rhs)
        {
            OpSymbol = opsymbol;
            LeftOp = lhs;
            RightOp = rhs;
        }

        public abstract void accept(NodeVisitor visitor);
    }

    public class ArithmeticOp : BinaryOp
    {
        public ArithmeticOp(string opsymbol, Expression lhs, Expression rhs)
            : base(opsymbol, lhs, rhs) { }

        public override void accept(NodeVisitor visitor)
        {
            LeftOp.accept(visitor);
            RightOp.accept(visitor);
            visitor.visit(this);
        }
    }

    public class LogicalOp : BinaryOp
    {
        public LogicalOp(string opsymbol, Expression lhs, Expression rhs)
            : base(opsymbol, lhs, rhs) { }

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

        public UnaryNot(Expression operand)
        {
            Operand = operand;
        }

        public void accept(NodeVisitor visitor)
        {
            Operand.accept(visitor);
            visitor.visit(this);
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

        public void accept(NodeVisitor visitor)
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
        public Expression Expression
        {
            get;
            private set;
        }

        public Assignment(Variable variable, Expression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public void accept(NodeVisitor visitor)
        {
            Variable.accept(visitor);
            Expression.accept(visitor);
            visitor.visit(this);
        }
    }

    public abstract class KeywordStatement : Statement
    {
        public string Keyword
        {
            get;
            private set;
        }

        public KeywordStatement(string keyword)
        {
            Keyword = keyword;
        }

        public abstract void accept(NodeVisitor visitor);
    }

    public class ExpressionStatement : KeywordStatement
    {
        public Expression Expression
        {
            get;
            private set;
        }

        public ExpressionStatement(string keyword, Expression expression)
            : base(keyword)
        {
            Expression = expression;
        }

        public override void accept(NodeVisitor visitor)
        {
            Expression.accept(visitor);
            visitor.visit(this);
        }
    }

    public class ReadStatement : KeywordStatement
    {
        public VariableReference Variable
        {
            get;
            private set;
        }

        public ReadStatement(string keyword, VariableReference variable)
            : base(keyword)
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

        public Loop(VariableReference variable, Range range, List<Statement> body)
        {
            Variable = variable;
            Range = range;
            LoopBody = body;
        }

        public void accept(NodeVisitor visitor)
        {
            Variable.accept(visitor);
            Range.accept(visitor);

            foreach (Statement node in LoopBody)
            {
                node.accept(visitor);
            }

            visitor.visit(this);
        }
    }
}
