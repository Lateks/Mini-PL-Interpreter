using System;
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
    }
}
