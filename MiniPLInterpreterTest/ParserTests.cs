﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using MiniPLInterpreter.SyntaxAnalysis;
using MiniPLInterpreter.LexicalAnalysis;
using MiniPLInterpreter.Support.AbstractSyntaxTree;
using MiniPLInterpreter.Errors.Interpreter;

namespace MiniPLInterpreterTest
{
    [TestFixture]
    class VariableDeclarationTests
    {
        [Datapoints]
        public string[] types = {"int", "bool", "string"};

        [Theory]
        public void ValidVariableDeclarationTypes(string type)
        {
            string program = "var foo : " + type + ";";
            Parser parser = new Parser(new Scanner(program));
            Program tree = parser.Parse();
            Assert.That(tree.Children[0], Is.InstanceOf<VariableDeclaration>());
            VariableDeclaration var = (VariableDeclaration)tree.Children[0];
            Assert.That(var.Name, Is.EqualTo("foo"));
            Assert.That(var.Type, Is.EqualTo(type));
        }

        [Test]
        public void VariableDeclarationWithoutTypeFails()
        {
            string program = "var foo;";
            Parser parser = new Parser(new Scanner(program));
            Assert.Throws<SyntaxError>(() => parser.Parse());

            program = "var foo := 0;";
            parser = new Parser(new Scanner(program));
            Assert.Throws<SyntaxError>(() => parser.Parse());
        }

        [Test]
        public void VariableDeclarationWithUnknownType()
        {
            string program = "var foo : mytype;";
            Parser parser = new Parser(new Scanner(program));
            Assert.Throws<SyntaxError>(() => parser.Parse());
        }
    }

    [TestFixture]
    class KeywordTests
    {
        [Datapoints]
        public string[] types = {"var", "assert", "for", "int", "bool", "string",
                                    "in", "do", "read", "print", "end"};

        [Theory]
        public void KeywordInVariableDeclaration(string keyword)
        {
            string program = "var " + keyword + " : int;";
            Parser parser = new Parser(new Scanner(program));
            Assert.Throws<SyntaxError>(() => parser.Parse());
        }

        [Theory]
        public void KeywordAsVariableName(string keyword)
        {
            string program = keyword + " := 0;";
            Parser parser = new Parser(new Scanner(program));
            Assert.Throws<SyntaxError>(() => parser.Parse());
        }
    }

    [TestFixture]
    class ParserTests
    {
        [Test]
        public void MissingSemicolon()
        {
            string program = "foo := 0";
            Parser parser = new Parser(new Scanner(program));
            Assert.Throws<SyntaxError>(() => parser.Parse());
        }

        [Test]
        public void WholeProgram()
        {
            string program = "var nTimes : int := 0;\n" +
                             "print \"How many times?\";\n" +
                             "read nTimes;\n" +
                             "var x : int;\n" +
                             "for x in 0..nTimes-1 do\n" +
                             "\tprint x;\n" +
                             "\tprint \" : Hello, World!\n\";\n" +
                             "end for;\n" +
                             "assert (x = nTimes);";
            Parser parser = new Parser(new Scanner(program));
            Program tree = parser.Parse();
            Assert.That(tree.Children.Count, Is.EqualTo(6));
        }

        [Test]
        public void PrintLiteral()
        {
            string program = "print \"How many times?\";";
            Parser parser = new Parser(new Scanner(program));
            Program tree = parser.Parse();
            ExpressionStatement printstat = (ExpressionStatement)tree.Children[0];

            Assert.That(printstat.Keyword, Is.EqualTo("print"));
            Assert.That(printstat.Expression, Is.InstanceOf<StringLiteral>());
            Assert.That(((StringLiteral)printstat.Expression).Value, Is.EqualTo("How many times?"));
        }

        [Test]
        public void UnaryNot()
        {
            string program = "assert(!foobar);";
            Parser parser = new Parser(new Scanner(program));
            Program tree = parser.Parse();
            ExpressionStatement assert = (ExpressionStatement)tree.Children[0];
            Assert.That(assert.Expression, Is.InstanceOf<UnaryNot>());
            Assert.That(((VariableReference)((UnaryNot)assert.Expression).Operand).Name, Is.EqualTo("foobar"));
        }

        [Test]
        public void ArithmeticOperator()
        {
            string program = "foo := 1 + 2;";
            Parser parser = new Parser(new Scanner(program));
            Program tree = parser.Parse();
            Assignment assignment = (Assignment)tree.Children[0];
            Assert.That(assignment.Expression, Is.InstanceOf<BinaryOp>());
            ArithmeticOp plus = (ArithmeticOp)assignment.Expression;
            Assert.That(plus.OpSymbol, Is.EqualTo("+"));
            Assert.That(((IntegerLiteral)plus.LeftOp).Value, Is.EqualTo("1"));
            Assert.That(((IntegerLiteral)plus.RightOp).Value, Is.EqualTo("2"));
        }

        [Test]
        public void Assertion()
        {
            string program = "assert(foo);";
            Parser parser = new Parser(new Scanner(program));
            Program tree = parser.Parse();
            ExpressionStatement assertion = (ExpressionStatement)tree.Children[0];

            Assert.That(assertion.Keyword, Is.EqualTo("assert"));
            Assert.That(((VariableReference)assertion.Expression).Name, Is.EqualTo("foo"));
        }

        [Test]
        public void ReadVariable()
        {
            string program = "read foo;";
            Parser parser = new Parser(new Scanner(program));
            Program tree = parser.Parse();
            ReadStatement readstat = (ReadStatement)tree.Children[0];

            Assert.That(readstat.Variable, Is.InstanceOf<VariableReference>());
            Assert.That(((VariableReference)readstat.Variable).Name, Is.EqualTo("foo"));
        }

        [Test]
        public void ReadOnlyAcceptsAVariable()
        {
            string program = "read \"foo\";";
            Parser parser = new Parser(new Scanner(program));
            Assert.Throws<SyntaxError>(() => parser.Parse());

            program = "read 10;";
            parser = new Parser(new Scanner(program));
            Assert.Throws<SyntaxError>(() => parser.Parse());
        }

        [Test]
        public void VariableDeclarationAndAssignment()
        {
            string program = "var foo12 : int := 0;";
            Parser parser = new Parser(new Scanner(program));
            Program rootnode = parser.Parse();
            List<Statement> nodes = rootnode.Children;
            Assert.That(nodes.Count, Is.EqualTo(1));

            Assert.That(nodes[0], Is.InstanceOf<Assignment>());
            Assignment assignment = (Assignment)nodes[0];
            Assert.That(assignment.Variable, Is.InstanceOf<VariableDeclaration>());
            Assert.That(((VariableDeclaration)assignment.Variable).Name, Is.EqualTo("foo12"));
            Assert.That(((VariableDeclaration)assignment.Variable).Type, Is.EqualTo("int"));
            Assert.That(assignment.Expression, Is.InstanceOf<IntegerLiteral>());
            Assert.That(((IntegerLiteral)assignment.Expression).Value, Is.EqualTo("0"));
        }

        [Test]
        public void Assignment()
        {
            string program = "foo := 0;";
            Parser parser = new Parser(new Scanner(program));
            Program rootnode = parser.Parse();
            Assert.That(rootnode.Children[0], Is.InstanceOf<Assignment>());
            Assignment assignment = (Assignment)rootnode.Children[0];

            Assert.That(((VariableReference)assignment.Variable).Name, Is.EqualTo("foo"));
            Assert.That(assignment.Expression, Is.InstanceOf<IntegerLiteral>());
            Assert.That(((IntegerLiteral)assignment.Expression).Value, Is.EqualTo("0"));
        }

        [Test]
        public void ForLoop()
        {
            string program = "for i in 0..length-1 do\n" +
                             "\tprint x;\n" +
                             "\tprint 1 + 1;\n" +
                             "end for;";
            Parser parser = new Parser(new Scanner(program));
            Program rootnode = parser.Parse();
            Assert.That(rootnode.Children.Count, Is.EqualTo(1));
            Loop forloop = (Loop)rootnode.Children[0];

            Assert.That(((VariableReference)forloop.Variable).Name, Is.EqualTo("i"));
            Assert.That(((IntegerLiteral)((Range) forloop.Range).Begin).Value, Is.EqualTo("0"));
            BinaryOp minus = (BinaryOp)((Range) forloop.Range).End;
            Assert.That(minus.OpSymbol, Is.EqualTo("-"));
            Assert.That(((VariableReference)minus.LeftOp).Name, Is.EqualTo("length"));
            Assert.That(((IntegerLiteral)minus.RightOp).Value, Is.EqualTo("1"));
            Assert.That(forloop.LoopBody.Count, Is.EqualTo(2));
        }
    }
}
