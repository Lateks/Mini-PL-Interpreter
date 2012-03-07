﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis;
using TokenTypes;
using Errors;
using AST;

namespace SyntaxAnalysis
{
    public class Parser
    {
        private Scanner scanner;
        private Token input_token;

        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

        public Node Parse()
        {
            this.input_token = scanner.NextToken();
            return Program();
        }

        private Node Program()
        {
            if (input_token is KeywordToken || input_token is Identifier)
            {
                var root = new Program(StatementList());
                Match<EOF>();
                return root;
            }
            else
                throw new SyntaxError("Program must begin with a valid keyword or an identifier.");
        }

        // Checks that the current input symbol is of token type T
        // and matches value when given. Otherwise a syntax error
        // is thrown.
        private T Match<T>(string value = null) where T : Token
        {
            if (input_token is T)
            {
                if (value == null || (input_token is StringToken &&
                    ((StringToken)input_token).Value == value))
                {
                    var temp = (T)input_token;
                    input_token = scanner.NextToken();
                    return temp;
                }
                else
                    throw new SyntaxError("Expected " + value + " but got " +
                        ((StringToken)input_token).Value + ".");
            }
            else
                throw new SyntaxError("Unexpected token: " +
                    input_token.ToString());
        }

        private List<Node> StatementList()
        {
            var nodes = new List<Node>();
            nodes.Add(Statement());
            Match<EndLine>();
            nodes.AddRange(StatementListTail());
            return nodes;
        }

        private List<Node> StatementListTail()
        {
            // Epsilon production if input token is in the Follow set.
            if (input_token is EOF || (input_token is KeywordToken &&
                ((KeywordToken)input_token).Value == "end"))
                return new List<Node>();
            else
                return StatementList();
        }

        private Node Statement()
        {
            if (input_token is KeywordToken)
            {
                var token = (KeywordToken)input_token;
                switch (token.Value)
                {
                    case "var":
                        input_token = scanner.NextToken();
                        string variable = Identifier();
                        Match<TypeDeclaration>();
                        string type = Type();
                        var var = new VariableDeclaration(variable, type);
                        return OptionalAssignment(var);
                    case "for":
                        input_token = scanner.NextToken();
                        Variable ident = new Variable(Identifier());
                        Match<KeywordToken>("in");
                        Range range = RangeExpr();
                        Match<KeywordToken>("do");
                        var stmts = StatementList();
                        Match<KeywordToken>("end");
                        Match<KeywordToken>("for");
                        Loop loop = new Loop(ident, range, stmts);
                        return loop;
                    case "read":
                        Keyword keyword = new Keyword(token.Value);
                        input_token = scanner.NextToken();
                        ident = new Variable(Identifier());
                        return new Statement(keyword, ident);
                    case "print":
                        keyword = new Keyword(token.Value);
                        input_token = scanner.NextToken();
                        return new Statement(keyword, Expression());
                    case "assert":
                        keyword = new Keyword(token.Value);
                        input_token = scanner.NextToken();
                        Match<LeftParenthesis>();
                        Statement stmt = new Statement(keyword, Expression());
                        Match<RightParenthesis>();
                        return stmt;
                    default:
                        throw new SyntaxError("Invalid keyword " + token.Value + " starting a statement.");
                }
            }
            else
            {
                Identifier token = Match<Identifier>();
                Match<AssignmentToken>();
                return new Assignment(new Variable(token.Value), Expression());
            }
        }

        private Range RangeExpr()
        {
            var range_lhs = Expression();
            Match<RangeOperator>();
            var range_rhs = Expression();
            return new Range(range_lhs, range_rhs);
        }

        private Node Expression()
        {
            if (input_token is UnaryNotToken)
            {
                input_token = scanner.NextToken();
                return new UnaryNot(Operand());
            }
            else
            {
                Node op = Operand();
                return ExpressionTail(op);
            }
        }

        private Node ExpressionTail(Node lhs)
        {
            if (input_token is BinaryOperator)
            {
                BinaryOperator op = Match<BinaryOperator>();
                BinaryOp binop = new BinaryOp(op.Value, lhs, Operand());
                return binop;
            }
            return lhs;
        }

        private Node Operand()
        {
            if (input_token is IntegerLiteralToken)
            {
                IntegerLiteralToken token = Match<IntegerLiteralToken>();
                return new IntegerLiteral(token.Value);
            }
            else if (input_token is StringLiteralToken)
            {
                StringLiteralToken token = Match<StringLiteralToken>();
                return new StringLiteral(token.Value);
            }
            else if (input_token is Identifier)
            {
                Identifier token = Match<Identifier>();
                return new Variable(token.Value);
            }
            else if (input_token is LeftParenthesis)
            {
                input_token = scanner.NextToken();
                Node expr = Expression();
                Match<RightParenthesis>();
                return expr;
            }
            else
                throw new SyntaxError("Invalid operand: " + input_token.ToString());
        }

        private string Identifier()
        {
            Identifier token = Match<Identifier>();
            return token.Value;
        }

        private string Type()
        {
            TokenTypes.Type token = Match<TokenTypes.Type>();
            return token.Value;
        }

        private Node OptionalAssignment(Assignable variable)
        {
            if (input_token is AssignmentToken)
            {
                input_token = scanner.NextToken();
                return new Assignment(variable, Expression());
            }
            // otherwise produce epsilon
            return (Node) variable;
        }
    }
}
