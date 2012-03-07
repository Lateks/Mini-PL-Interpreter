using System;
using System.Collections.Generic;
using NUnit.Framework;
using SyntaxAnalysis;
using LexicalAnalysis;
using AST;
using Errors;

namespace MiniPLInterpreterTest
{
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
        public void PrintLiteral()
        {
            string program = "print \"How many times?\";";
            Parser parser = new Parser(new Scanner(program));
            Program tree = (Program) parser.Parse();
            Statement printstat = (Statement)tree.Children[0];

            Assert.That(((KeywordNode) printstat.Keyword).Name, Is.EqualTo("print"));
            Assert.That(printstat.Expression, Is.InstanceOf<StringLiteralNode>());
            Assert.That(((StringLiteralNode)printstat.Expression).Value, Is.EqualTo("\"How many times?\""));
        }

        [Test]
        public void ReadVariable()
        {
            string program = "read foo;";
            Parser parser = new Parser(new Scanner(program));
            Program tree = (Program)parser.Parse();
            Statement readstat = (Statement)tree.Children[0];

            Assert.That(((KeywordNode)readstat.Keyword).Name, Is.EqualTo("read"));
            Assert.That(readstat.Expression, Is.InstanceOf<Variable>());
            Assert.That(((Variable)readstat.Expression).Name, Is.EqualTo("foo"));
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
        public void Assignment()
        {
            string program = "var foo12 : int := 0;";
            Parser parser = new Parser(new Scanner(program));
            Node AST = parser.Parse();
            Assert.That(AST, Is.InstanceOf<Program>());
            Program rootnode = (Program)AST;
            List<Node> nodes = rootnode.Children;
            Assert.That(nodes.Count, Is.EqualTo(1));

            Assert.That(nodes[0], Is.InstanceOf<Assignment>());
            Assignment assignment = (Assignment)nodes[0];
            Assert.That(assignment.Variable, Is.InstanceOf<VariableDeclaration>());
            Assert.That(((VariableDeclaration)assignment.Variable).Name, Is.EqualTo("foo12"));
            Assert.That(((VariableDeclaration)assignment.Variable).Type, Is.EqualTo("int"));
            Assert.That(assignment.Expression, Is.InstanceOf<IntegerLiteralNode>());
            Assert.That(((IntegerLiteralNode)assignment.Expression).Value, Is.EqualTo("0"));
        }

        [Test]
        public void ForLoop()
        {
            string program = "for i in 0..length-1 do\n" +
                             "\tprint x;\n" +
                             "\tprint 1 + 1;\n" +
                             "end for;";
            Parser parser = new Parser(new Scanner(program));
            Program rootnode = (Program)parser.Parse();
            Assert.That(rootnode.Children.Count, Is.EqualTo(1));
            Loop forloop = (Loop)rootnode.Children[0];

            Assert.That(((Variable)forloop.Variable).Name, Is.EqualTo("i"));
            Assert.That(((IntegerLiteralNode)((Range) forloop.Range).Begin).Value, Is.EqualTo("0"));
            BinaryOp minus = (BinaryOp)((Range) forloop.Range).End;
            Assert.That(minus.OpSymbol, Is.EqualTo("-"));
            Assert.That(((Variable)minus.LeftOp).Name, Is.EqualTo("length"));
            Assert.That(((IntegerLiteralNode)minus.RightOp).Value, Is.EqualTo("1"));
            Assert.That(forloop.LoopBody.Count, Is.EqualTo(2));
        }
    }
}
