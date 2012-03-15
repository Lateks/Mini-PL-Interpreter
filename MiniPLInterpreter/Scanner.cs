using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using TokenTypes;
using Errors;

namespace LexicalAnalysis
{
    public class Scanner
    {
        private Stack<char> input;
        public int Row
        {
            get;
            private set;
        }
        public int Col
        {
            get;
            private set;
        }
        private static char ENDLINE = ';';
        private static char LEFT_PAREN = '(';
        private static char RIGHT_PAREN = ')';
        private static HashSet<char> symbols =
            new HashSet<char>(new char[] { ENDLINE, LEFT_PAREN, RIGHT_PAREN });
        private static HashSet<char> operators =
            new HashSet<char>(new char[]
            {'/', '+', '-', '*', '<', '=', '&', '!'});
        private static HashSet<string> keywords =
            new HashSet<string>(new string[]
            {"var", "for", "end", "in", "do", "read", "print", "assert"});
        private static HashSet<string> types =
            new HashSet<string>(new string[] { "int", "string", "bool" });

        public Scanner(string input)
        {
            var inputchars = input.ToArray();
            Array.Reverse(inputchars);
            this.input = new Stack<char>(inputchars);
            Row = 1; Col = 0;
        }

        // Implements an ad-hoc procedural scanner.
        public Token NextToken()
        {
            SkipWhiteSpaceAndComments();
            if (!InputLeft())
                return new EOF(Row, Col);
            if (input.Peek().Equals('.'))
                return MakeDotDotToken();
            else if (input.Peek().Equals(':'))
                return MakeColonOrAssignmentToken();
            else if (operators.Contains(input.Peek()))
                return MakeOperatorToken();
            else if (symbols.Contains(input.Peek()))
                return MakeSymbolToken();
            else if (Char.IsDigit(input.Peek()))
                return MakeIntegerLiteralToken();
            else if (Char.IsLetter(input.Peek()))
                return MakeIdentifierOrKeywordToken();
            else if (input.Peek().Equals('"'))
                return MakeStringLiteralToken();
            else
                throw new LexicalError("Invalid token \"" + PopInput() +
                    "\" on row " + Row.ToString() + ", col " + Col.ToString() + ".");
        }

        private void SkipWhiteSpaceAndComments()
        {
            while (InputLeft())
            {
                SkipWhiteSpace();
                bool no_comments_skipped = !SkipComments();
                if ((no_comments_skipped && !NextCharIsWhiteSpace()))
                    return;
            }
        }

        private void SkipWhiteSpace()
        {
            while (NextCharIsWhiteSpace())
                PopInput();
        }

        private bool NextCharIsWhiteSpace()
        {
            return (InputLeft() && Char.IsWhiteSpace(input.Peek()));
        }

        // Returns true if something was skipped and false otherwise.
        private bool SkipComments()
        {
            try
            {
                if (!InputLeft() || !input.Peek().Equals('/'))
                    return false;

                char symbol = input.Pop();
                if (InputLeft() && input.Peek().Equals('/'))
                    SkipOneLineComment();
                else if (InputLeft() && input.Peek().Equals('*'))
                    SkipMultilineComment();
                else
                {
                    input.Push(symbol);
                    return false;
                }
                return true;
            }
            catch (InvalidOperationException)
            {
                throw new LexicalError("Reached end of input while scanning for a comment.");
            }
        }

        private void SkipOneLineComment() {
            ReadUntil('\n');
        }

        private void SkipMultilineComment()
        {
            PopInput();
            while (true)
            {
                ReadUntil('*');
                if (input.Peek().Equals('/'))
                {
                    PopInput();
                    return;
                }
            }
        }

        private string PopInput()
        {
            char symbol = input.Pop();
            if (symbol.Equals('\n'))
            {
                Row++;
                Col = 0;
            }
            else
                Col++;
            return symbol.ToString();
        }

        private void ReadUntil(char symbol)
        {
            while (!input.Peek().Equals(symbol))
                PopInput();
            PopInput();
        }

        private bool InputLeft()
        {
            return input.Count > 0;
        }

        private Token MakeOperatorToken()
        {
            if (input.Peek().Equals('!'))
            {
                PopInput();
                return new UnaryNotToken(Row, Col);
            }
            return new BinaryOperator(PopInput(), Row, Col);
        }

        private Token MakeSymbolToken()
        {
            string token = PopInput();
            if (token.Equals(ENDLINE.ToString()))
                return new EndLine(Row, Col);
            else if (token.Equals(LEFT_PAREN.ToString()))
                return new LeftParenthesis(Row, Col);
            else
                return new RightParenthesis(Row, Col);
        }

        private RangeOperator MakeDotDotToken()
        {
            PopInput();
            if (InputLeft() && input.Peek().Equals('.'))
            {
                PopInput();
                return new RangeOperator(Row, Col);
            }
            else
                throw new LexicalError("Invalid token \".\" on row " +
                    Row.ToString() + ", col " + Col.ToString() + ".");
        }

        private Token MakeColonOrAssignmentToken()
        {
            string token = PopInput();
            if (InputLeft() && input.Peek().Equals('='))
            {
                token += PopInput();
                return new AssignmentToken(Row, Col);
            }
            return new TypeDeclaration(Row, Col);
        }

        private IntegerLiteralToken MakeIntegerLiteralToken()
        {
            string token = "";
            while (InputLeft() && Char.IsDigit(input.Peek()))
                token += PopInput();
            return new IntegerLiteralToken(token, Row, Col);
        }

        private Token MakeIdentifierOrKeywordToken()
        {
            string token = "";
            while (InputLeft() && (Char.IsLetterOrDigit(input.Peek()) ||
                                   input.Peek().Equals('_')))
                token += PopInput();
            if (types.Contains(token))
                return new TokenTypes.Type(token, Row, Col);
            if (keywords.Contains(token))
                return new KeywordToken(token, Row, Col);
            return new Identifier(token, Row, Col);
        }

        private StringLiteralToken MakeStringLiteralToken()
        {
            PopInput(); // discard delimiting quotes
            string token = "";
            while (InputLeft() && !(input.Peek().Equals('"')))
                if (input.Peek().Equals('\\'))
                    token += GetEscapeCharacter();
                else
                    token += PopInput();
            if (!InputLeft())
                throw new LexicalError("Reached end of input while scanning for a string literal.");
            PopInput();
            return new StringLiteralToken(token, Row, Col);
        }

        private string GetEscapeCharacter()
        {
            PopInput(); // discard '\'
            if (InputLeft()) // currently accepts anything as an escape character
            {
                string token = PopInput();
                switch (token)
                {
                    case "\\":
                        return token;
                    case "\"":
                        return token;
                    case "n":
                        return "\n";
                    case "r":
                        return "\r";
                    case "t":
                        return "\t";
                    case "v":
                        return "\v";
                    default:
                        throw new LexicalError("Unknown escape character \\" + token + ".");
                }
            }
            else
                throw new LexicalError("Reached end of input while scanning for a string literal.");
        }
    }
}