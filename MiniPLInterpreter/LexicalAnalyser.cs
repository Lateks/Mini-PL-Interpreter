using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LexicalAnalyser
{
    public class Lexer
    {
        private Stack<char> input;

        public Lexer(string input)
        {
            var inputchars = input.ToArray();
            Array.Reverse(inputchars);
            this.input = new Stack<char>(inputchars);
        }

        public string NextToken()
        {
            SkipWhiteSpaceAndComments();
            if (!InputLeft())
                return null;
            if (input.Peek().Equals('.'))
                return MakeDotDotToken();
            else if (input.Peek().Equals('/'))
                return input.Pop().ToString();
            else if (Char.IsDigit(input.Peek()))
                return MakeIntegerLiteralToken();
            else if (Char.IsLetter(input.Peek()))
                return MakeIdentifierToken();
            else if (input.Peek().Equals('"'))
                return MakeStringLiteralToken();
            else
                throw new LexicalError("Invalid token or not implemented yet.");
        }

        private void SkipWhiteSpaceAndComments()
        {
            while (true)
            {
                SkipWhiteSpace();
                bool skipped = SkipComments();
                if ((!skipped && !NextCharIsWhiteSpace()) || !InputLeft())
                    return;
            }
        }

        private void SkipWhiteSpace()
        {
            while (NextCharIsWhiteSpace())
                input.Pop();
        }

        private bool NextCharIsWhiteSpace()
        {
            return (InputLeft() && Char.IsWhiteSpace(input.Peek()));
        }

        private bool SkipComments()
        {
            try
            {
                if (!input.Peek().Equals('/'))
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
                throw new LexicalError("Reached end of file while scanning for a comment.");
            }
        }

        private void SkipOneLineComment() {
            while (!input.Peek().Equals('\n'))
                input.Pop();
        }

        private void SkipMultilineComment()
        {
            input.Pop();
            while (true)
            {
                while (!input.Peek().Equals('*'))
                    input.Pop();
                input.Pop();
                if (input.Peek().Equals('/'))
                {
                    input.Pop();
                    return;
                }
            }
        }

        private bool InputLeft()
        {
            return input.Count > 0;
        }

        private string MakeDotDotToken()
        {
            string token = "" + input.Pop();
            if (InputLeft() && input.Peek().Equals('.'))
                return token + input.Pop();
            else
                throw new LexicalError("Invalid token \".\"");
        }

        private string MakeIntegerLiteralToken()
        {
            string token = "";
            while (InputLeft() && Char.IsDigit(input.Peek()))
                token += input.Pop();
            return token;
        }

        private string MakeIdentifierToken()
        {
            string token = "";
            while (InputLeft() && (Char.IsLetterOrDigit(input.Peek()) ||
                                   input.Peek().Equals('_')))
                token += input.Pop();
            return token;
        }

        private string MakeStringLiteralToken()
        {
            string token = "" + input.Pop();
            while (InputLeft() && !(input.Peek().Equals('"')))
                if (input.Peek().Equals('\\'))
                    token += GetEscapeCharacter();
                else
                    token += input.Pop();
            if (!InputLeft())
                throw new LexicalError("Reached end of input while scanning for a string literal.");
            return token + input.Pop();
        }

        private string GetEscapeCharacter()
        {
            string token = "" + input.Pop();
            if (InputLeft()) // currently accepts anything as an escape character
                return token + input.Pop();
            else
                throw new LexicalError("Reached end of input while scanning for a string literal.");
        }
    }

    public class LexicalError : System.Exception
    {
        public LexicalError(string message) {}
    }
}