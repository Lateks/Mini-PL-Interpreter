using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TokenTypes
{
    public class Token
    {
        private int row;
        private int col;

        public int Row
        {
            get { return row; }
        }
        public int Col
        {
            get { return col; }
        }

        public Token(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }

    public interface ValueToken<T>
    {
        T Value
        {
            get;
        }
    }

    public class IntegerLiteral : Token, ValueToken<int>
    {
        private int value;
        public int Value
        {
            get { return value; }
        }

        public IntegerLiteral(int value, int row, int col)
            : base(row, col)
        {
            this.value = value;
        }
    }

    public class StringLiteral : Token, ValueToken<string>
    {
        private string value;
        public string Value
        {
            get { return value; }
        }

        public StringLiteral(string value, int row, int col)
            : base(row, col)
        {
            this.value = value;
        }
    }

    public class StringToken : Token, ValueToken<string>
    {
        private string value;
        public string Value
        {
            get { return value; }
        }

        public StringToken(string name, int row, int col)
            : base(row, col)
        {
            this.value = name;
        }
    }

    public class Identifier : StringToken, ValueToken<string>
    {
        public Identifier(string name, int row, int col)
            : base(name, row, col) { }
    }

    public class Keyword : StringToken
    {
        public Keyword(string name, int row, int col)
            : base(name, row, col) { }
    }

    public class Type : Keyword
    {
        public Type(string name, int row, int col)
            : base(name, row, col) { }        
    }

    public class LeftParenthesis : Token
    {
        public LeftParenthesis(int row, int col)
            : base(row, col) { }
    }

    public class RightParenthesis : Token
    {
        public RightParenthesis(int row, int col)
            : base(row, col) { }
    }

    public class EndLine : Token
    {
        public EndLine(int row, int col)
            : base(row, col) { }
    }

    public class Operator : StringToken, ValueToken<string>
    {
        public Operator(string symbol, int row, int col)
            : base(symbol, row, col) { }
    }

    public class EOF : Token
    {
        public EOF(int row, int col) : base(row, col) { }
    }
}