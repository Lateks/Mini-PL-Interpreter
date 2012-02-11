using System;
using NUnit.Framework;
using LexicalAnalyser;

namespace LexerTest
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void IntegerConstants()
        {
            var lexer = new Lexer("123");
            Assert.That(lexer.NextToken(), Is.EqualTo("123"));
            lexer = new Lexer("1 23");
            Assert.That(lexer.NextToken(), Is.EqualTo("1"));
            Assert.That(lexer.Row, Is.EqualTo(1));
            Assert.That(lexer.Col, Is.EqualTo(1));
            Assert.That(lexer.NextToken(), Is.EqualTo("23"));
        }

        [Test]
        public void DoubleDots()
        {
            var lexer = new Lexer("..");
            Assert.That(lexer.NextToken(), Is.EqualTo(".."));
            lexer = new Lexer(".");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
            Assert.That(lexer.Col, Is.EqualTo(1));
        }

        [Test]
        public void Identifiers()
        {
            var lexer = new Lexer("for");
            Assert.That(lexer.NextToken(), Is.EqualTo("for"));
            lexer = new Lexer("42for");
            Assert.That(lexer.NextToken(), Is.EqualTo("42"));
            lexer = new Lexer("f_o12a");
            Assert.That(lexer.NextToken(), Is.EqualTo("f_o12a"));
        }

        [Test]
        public void StringLiterals()
        {
            var lexer = new Lexer("\"foo 42\"");
            Assert.That(lexer.NextToken(), Is.EqualTo("\"foo 42\""));
            lexer = new Lexer("\"foo\\n\"");
            Assert.That(lexer.NextToken(), Is.EqualTo("\"foo\\n\""));
            lexer = new Lexer("\"foo\\\"...\\\"\"");
            Assert.That(lexer.NextToken(), Is.EqualTo("\"foo\\\"...\\\"\""));
            lexer = new Lexer("\"foo\\\"");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
            lexer = new Lexer("\"foo\\");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
        }

        [Test]
        public void CommentsAreSkipped()
        {
            var lexer = new Lexer("// ... \n // ... \n foo");
            Assert.That(lexer.NextToken(), Is.EqualTo("foo"));
            Assert.That(lexer.Row, Is.EqualTo(3));
            Assert.That(lexer.Col, Is.EqualTo(4));
            lexer = new Lexer("/* ... \n\n*/ \tfoo");
            Assert.That(lexer.NextToken(), Is.EqualTo("foo"));
            Assert.That(lexer.Row, Is.EqualTo(3));
            Assert.That(lexer.Col, Is.EqualTo(7));
            lexer = new Lexer("\n\n// ...//\n// ... \n\n/* ... */ foo");
            Assert.That(lexer.NextToken(), Is.EqualTo("foo"));
        }

        [Test]
        public void DivisionSymbolIsNotConfusedWithAComment()
        {
            var lexer = new Lexer("/");
            Assert.That(lexer.NextToken(), Is.EqualTo("/"));
            lexer = new Lexer("// .. / ..\n /");
            Assert.That(lexer.NextToken(), Is.EqualTo("/"));
            Assert.That(lexer.Col, Is.EqualTo(2));
        }

        [Test]
        public void Operators()
        {
            var lexer = new Lexer("+ =");
            Assert.That(lexer.NextToken(), Is.EqualTo("+"));
            Assert.That(lexer.NextToken(), Is.EqualTo("="));
        }

        [Test]
        public void AssignmentAndColon()
        {
            var lexer = new Lexer(":");
            Assert.That(lexer.NextToken(), Is.EqualTo(":"));
            lexer = new Lexer(":=");
            Assert.That(lexer.NextToken(), Is.EqualTo(":="));
        }

        [Test]
        public void ShouldBeInvalid()
        {
            var lexer = new Lexer("$");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
            Assert.That(lexer.Col, Is.EqualTo(1));
        }
    }
}
