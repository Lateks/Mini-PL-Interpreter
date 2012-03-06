using System;
using NUnit.Framework;
using LexicalAnalyser;
using TokenTypes;

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
            var lexer = new Lexer(input);
            Token next = lexer.NextToken();
            Assert.That(next, Is.InstanceOf<Keyword>());
            Assert.That(((Keyword)next).Name, Is.EqualTo(input));
        }

        [Test]
        public void IntegerConstants()
        {
            var lexer = new Lexer("123");
            Assert.That(((IntegerLiteral) lexer.NextToken()).Value, Is.EqualTo(123));
            Assert.That(lexer.NextToken(), Is.Null);
            lexer = new Lexer("1 23");
            var token = (IntegerLiteral) lexer.NextToken();
            Assert.That(token.Value, Is.EqualTo(1));
            Assert.That(token.Row, Is.EqualTo(1));
            Assert.That(token.Col, Is.EqualTo(1));
            Assert.That(((IntegerLiteral) lexer.NextToken()).Value, Is.EqualTo(23));
        }

        [Test]
        public void DoubleDots()
        {
            var lexer = new Lexer("..");
            Assert.That(((Operator) lexer.NextToken()).Symbol, Is.EqualTo(".."));
            lexer = new Lexer(".");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
            Assert.That(lexer.Col, Is.EqualTo(1));
        }

        [Test]
        public void Identifiers()
        {
            var lexer = new Lexer("42foo");
            Assert.That(((IntegerLiteral) lexer.NextToken()).Value, Is.EqualTo(42));
            Token next = lexer.NextToken();
            Assert.That(next, Is.InstanceOf<Identifier>());
            Assert.That(((Identifier) next).Name, Is.EqualTo("foo"));
            lexer = new Lexer("f_o12a");
            Assert.That(((Identifier) lexer.NextToken()).Name, Is.EqualTo("f_o12a"));
        }

        [Test]
        public void StringLiterals()
        {
            var lexer = new Lexer("\"foo 42\"");
            Assert.That(((StringLiteral) lexer.NextToken()).Value, Is.EqualTo("\"foo 42\""));
            Assert.That(lexer.NextToken(), Is.Null);
            lexer = new Lexer("\"foo\\n\"");
            Assert.That(((StringLiteral) lexer.NextToken()).Value, Is.EqualTo("\"foo\\n\""));
            lexer = new Lexer("\"foo\\\"...\\\"\"");
            Assert.That(((StringLiteral) lexer.NextToken()).Value, Is.EqualTo("\"foo\\\"...\\\"\""));
            lexer = new Lexer("\"foo\\\"");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
            lexer = new Lexer("\"foo\\");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
        }

        [Test]
        public void WhiteSpaceIsSkipped()
        {
            var lexer = new Lexer("\n\t\v\n  foo");
            Assert.That(((Identifier) lexer.NextToken()).Name, Is.EqualTo("foo"));
        }

        [Test]
        public void CommentsAreSkipped()
        {
            var lexer = new Lexer("// ... \n // ... \n foo");
            var token = (Identifier) lexer.NextToken();
            Assert.That(token.Name, Is.EqualTo("foo"));
            Assert.That(token.Row, Is.EqualTo(3));
            Assert.That(token.Col, Is.EqualTo(4));
            lexer = new Lexer("/* ... \n\n*/ \tfoo");
            token = (Identifier) lexer.NextToken();
            Assert.That(token.Name, Is.EqualTo("foo"));
            Assert.That(token.Row, Is.EqualTo(3));
            Assert.That(token.Col, Is.EqualTo(7));
            lexer = new Lexer("\n\n// ...//\n// ... \n\n/* ... */ foo");
            Assert.That(((Identifier) lexer.NextToken()).Name, Is.EqualTo("foo"));
        }

        [Test]
        public void CombinedWhiteSpaceAndComments()
        {
            var lexer = new Lexer("\n\t\t// ... \n // ... \n     foo");
            Assert.That(((Identifier) lexer.NextToken()).Name, Is.EqualTo("foo"));
        }

        [Test]
        public void InputConsistingOfWhitespaceOnly()
        {
            var lexer = new Lexer("\n   ");
            Assert.That(lexer.NextToken(), Is.Null);
        }

        [Test]
        public void DivisionSymbolIsNotConfusedWithAComment()
        {
            var lexer = new Lexer("/");
            Assert.That(lexer.NextToken(), Is.InstanceOf<Operator>());
            lexer = new Lexer("// .. / ..\n /");
            Assert.That(((Operator) lexer.NextToken()).Symbol, Is.EqualTo("/"));
            Assert.That(lexer.Col, Is.EqualTo(2));
            Assert.That(lexer.Row, Is.EqualTo(2));
        }

        [Test]
        public void Operators()
        {
            var lexer = new Lexer("+ =");
            Assert.That(((Operator) lexer.NextToken()).Symbol, Is.EqualTo("+"));
            Assert.That(((Operator) lexer.NextToken()).Symbol, Is.EqualTo("="));
        }

        [Test]
        public void AssignmentAndColon()
        {
            var lexer = new Lexer(":");
            Assert.That(((Operator) lexer.NextToken()).Symbol, Is.EqualTo(":"));
            lexer = new Lexer(":=");
            Assert.That(((Operator) lexer.NextToken()).Symbol, Is.EqualTo(":="));
        }

        [Test]
        public void ShouldBeInvalid()
        {
            var lexer = new Lexer("$");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
            Assert.That(lexer.Col, Is.EqualTo(1));
        }

        [Test]
        public void Statement()
        {
            var lexer = new Lexer("var x : int := 4 + (2 * \"foo\");");
            Assert.That(((Keyword) lexer.NextToken()).Name, Is.EqualTo("var"));
            Assert.That(((Identifier) lexer.NextToken()).Name, Is.EqualTo("x"));
            Assert.That(((Operator) lexer.NextToken()).Symbol, Is.EqualTo(":"));
            Assert.That(((Keyword) lexer.NextToken()).Name, Is.EqualTo("int"));
            Assert.That(((Operator) lexer.NextToken()).Symbol, Is.EqualTo(":="));
            Assert.That(((IntegerLiteral) lexer.NextToken()).Value, Is.EqualTo(4));
            Assert.That(((Operator) lexer.NextToken()).Symbol, Is.EqualTo("+"));
            Assert.That(lexer.NextToken(), Is.InstanceOf<LeftParenthesis>());
            Assert.That(((IntegerLiteral) lexer.NextToken()).Value, Is.EqualTo(2));
            Assert.That(((Operator) lexer.NextToken()).Symbol, Is.EqualTo("*"));
            Assert.That(((StringLiteral) lexer.NextToken()).Value, Is.EqualTo("\"foo\""));
            Assert.That(lexer.NextToken(), Is.InstanceOf<RightParenthesis>());
            Assert.That(lexer.NextToken(), Is.InstanceOf<EndLine>());
        }
    }
}
