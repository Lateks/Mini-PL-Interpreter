using System;
using NUnit.Framework;
using MiniPLInterpreter.LexicalAnalysis;
using MiniPLInterpreter.Support.TokenTypes;
using MiniPLInterpreter.Errors.Interpreter;

namespace LexerTest
{
    [TestFixture]
    public class KeywordTest
    {
        [Datapoints]
        public string[] keywords = {"var", "for", "end", "in", "do", "read",
                                       "print", "int", "string", "bool", "assert"};

        [Theory]
        public void Keywords(string keyword)
        {
            var lexer = new Scanner(keyword);
            Token next = lexer.NextToken();
            Assert.That(next, Is.InstanceOf<KeywordToken>());
            Assert.That(((KeywordToken)next).Value, Is.EqualTo(keyword));
        }
    }

    [TestFixture]
    public class OperatorTest
    {
        [Datapoints]
        public string[] binaryOperators = { "+", "-", "*", "/", "&", "=" };

        [Theory]
        public void BinaryOperators(string binop)
        {
            var lexer = new Scanner(binop);
            Token token = lexer.NextToken();
            Assert.That(token, Is.InstanceOf<BinaryOperator>());
            Assert.That(((BinaryOperator)token).Value, Is.EqualTo(binop));
        }

        [Test]
        public void UnaryNot()
        {
            var lexer = new Scanner("!");
            Assert.That(lexer.NextToken(), Is.InstanceOf<UnaryNotToken>());
        }
    }

    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void IntegerConstants()
        {
            var lexer = new Scanner("123");
            Assert.That(((IntegerLiteralToken) lexer.NextToken()).Value, Is.EqualTo("123"));
            Assert.That(lexer.NextToken(), Is.InstanceOf<EOF>());
            lexer = new Scanner("1 23");
            var token = (IntegerLiteralToken) lexer.NextToken();
            Assert.That(token.Value, Is.EqualTo("1"));
            Assert.That(token.Row, Is.EqualTo(1));
            Assert.That(token.Col, Is.EqualTo(1));
            Assert.That(((IntegerLiteralToken) lexer.NextToken()).Value, Is.EqualTo("23"));
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
            Assert.That(((IntegerLiteralToken) lexer.NextToken()).Value, Is.EqualTo("42"));
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
            Assert.That(((StringLiteralToken) lexer.NextToken()).Value, Is.EqualTo("foo 42"));
            Assert.That(lexer.NextToken(), Is.InstanceOf<EOF>());
            lexer = new Scanner("\"foo\\n\"");
            Assert.That(((StringLiteralToken) lexer.NextToken()).Value, Is.EqualTo("foo\n"));
            lexer = new Scanner("\"foo\\\"...\\\"\"");
            Assert.That(((StringLiteralToken) lexer.NextToken()).Value, Is.EqualTo("foo\"...\""));
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
        public void AssignmentAndColon()
        {
            var lexer = new Scanner(":");
            Assert.That(lexer.NextToken(), Is.InstanceOf<TypeDeclaration>());
            lexer = new Scanner(":=");
            Assert.That(lexer.NextToken(), Is.InstanceOf<AssignmentToken>());
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
            Assert.That(((KeywordToken) lexer.NextToken()).Value, Is.EqualTo("var"));
            Assert.That(((Identifier) lexer.NextToken()).Value, Is.EqualTo("x"));
            Assert.That(lexer.NextToken(), Is.InstanceOf<TypeDeclaration>());
            Assert.That(((KeywordToken) lexer.NextToken()).Value, Is.EqualTo("int"));
            Assert.That(lexer.NextToken(), Is.InstanceOf<AssignmentToken>());
            Assert.That(((IntegerLiteralToken) lexer.NextToken()).Value, Is.EqualTo("4"));
            Assert.That(((BinaryOperator) lexer.NextToken()).Value, Is.EqualTo("+"));
            Assert.That(lexer.NextToken(), Is.InstanceOf<LeftParenthesis>());
            Assert.That(((IntegerLiteralToken) lexer.NextToken()).Value, Is.EqualTo("2"));
            Assert.That(((BinaryOperator) lexer.NextToken()).Value, Is.EqualTo("*"));
            Assert.That(((StringLiteralToken) lexer.NextToken()).Value, Is.EqualTo("foo"));
            Assert.That(lexer.NextToken(), Is.InstanceOf<RightParenthesis>());
            Assert.That(lexer.NextToken(), Is.InstanceOf<EndLine>());
        }
    }
}
