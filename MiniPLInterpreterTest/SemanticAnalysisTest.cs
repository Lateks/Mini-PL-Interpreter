using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MiniPlInterpreter;
using SyntaxAnalysis;
using LexicalAnalysis;
using AST;
using Errors;

namespace MiniPLInterpreterTest
{
    [TestFixture]
    class PrintAndReadTest
    {
        List<Statement> statementlist;
        TypeCheckingVisitor symbolTableBuilder;

        [SetUp]
        public void SetUp()
        {
            statementlist = new List<Statement>();
            symbolTableBuilder = new TypeCheckingVisitor();
        }

        [Datapoint]
        string stringtype = "string";

        [Datapoint]
        string inttype = "int";

        [Theory]
        public void ValidPrint(string type)
        {
            var variabledecl = new VariableDeclaration("foo", type);
            statementlist.Add(variabledecl);
            var variable = new VariableReference("foo");
            var print = new ExpressionStatement("print", variable);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void PrintBool()
        {
            var integer1 = new IntegerLiteral("1");
            var integer2 = new IntegerLiteral("2");
            var equal = new LogicalOp("=", integer1, integer2);
            var print = new ExpressionStatement("print", equal);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Theory]
        public void ValidRead(string type)
        {
            var variabledecl = new VariableDeclaration("foo", type);
            statementlist.Add(variabledecl);
            var variable = new VariableReference("foo");
            var print = new ExpressionStatement("read", variable);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void ReadBool()
        {
            var variabledecl = new VariableDeclaration("foo", "bool");
            statementlist.Add(variabledecl);
            var variable = new VariableReference("foo");
            var print = new ReadStatement(variable);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }
    }

    [TestFixture]
    class SemanticAnalysisTest
    {
        List<Statement> statementlist;
        TypeCheckingVisitor symbolTableBuilder;

        [SetUp]
        public void SetUp()
        {
            statementlist = new List<Statement>();
            symbolTableBuilder = new TypeCheckingVisitor();
        }

        [Test]
        public void IntAssignmentTest()
        {
            var variable = new VariableDeclaration("foo", "int");
            var integer = new IntegerLiteral("4");
            var assignment = new Assignment(variable, integer);
            statementlist.Add(assignment);
            var parsetree = new Program(statementlist);

            var symbolTable = symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree);
            Assert.That(symbolTable.resolve("foo"), Is.InstanceOf<Symbol>());
            Assert.That(symbolTable.resolve("foo").Type, Is.EqualTo("int"));
        }

        [Test]
        public void ArithmeticOperationTest()
        {
            var integer = new IntegerLiteral("9");
            var plus = new ArithmeticOp("+", integer, integer);
            var times = new ArithmeticOp("*", plus, integer);
            var div = new ArithmeticOp("/", integer, integer);
            var minus = new ArithmeticOp("-", times, div);
            var variable = new VariableDeclaration("foo", "int");
            var assignment = new Assignment(variable, minus);
            statementlist.Add(assignment);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void AssertionTest()
        {
            var integer = new IntegerLiteral("1");
            var equal = new LogicalOp("=", integer, integer);
            var assertion = new ExpressionStatement("assert", equal);
            statementlist.Add(assertion);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void LogicalOpEquals()
        {
            var integer = new IntegerLiteral("1");
            var stringlit = new StringLiteral("\"foobar\"");
            var equal = new LogicalOp("=", integer, stringlit);
            var assert = new ExpressionStatement("assert", equal);
            statementlist.Add(assert);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void LogicalOpAnd()
        {
            var integer = new IntegerLiteral("1");
            var stringlit = new StringLiteral("\"foobar\"");
            var equal = new LogicalOp("=", integer, stringlit);
            var and = new LogicalOp("&", equal, equal);
            var assert = new ExpressionStatement("assert", and);
            statementlist.Add(assert);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void UnaryNot()
        {
            var integer = new IntegerLiteral("42");
            var and = new LogicalOp("=", integer, integer);
            var not = new UnaryNot(and);
            var assert = new ExpressionStatement("assert", and);
            statementlist.Add(assert);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }
    }

    [TestFixture]
    class TestSemanticErrors
    {
        List<Statement> statementlist;
        TypeCheckingVisitor symbolTableBuilder;

        [SetUp]
        public void SetUp()
        {
            statementlist = new List<Statement>();
            symbolTableBuilder = new TypeCheckingVisitor();
        }

        [Test]
        public void UndefinedVariable()
        {
            var variable = new VariableReference("foo");
            var integer = new IntegerLiteral("42");
            var assignment = new Assignment(variable, integer);
            statementlist.Add(assignment);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void FaultyArithmetic()
        {
            var variable = new VariableDeclaration("foo", "int");
            var integer = new IntegerLiteral("42");
            var stringlit = new StringLiteral("\"foobar\"");
            var plus = new ArithmeticOp("+", integer, integer);
            var times = new ArithmeticOp("*", stringlit, plus);
            var assignment = new Assignment(variable, times);
            statementlist.Add(assignment);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void StringArithmetic()
        {
            var stringlit = new StringLiteral("\"foobar\"");
            var plus = new ArithmeticOp("+", stringlit, stringlit);
            var print = new ExpressionStatement("print", plus);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void IntegerOverflow()
        {
            var integer = new IntegerLiteral("9999999999999999999999999999");
            var print = new ExpressionStatement("print", integer);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void NonBooleanArgumentsToAnd()
        {
            var integer = new IntegerLiteral("5");
            var equal = new LogicalOp("=", integer, integer);
            var and = new LogicalOp("&", equal, integer);
            var assert = new ExpressionStatement("assert", and);
            statementlist.Add(assert);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void IntegerArgumentToNot()
        {
            var integer = new IntegerLiteral("5");
            var not = new UnaryNot(integer);
            var assert = new ExpressionStatement("assert", not);
            statementlist.Add(assert);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void StringArgumentToNot()
        {
            var stringlit = new StringLiteral("\"foobar\"");
            var not = new UnaryNot(stringlit);
            var assert = new ExpressionStatement("assert", not);
            statementlist.Add(assert);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }
    }

    [TestFixture]
    class LoopTest
    {
        List<Statement> statementlist;
        TypeCheckingVisitor symbolTableBuilder;

        [SetUp]
        public void SetUp()
        {
            statementlist = new List<Statement>();
            symbolTableBuilder = new TypeCheckingVisitor();
        }

        [Datapoint]
        string stringtype = "string";

        [Datapoint]
        string booltype = "bool";

        [Theory]
        public void NonIntegerLoopVariable(string type)
        {
            var variabledecl = new VariableDeclaration("foo", type);
            statementlist.Add(variabledecl);
            var variable = new VariableReference("foo");
            var integer = new IntegerLiteral("5");
            var range = new Range(integer, integer);
            var variabledecl2 = new VariableDeclaration("bar", "int");
            var loopbody = new List<Statement>();
            loopbody.Add(variabledecl2);
            var loop = new Loop(variable, range, loopbody);
            statementlist.Add(loop);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Theory]
        public void SeveralVariableDeclarationsBeforeLoop(string type)
        {
            var intdeclaration = new VariableDeclaration("foo", "int");
            var otherdeclaration = new VariableDeclaration("bar", type);
            var variableref = new VariableReference("bar");
            var integer = new IntegerLiteral("5");
            var range = new Range(integer, integer);
            var loop = new Loop(variableref, range, new List<Statement>());
            statementlist.Add(intdeclaration);
            statementlist.Add(otherdeclaration);
            statementlist.Add(loop);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void ValidLoop()
        {
            var integer1 = new IntegerLiteral("1");
            var integer2 = new IntegerLiteral("5");
            var range = new Range(integer1, integer2);
            var variabledecl = new VariableDeclaration("foo", "int");
            var variable = new VariableReference("foo");
            var loop = new Loop(variable, range, new List<Statement>());
            statementlist.Add(variabledecl);
            statementlist.Add(loop);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void InvalidRange()
        {
            var stringlit = new StringLiteral("foo");
            var range = new Range(stringlit, stringlit);
            var variabledecl = new VariableDeclaration("foo", "int");
            var variable = new VariableReference("foo");
            var loop = new Loop(variable, range, new List<Statement>());
            statementlist.Add(variabledecl);
            statementlist.Add(loop);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }
    }

    [TestFixture]
    class SymbolTableTest
    {
        [Test]
        public void TestDefine()
        {
            var table = new SymbolTable();
            var variable = new Symbol("foo", "bar");
            table.define(variable);
            Assert.That(table.resolve("foo").Type, Is.EqualTo("bar"));
        }

        [Test]
        public void TestResolveUnknown()
        {
            var table = new SymbolTable();
            Assert.That(table.resolve("foo"), Is.Null);
        }
    }
}
