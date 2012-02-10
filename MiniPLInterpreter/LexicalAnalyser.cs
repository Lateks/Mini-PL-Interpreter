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
            {
                token += input.Dequeue();
                return token;
            }
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
    }

    public class LexicalError : System.Exception
    {
        public LexicalError(string message) {}
    }
}