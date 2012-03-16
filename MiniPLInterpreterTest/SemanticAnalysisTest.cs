using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MiniPLInterpreter;
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
            var variabledecl = new VariableDeclaration("foo", type, 0);
            statementlist.Add(variabledecl);
            var variable = new VariableReference("foo", 0);
            var print = new ExpressionStatement("print", variable, 0);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void PrintBool()
        {
            var integer1 = new IntegerLiteral("1", 0);
            var integer2 = new IntegerLiteral("2", 0);
            var equal = new LogicalOp("=", integer1, integer2, 0);
            var print = new ExpressionStatement("print", equal, 0);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Theory]
        public void ValidRead(string type)
        {
            var variabledecl = new VariableDeclaration("foo", type, 0);
            statementlist.Add(variabledecl);
            var variable = new VariableReference("foo", 0);
            var print = new ExpressionStatement("read", variable, 0);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void ReadBool()
        {
            var variabledecl = new VariableDeclaration("foo", "bool", 0);
            statementlist.Add(variabledecl);
            var variable = new VariableReference("foo", 0);
            var print = new ReadStatement(variable, 0);
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
            var variable = new VariableDeclaration("foo", "int", 0);
            var integer = new IntegerLiteral("4", 0);
            var assignment = new Assignment(variable, integer, 0);
            statementlist.Add(assignment);
            var parsetree = new Program(statementlist);

            var symbolTable = symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree);
            Assert.That(symbolTable.resolve("foo"), Is.InstanceOf<Symbol>());
            Assert.That(symbolTable.resolve("foo").Type, Is.EqualTo("int"));
        }

        [Test]
        public void ArithmeticOperationTest()
        {
            var integer = new IntegerLiteral("9", 0);
            var plus = new ArithmeticOp("+", integer, integer, 0);
            var times = new ArithmeticOp("*", plus, integer, 0);
            var div = new ArithmeticOp("/", integer, integer, 0);
            var minus = new ArithmeticOp("-", times, div, 0);
            var variable = new VariableDeclaration("foo", "int", 0);
            var assignment = new Assignment(variable, minus, 0);
            statementlist.Add(assignment);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void AssertionTest()
        {
            var integer = new IntegerLiteral("1", 0);
            var equal = new LogicalOp("=", integer, integer, 0);
            var assertion = new ExpressionStatement("assert", equal, 0);
            statementlist.Add(assertion);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void LogicalOpEquals()
        {
            var integer = new IntegerLiteral("1", 0);
            var stringlit = new StringLiteral("\"foobar\"", 0);
            var equal = new LogicalOp("=", integer, stringlit, 0);
            var assert = new ExpressionStatement("assert", equal, 0);
            statementlist.Add(assert);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void LogicalOpAnd()
        {
            var integer = new IntegerLiteral("1", 0);
            var equal = new LogicalOp("=", integer, integer, 0);
            var and = new LogicalOp("&", equal, equal, 0);
            var assert = new ExpressionStatement("assert", and, 0);
            statementlist.Add(assert);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void UnaryNot()
        {
            var integer = new IntegerLiteral("42", 0);
            var and = new LogicalOp("=", integer, integer, 0);
            var not = new UnaryNot(and, 0);
            var assert = new ExpressionStatement("assert", and, 0);
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
            var variable = new VariableReference("foo", 0);
            var integer = new IntegerLiteral("42", 0);
            var assignment = new Assignment(variable, integer, 0);
            statementlist.Add(assignment);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void VariableDefinitionAndReferenceInAssignment()
        {
            var variableref = new VariableReference("foo", 0);
            var variabledecl = new VariableDeclaration("foo", "int", 0);
            var assignment = new Assignment(variabledecl, variableref, 0);
            statementlist.Add(assignment);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void FaultyArithmetic()
        {
            var variable = new VariableDeclaration("foo", "int", 0);
            var integer = new IntegerLiteral("42", 0);
            var stringlit = new StringLiteral("\"foobar\"", 0);
            var plus = new ArithmeticOp("+", integer, integer, 0);
            var times = new ArithmeticOp("*", stringlit, plus, 0);
            var assignment = new Assignment(variable, times, 0);
            statementlist.Add(assignment);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void StringArithmetic()
        {
            var stringlit = new StringLiteral("\"foobar\"", 0);
            var plus = new ArithmeticOp("+", stringlit, stringlit, 0);
            var print = new ExpressionStatement("print", plus, 0);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void IntegerOverflow()
        {
            var integer = new IntegerLiteral("9999999999999999999999999999", 0);
            var print = new ExpressionStatement("print", integer, 0);
            statementlist.Add(print);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void NonBooleanArgumentsToAnd()
        {
            var integer = new IntegerLiteral("5", 0);
            var equal = new LogicalOp("=", integer, integer, 0);
            var and = new LogicalOp("&", equal, integer, 0);
            var assert = new ExpressionStatement("assert", and, 0);
            statementlist.Add(assert);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void IntegerArgumentToNot()
        {
            var integer = new IntegerLiteral("5", 0);
            var not = new UnaryNot(integer, 0);
            var assert = new ExpressionStatement("assert", not, 0);
            statementlist.Add(assert);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void StringArgumentToNot()
        {
            var stringlit = new StringLiteral("\"foobar\"", 0);
            var not = new UnaryNot(stringlit, 0);
            var assert = new ExpressionStatement("assert", not, 0);
            statementlist.Add(assert);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void InvalidTypesInAssignment()
        {
            var variabledecl = new VariableDeclaration("foo", "int", 0);
            var assignment = new Assignment(variabledecl, new StringLiteral("foo", 0), 0);
            statementlist.Add(assignment);
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
            var variabledecl = new VariableDeclaration("foo", type, 0);
            statementlist.Add(variabledecl);
            var variable = new VariableReference("foo", 0);
            var integer = new IntegerLiteral("5", 0);
            var range = new Range(integer, integer, 0);
            var variabledecl2 = new VariableDeclaration("bar", "int", 0);
            var loopbody = new List<Statement>();
            loopbody.Add(variabledecl2);
            var loop = new Loop(variable, range, loopbody, 0);
            statementlist.Add(loop);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Theory]
        public void SeveralVariableDeclarationsBeforeLoop(string type)
        {
            var intdeclaration = new VariableDeclaration("foo", "int", 0);
            var otherdeclaration = new VariableDeclaration("bar", type, 0);
            var variableref = new VariableReference("bar", 0);
            var integer = new IntegerLiteral("5", 0);
            var range = new Range(integer, integer, 0);
            var loop = new Loop(variableref, range, new List<Statement>(), 0);
            statementlist.Add(intdeclaration);
            statementlist.Add(otherdeclaration);
            statementlist.Add(loop);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void ValidLoop()
        {
            var integer1 = new IntegerLiteral("1", 0);
            var integer2 = new IntegerLiteral("5", 0);
            var range = new Range(integer1, integer2, 0);
            var variabledecl = new VariableDeclaration("foo", "int", 0);
            var variable = new VariableReference("foo", 0);
            var loop = new Loop(variable, range, new List<Statement>(), 0);
            statementlist.Add(variabledecl);
            statementlist.Add(loop);
            var parsetree = new Program(statementlist);

            Assert.DoesNotThrow(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void InvalidRange()
        {
            var stringlit = new StringLiteral("foo", 0);
            var range = new Range(stringlit, stringlit, 0);
            var variabledecl = new VariableDeclaration("foo", "int", 0);
            var variable = new VariableReference("foo", 0);
            var loop = new Loop(variable, range, new List<Statement>(), 0);
            statementlist.Add(variabledecl);
            statementlist.Add(loop);
            var parsetree = new Program(statementlist);

            Assert.Throws<SemanticError>(() => symbolTableBuilder.BuildSymbolTableAndTypeCheck(parsetree));
        }

        [Test]
        public void CannotDeclareVariablesInsideLoop()
        {
            var integer1 = new IntegerLiteral("1", 0);
            var integer2 = new IntegerLiteral("5", 0);
            var range = new Range(integer1, integer2, 0);
            var variabledecl = new VariableDeclaration("foo", "int", 0);
            var variable = new VariableReference("foo", 0);
            var loopbodydecl = new VariableDeclaration("bar", "int", 0);
            var loopbody = new List<Statement>();
            loopbody.Add(loopbodydecl);
            var loop = new Loop(variable, range, loopbody, 0);
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
