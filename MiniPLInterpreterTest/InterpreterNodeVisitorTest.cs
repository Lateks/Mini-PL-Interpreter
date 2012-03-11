using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MiniPlInterpreter;
using AST;
using Errors;

namespace MiniPLInterpreterTest
{
    [TestFixture]
    class TestVariableArithmetic
    {
        SymbolTable symboltable;
        Assignment op1assignment;
        Assignment op2assignment;
        VariableDeclaration resultdecl;
        VariableReference op1;
        VariableReference op2;
        List<Statement> program;
        InterpretingNodeVisitor interpreter;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            var op1decl = new VariableDeclaration("op1", "int");
            var op2decl = new VariableDeclaration("op2", "int");
            resultdecl = new VariableDeclaration("result", "int");
            op1assignment = new Assignment(op1decl, new IntegerLiteral("5"));
            op2assignment = new Assignment(op2decl, new IntegerLiteral("2"));
            op1 = new VariableReference("op1");
            op2 = new VariableReference("op2");
        }

        [SetUp]
        public void SetUp()
        {
            symboltable = new SymbolTable();
            symboltable.define(new Symbol("op1", "int"));
            symboltable.define(new Symbol("op2", "int"));
            symboltable.define(new Symbol("result", "int"));

            program = new List<Statement>();
            program.Add(op1assignment);
            program.Add(op2assignment);

            interpreter = new InterpretingNodeVisitor(symboltable);
        }

        [Test]
        public void Multiplication()
        {
            var multop = new ArithmeticOp("*", op1, op2);
            var assignment = new Assignment(resultdecl, multop);
            program.Add(assignment);

            interpreter.run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(10));
        }

        [Test]
        public void Addition()
        {
            var addop = new ArithmeticOp("+", op1, op2);
            var assignment = new Assignment(resultdecl, addop);
            program.Add(assignment);

            interpreter.run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(7));
        }

        [Test]
        public void Subtraction()
        {
            var subop = new ArithmeticOp("-", op1, op2);
            var assignment = new Assignment(resultdecl, subop);
            program.Add(assignment);

            interpreter.run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(3));
        }

        [Test]
        public void Division()
        {
            var divop = new ArithmeticOp("/", op1, op2);
            var assignment = new Assignment(resultdecl, divop);
            program.Add(assignment);

            interpreter.run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(2));
        }
    }

    [TestFixture]
    class TestLiteralArithmetic
    {
        SymbolTable symboltable;
        VariableDeclaration resultdecl;
        IntegerLiteral op1;
        IntegerLiteral op2;
        List<Statement> program;
        InterpretingNodeVisitor interpreter;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            resultdecl = new VariableDeclaration("result", "int");
            op1 = new IntegerLiteral("5");
            op2 = new IntegerLiteral("2");
        }

        [SetUp]
        public void SetUp()
        {
            symboltable = new SymbolTable();
            symboltable.define(new Symbol("result", "int"));

            program = new List<Statement>();
            interpreter = new InterpretingNodeVisitor(symboltable);
        }

        [Test]
        public void Multiplication()
        {
            var multop = new ArithmeticOp("*", op1, op2);
            var assignment = new Assignment(resultdecl, multop);
            program.Add(assignment);

            interpreter.run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(10));
        }

        [Test]
        public void Addition()
        {
            var addop = new ArithmeticOp("+", op1, op2);
            var assignment = new Assignment(resultdecl, addop);
            program.Add(assignment);

            interpreter.run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(7));
        }

        [Test]
        public void Subtraction()
        {
            var subop = new ArithmeticOp("-", op1, op2);
            var assignment = new Assignment(resultdecl, subop);
            program.Add(assignment);

            interpreter.run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(3));
        }

        [Test]
        public void Division()
        {
            var divop = new ArithmeticOp("/", op1, op2);
            var assignment = new Assignment(resultdecl, divop);
            program.Add(assignment);

            interpreter.run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(2));
        }
    }

    [TestFixture]
    class TestAssert
    {
        SymbolTable symboltable;
        List<Statement> program;
        InterpretingNodeVisitor interpreter;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            symboltable = new SymbolTable();
            interpreter = new InterpretingNodeVisitor(symboltable);
        }

        [SetUp]
        public void SetUp()
        {
            program = new List<Statement>();
        }

        [Test]
        public void SuccessfulAssert()
        {
            var boolean = new LogicalOp("=", new IntegerLiteral("5"), new IntegerLiteral("5"));
            var assertion = new ExpressionStatement("assert", boolean);
            program.Add(assertion);

            Assert.DoesNotThrow(() => interpreter.run(new Program(program)));
        }

        [Test]
        public void FailedAssert()
        {
            var boolean = new LogicalOp("=", new IntegerLiteral("4"), new IntegerLiteral("5"));
            var assertion = new ExpressionStatement("assert", boolean);
            program.Add(assertion);

            Assert.Throws<AssertionFailed>(() => interpreter.run(new Program(program)));
        }
    }
}
