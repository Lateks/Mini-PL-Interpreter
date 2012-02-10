using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LexicalAnalyser
{
    public class Lexer
    {
        private Queue<char> input;

        public Lexer(string input)
        {
            this.input = new Queue<char>(input);
        }

        public string NextToken()
        {
            SkipWhiteSpace();
            if (!InputLeft())
                return null;
            if (input.Peek().Equals('.'))
                return MakeDotDotToken();
            else if (Char.IsDigit(input.Peek()))
                return MakeIntegerConstantToken();
            else if (Char.IsLetter(input.Peek()))
                return MakeIdentifierToken();
            else if (input.Peek().Equals('"'))
                return MakeStringConstantToken();
            else
                throw new LexicalError("Invalid token or not implemented yet.");
        }

        private void SkipWhiteSpace()
        {
            while (InputLeft() && Char.IsWhiteSpace(input.Peek()))
                input.Dequeue();
        }

        private bool InputLeft()
        {
            return input.Count > 0;
        }

        private string MakeDotDotToken()
        {
            string token = "" + input.Dequeue();
            if (InputLeft() && input.Peek().Equals('.'))
                return token + input.Dequeue();
            else
                throw new LexicalError("Invalid token \".\"");
        }

        private string MakeIntegerConstantToken()
        {
            string token = "";
            while (InputLeft() && Char.IsDigit(input.Peek()))
                token += input.Dequeue();
            return token;
        }

        private string MakeIdentifierToken()
        {
            string token = "";
            while (InputLeft() && (Char.IsLetterOrDigit(input.Peek()) ||
                                   input.Peek().Equals('_')))
                token += input.Dequeue();
            return token;
        }

        private string MakeStringConstantToken()
        {
            string token = "" + input.Dequeue();
            while (InputLeft() && !(input.Peek().Equals('"')))
                if (input.Peek().Equals('\\'))
                    token += GetEscapeCharacter();
                else
                    token += input.Dequeue();
            if (!InputLeft())
                throw new LexicalError("Reached end of input while scanning for string literal.");
            return token + input.Dequeue();
        }

        private string GetEscapeCharacter()
        {
            string token = "" + input.Dequeue();
            if (InputLeft()) // currently accepts anything as an escape character
                return token + input.Dequeue();
            else
                throw new LexicalError("Reached end of input while scanning for string literal.");
        }
    }

    public class LexicalError : System.Exception
    {
        public LexicalError(string message) {}
    }
}