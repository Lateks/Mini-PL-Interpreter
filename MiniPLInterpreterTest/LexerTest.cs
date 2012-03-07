using System;
using NUnit.Framework;
using LexicalAnalysis;
using TokenTypes;
using Errors;

namespace LexerTest
{
    [TestFixture]
    public class LexerTests
    {
        [Datapoints]
        public string[] keywords = {"var", "for", "end", "in", "do", "read",
                                       "print", "int", "string", "bool", "assert"};

        [Theory]
        public void Keywords(string input)
        {
            var lexer = new Scanner(input);
            Token next = lexer.NextToken();
            Assert.That(next, Is.InstanceOf<Keyword>());
            Assert.That(((Keyword)next).Value, Is.EqualTo(input));
        }

        [Test]
        public void IntegerConstants()
        {
            var lexer = new Scanner("123");
            Assert.That(((IntegerLiteral) lexer.NextToken()).Value, Is.EqualTo("123"));
            Assert.That(lexer.NextToken(), Is.InstanceOf<EOF>());
            lexer = new Scanner("1 23");
            var token = (IntegerLiteral) lexer.NextToken();
            Assert.That(token.Value, Is.EqualTo("1"));
            Assert.That(token.Row, Is.EqualTo(1));
            Assert.That(token.Col, Is.EqualTo(1));
            Assert.That(((IntegerLiteral) lexer.NextToken()).Value, Is.EqualTo("23"));
        }

        [Test]
        public void DoubleDots()
        {
            var lexer = new Scanner("..");
            Assert.That(lexer.NextToken(), Is.InstanceOf<RangeOperator>());
            lexer = new Scanner(".");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
            Assert.That(lexer.Col, Is.EqualTo(1));
        }

        [Test]
        public void Identifiers()
        {
            var lexer = new Scanner("42foo");
            Assert.That(((IntegerLiteral) lexer.NextToken()).Value, Is.EqualTo("42"));
            Token next = lexer.NextToken();
            Assert.That(next, Is.InstanceOf<Identifier>());
            Assert.That(((Identifier) next).Value, Is.EqualTo("foo"));
            lexer = new Scanner("f_o12a");
            Assert.That(((Identifier) lexer.NextToken()).Value, Is.EqualTo("f_o12a"));
        }

        [Test]
        public void StringLiterals()
        {
            var lexer = new Scanner("\"foo 42\"");
            Assert.That(((StringLiteral) lexer.NextToken()).Value, Is.EqualTo("\"foo 42\""));
            Assert.That(lexer.NextToken(), Is.InstanceOf<EOF>());
            lexer = new Scanner("\"foo\\n\"");
            Assert.That(((StringLiteral) lexer.NextToken()).Value, Is.EqualTo("\"foo\\n\""));
            lexer = new Scanner("\"foo\\\"...\\\"\"");
            Assert.That(((StringLiteral) lexer.NextToken()).Value, Is.EqualTo("\"foo\\\"...\\\"\""));
            lexer = new Scanner("\"foo\\\"");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
            lexer = new Scanner("\"foo\\");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
        }

        [Test]
        public void WhiteSpaceIsSkipped()
        {
            var lexer = new Scanner("\n\t\v\n  foo");
            Assert.That(((Identifier) lexer.NextToken()).Value, Is.EqualTo("foo"));
        }

        [Test]
        public void CommentsAreSkipped()
        {
            var lexer = new Scanner("// ... \n // ... \n foo");
            var token = (Identifier) lexer.NextToken();
            Assert.That(token.Value, Is.EqualTo("foo"));
            Assert.That(token.Row, Is.EqualTo(3));
            Assert.That(token.Col, Is.EqualTo(4));
            lexer = new Scanner("/* ... \n\n*/ \tfoo");
            token = (Identifier) lexer.NextToken();
            Assert.That(token.Value, Is.EqualTo("foo"));
            Assert.That(token.Row, Is.EqualTo(3));
            Assert.That(token.Col, Is.EqualTo(7));
            lexer = new Scanner("\n\n// ...//\n// ... \n\n/* ... */ foo");
            Assert.That(((Identifier) lexer.NextToken()).Value, Is.EqualTo("foo"));
        }

        [Test]
        public void CombinedWhiteSpaceAndComments()
        {
            var lexer = new Scanner("\n\t\t// ... \n // ... \n     foo");
            Assert.That(((Identifier) lexer.NextToken()).Value, Is.EqualTo("foo"));
        }

        [Test]
        public void InputConsistingOfWhitespaceOnly()
        {
            var lexer = new Scanner("\n   ");
            Assert.That(lexer.NextToken(), Is.InstanceOf<EOF>());
        }

        [Test]
        public void DivisionSymbolIsNotConfusedWithAComment()
        {
            var lexer = new Scanner("/");
            Assert.That(lexer.NextToken(), Is.InstanceOf<BinaryOperator>());
            lexer = new Scanner("// .. / ..\n /");
            Assert.That(((BinaryOperator) lexer.NextToken()).Value, Is.EqualTo("/"));
            Assert.That(lexer.Col, Is.EqualTo(2));
            Assert.That(lexer.Row, Is.EqualTo(2));
        }

        [Test]
        public void Operators()
        {
            var lexer = new Scanner("+ = !");
            Assert.That(((BinaryOperator) lexer.NextToken()).Value, Is.EqualTo("+"));
            Assert.That(((BinaryOperator) lexer.NextToken()).Value, Is.EqualTo("="));
            Assert.That(lexer.NextToken(), Is.InstanceOf<UnaryNot>());
        }

        [Test]
        public void AssignmentAndColon()
        {
            var lexer = new Scanner(":");
            Assert.That(((BinaryOperator) lexer.NextToken()).Value, Is.EqualTo(":"));
            lexer = new Scanner(":=");
            Assert.That(((BinaryOperator) lexer.NextToken()).Value, Is.EqualTo(":="));
        }

        [Test]
        public void ShouldBeInvalid()
        {
            var lexer = new Scanner("$");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
            Assert.That(lexer.Col, Is.EqualTo(1));
        }

        [Test]
        public void Statement()
        {
            var lexer = new Scanner("var x : int := 4 + (2 * \"foo\");");
            Assert.That(((Keyword) lexer.NextToken()).Value, Is.EqualTo("var"));
            Assert.That(((Identifier) lexer.NextToken()).Value, Is.EqualTo("x"));
            Assert.That(((BinaryOperator) lexer.NextToken()).Value, Is.EqualTo(":"));
            Assert.That(((Keyword) lexer.NextToken()).Value, Is.EqualTo("int"));
            Assert.That(((BinaryOperator) lexer.NextToken()).Value, Is.EqualTo(":="));
            Assert.That(((IntegerLiteral) lexer.NextToken()).Value, Is.EqualTo("4"));
            Assert.That(((BinaryOperator) lexer.NextToken()).Value, Is.EqualTo("+"));
            Assert.That(lexer.NextToken(), Is.InstanceOf<LeftParenthesis>());
            Assert.That(((IntegerLiteral) lexer.NextToken()).Value, Is.EqualTo("2"));
            Assert.That(((BinaryOperator) lexer.NextToken()).Value, Is.EqualTo("*"));
            Assert.That(((StringLiteral) lexer.NextToken()).Value, Is.EqualTo("\"foo\""));
            Assert.That(lexer.NextToken(), Is.InstanceOf<RightParenthesis>());
            Assert.That(lexer.NextToken(), Is.InstanceOf<EndLine>());
        }
    }
}
