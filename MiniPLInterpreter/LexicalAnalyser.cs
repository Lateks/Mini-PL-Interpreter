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
    }

    public class LexicalError : System.Exception
    {
        public LexicalError(string message) {}
    }
}