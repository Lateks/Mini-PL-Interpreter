﻿using System;
using NUnit.Framework;
using LexicalAnalyser;

namespace LexerTest
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void IntegerConstantTest()
        {
            var lexer = new Lexer("123");
            Assert.That(lexer.NextToken(), Is.EqualTo("123"));
            lexer = new Lexer("1 23");
            Assert.That(lexer.NextToken(), Is.EqualTo("1"));
            Assert.That(lexer.NextToken(), Is.EqualTo("23"));
        }

        [Test]
        public void DotDotTest()
        {
            var lexer = new Lexer("..");
            Assert.That(lexer.NextToken(), Is.EqualTo(".."));
            lexer = new Lexer(".");
            Assert.Throws<LexicalError>(() => lexer.NextToken());
        }

        [Test]
        public void IdentifierTest()
        {
            var lexer = new Lexer("for");
            Assert.That(lexer.NextToken(), Is.EqualTo("for"));
            lexer = new Lexer("42for");
            Assert.That(lexer.NextToken(), Is.EqualTo("42"));
            lexer = new Lexer("f_o12a");
            Assert.That(lexer.NextToken(), Is.EqualTo("f_o12a"));
        }

        [Test]
        public void StringConstantTest()
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
    }
}
