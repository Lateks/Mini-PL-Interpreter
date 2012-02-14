﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using TokenTypes;

namespace LexicalAnalyser
{
    public class Lexer
    {
        private Stack<char> input;
        private int row, col;
        public int Row
        {
            get { return row; }
        }
        public int Col
        {
            get { return col; }
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
            {"var", "for", "end", "in", "do", "read", "print", "int",
             "string", "bool", "assert"});

        public Lexer(string input)
        {
            var inputchars = input.ToArray();
            Array.Reverse(inputchars);
            this.input = new Stack<char>(inputchars);
            row = 1; col = 0;
        }

        // Implements an ad-hoc procedural scanner.
        public Token NextToken()
        {
            SkipWhiteSpaceAndComments();
            if (!InputLeft())
                return null;
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
                    "\" on row " + row.ToString() + ", col " + col.ToString() + ".");
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
                row++;
                col = 0;
            }
            else
                col++;
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

        private Operator MakeOperatorToken()
        {
            return new Operator(PopInput());
        }

        private Token MakeSymbolToken()
        {
            string token = PopInput();
            if (token.Equals(ENDLINE))
                return new EndLine();
            else if (token.Equals(LEFT_PAREN))
                return new LeftParenthesis();
            else
                return new RightParenthesis();
        }

        private Operator MakeDotDotToken()
        {
            string token = PopInput();
            if (InputLeft() && input.Peek().Equals('.'))
                return new Operator(token + PopInput());
            else
                throw new LexicalError("Invalid token \".\" on row " +
                    row.ToString() + ", col " + col.ToString() + ".");
        }

        private Operator MakeColonOrAssignmentToken()
        {
            string token = PopInput();
            if (InputLeft() && input.Peek().Equals('='))
                token += PopInput();
            return new Operator(token);
        }

        private IntegerLiteral MakeIntegerLiteralToken()
        {
            string token = "";
            while (InputLeft() && Char.IsDigit(input.Peek()))
                token += PopInput();
            return new IntegerLiteral(Convert.ToInt32(token));
        }

        private Token MakeIdentifierOrKeywordToken()
        {
            string token = "";
            while (InputLeft() && (Char.IsLetterOrDigit(input.Peek()) ||
                                   input.Peek().Equals('_')))
                token += PopInput();
            if (keywords.Contains(token))
                return new Keyword(token);
            return new Identifier(token);
        }

        private StringLiteral MakeStringLiteralToken()
        {
            string token = PopInput();
            while (InputLeft() && !(input.Peek().Equals('"')))
                if (input.Peek().Equals('\\'))
                    token += GetEscapeCharacter();
                else
                    token += PopInput();
            if (!InputLeft())
                throw new LexicalError("Reached end of input while scanning for a string literal.");
            return new StringLiteral(token + PopInput());
        }

        private string GetEscapeCharacter()
        {
            string token = PopInput();
            if (InputLeft()) // currently accepts anything as an escape character
                return token + PopInput();
            else
                throw new LexicalError("Reached end of input while scanning for a string literal.");
        }
    }

    public class LexicalError : System.Exception
    {
        public LexicalError(string message) {}
    }
}