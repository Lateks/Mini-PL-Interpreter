using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MiniPLInterpreter;
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

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(10));
        }

        [Test]
        public void Addition()
        {
            var addop = new ArithmeticOp("+", op1, op2);
            var assignment = new Assignment(resultdecl, addop);
            program.Add(assignment);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(7));
        }

        [Test]
        public void Subtraction()
        {
            var subop = new ArithmeticOp("-", op1, op2);
            var assignment = new Assignment(resultdecl, subop);
            program.Add(assignment);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(3));
        }

        [Test]
        public void Division()
        {
            var divop = new ArithmeticOp("/", op1, op2);
            var assignment = new Assignment(resultdecl, divop);
            program.Add(assignment);

            interpreter.Run(new Program(program));
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

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(10));
        }

        [Test]
        public void Addition()
        {
            var addop = new ArithmeticOp("+", op1, op2);
            var assignment = new Assignment(resultdecl, addop);
            program.Add(assignment);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(7));
        }

        [Test]
        public void Subtraction()
        {
            var subop = new ArithmeticOp("-", op1, op2);
            var assignment = new Assignment(resultdecl, subop);
            program.Add(assignment);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(3));
        }

        [Test]
        public void Division()
        {
            var divop = new ArithmeticOp("/", op1, op2);
            var assignment = new Assignment(resultdecl, divop);
            program.Add(assignment);

            interpreter.Run(new Program(program));
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

            Assert.DoesNotThrow(() => interpreter.Run(new Program(program)));
        }

        [Test]
        public void FailedAssert()
        {
            var boolean = new LogicalOp("=", new IntegerLiteral("4"), new IntegerLiteral("5"));
            var assertion = new ExpressionStatement("assert", boolean);
            program.Add(assertion);

            Assert.Throws<MiniPLAssertionFailed>(() => interpreter.Run(new Program(program)));
        }
    }

    [TestFixture]
    class LogicalOpTest
    {
        SymbolTable symboltable;
        List<Statement> program;
        VariableDeclaration result;
        InterpretingNodeVisitor interpreter;

        [SetUp]
        public void SetUp()
        {
            program = new List<Statement>();
            symboltable = new SymbolTable();
            symboltable.define(new Symbol("result", "bool"));
            result = new VariableDeclaration("result", "bool");
            interpreter = new InterpretingNodeVisitor(symboltable);
        }

        [Test]
        public void StringEquals()
        {
            var equals = new LogicalOp("=", new StringLiteral("foo"), new StringLiteral("foo"));
            var assignment = new Assignment(result, equals);
            program.Add(assignment);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(true));
        }

        [Test]
        public void StringDoesNotEqual()
        {
            var equals = new LogicalOp("=", new StringLiteral("bar"), new StringLiteral("foo"));
            var assignment = new Assignment(result, equals);
            program.Add(assignment);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(false));
        }

        [Test]
        public void IntEquals()
        {
            var equals = new LogicalOp("=", new IntegerLiteral("5"), new IntegerLiteral("5"));
            var assignment = new Assignment(result, equals);
            program.Add(assignment);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(true));
        }

        [Test]
        public void IntDoesNotEqual()
        {
            var equals = new LogicalOp("=", new IntegerLiteral("4"), new IntegerLiteral("5"));
            var assignment = new Assignment(result, equals);
            program.Add(assignment);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(false));
        }

        [Test]
        public void BoolEquals()
        {
            var boolean = new LogicalOp("=", new IntegerLiteral("4"), new IntegerLiteral("5"));
            var equals = new LogicalOp("=", boolean, boolean);
            var assignment = new Assignment(result, equals);
            program.Add(assignment);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(true));
        }

        [Test]
        public void BoolDoesNotEqual()
        {
            var falseboolean = new LogicalOp("=", new IntegerLiteral("4"), new IntegerLiteral("5"));
            var trueboolean = new LogicalOp("=", new IntegerLiteral("5"), new IntegerLiteral("5"));
            var equals = new LogicalOp("=", falseboolean, trueboolean);
            var assignment = new Assignment(result, equals);
            program.Add(assignment);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(false));
        }

        [Test]
        public void FalseAnd()
        {
            var falseboolean = new LogicalOp("=", new IntegerLiteral("4"), new IntegerLiteral("5"));
            var trueboolean = new LogicalOp("=", new IntegerLiteral("4"), new IntegerLiteral("5"));
            var and1 = new LogicalOp("&", falseboolean, trueboolean);
            var and2 = new LogicalOp("&", falseboolean, falseboolean);
            var assignment1 = new Assignment(result, and1);
            program.Add(assignment1);
            var result2 = new VariableDeclaration("result2", "bool");
            symboltable.define(new Symbol("result2", "bool"));
            var assignment2 = new Assignment(result2, and2);
            program.Add(assignment2);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(false));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result2")], Is.EqualTo(false));
        }

        [Test]
        public void TrueAnd()
        {
            var trueboolean = new LogicalOp("=", new IntegerLiteral("4"), new IntegerLiteral("4"));
            var and = new LogicalOp("&", trueboolean, trueboolean);
            var assignment = new Assignment(result, and);
            program.Add(assignment);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(true));
        }
    }

    [TestFixture]
    class TestLoop
    {
        [Test]
        public void LoopTest()
        {
            var loopvardecl = new VariableDeclaration("loopvariable", "int");
            var loopvarref = new VariableReference("loopvariable");
            var range = new Range(new IntegerLiteral("2"), new IntegerLiteral("5"));
            var loopbody = new List<Statement>();
            var resultdecl = new VariableDeclaration("result", "int");
            var resultref = new VariableReference("result");
            loopbody.Add(new Assignment(resultref,
                new ArithmeticOp("+", resultref, loopvarref)));
            var program = new List<Statement>();
            program.Add(resultdecl);
            program.Add(loopvardecl);
            program.Add(new Loop(loopvarref, range, loopbody));

            var symboltable = new SymbolTable();
            symboltable.define(new Symbol("loopvariable", "int"));
            symboltable.define(new Symbol("result", "int"));
            var interpreter = new InterpretingNodeVisitor(symboltable);

            interpreter.Run(new Program(program));
            Assert.That(interpreter.Valuetable[symboltable.resolve("result")], Is.EqualTo(14));
        }
    }
}
