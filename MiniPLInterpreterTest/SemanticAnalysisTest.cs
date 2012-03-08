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
            var variable = new VariableDeclaration("foo", "int");
            var integer = new IntegerLiteral("9");
            var plus = new ArithmeticOp("+", integer, integer);
            var times = new ArithmeticOp("*", plus, integer);
            var div = new ArithmeticOp("/", integer, integer);
            var minus = new ArithmeticOp("-", times, div);
            var assignment = new Assignment(variable, minus);
            statementlist.Add(assignment);
            var parsetree = new Program(statementlist);

            var symbolTable = symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree);
            Assert.That(symbolTable.resolve("foo"), Is.InstanceOf<Symbol>());
            Assert.That(symbolTable.resolve("foo").Type, Is.EqualTo("int"));
        }

        [Test]
        public void PrintString()
        {
            var variabledecl = new VariableDeclaration("foo", "string");
            statementlist.Add(variabledecl);
            var variable = new VariableReference("foo");
            var print = new ExpressionStatement(new Keyword("print"), variable);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            var symbolTable = symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree);
            Assert.That(symbolTable.resolve("foo"), Is.InstanceOf<Symbol>());
            Assert.That(symbolTable.resolve("foo").Type, Is.EqualTo("string"));
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
            var print = new ExpressionStatement(new Keyword("print"), plus);
            statementlist.Add(print);
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
