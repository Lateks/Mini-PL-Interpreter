using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis;
using TokenTypes;
using Errors;

namespace SyntaxAnalysis
{
    class Parser
    {
        private Scanner scanner;
        private Token input_token;

        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
            this.input_token = scanner.NextToken();
        }

        private void Program()
        {
            if (input_token is Keyword || input_token is Identifier)
            {
                StatementList();
                Match<EOF>();
            }
            else
                throw new SyntaxError("foobar");
        }

        // Checks that the current input symbol is of token type T
        // and matches value when given. Otherwise a syntax error
        // is thrown.
        private void Match<T>(string value = null) where T : Token
        {
            if (input_token is T)
            {
                if (value == null || (input_token is StringToken &&
                    ((StringToken)input_token).Value == value))
                    input_token = scanner.NextToken();
            }
            else
                throw new SyntaxError("foobar");
        }

        private void StatementList()
        {
            Statement();
            Match<EndLine>();
            StatementListTail();
        }

        private void StatementListTail()
        {
            if (input_token is EOF) // epsilon production
                return;
            else
                StatementList();
        }

        private void Statement()
        {
            if (input_token is Keyword)
            {
                var token = (Keyword)input_token;
                switch (token.Value)
                {
                    case "var":
                        input_token = scanner.NextToken();
                        Identifier();
                        Match<Operator>(":");
                        Type();
                        OptionalAssignment();
                        break;
                    case "for":
                        input_token = scanner.NextToken();
                        Identifier();
                        Match<Keyword>("in");
                        Expression();
                        Match<Operator>("..");
                        Expression();
                        Match<Keyword>("do");
                        StatementList();
                        Match<Keyword>("end");
                        Match<Keyword>("for");
                        break;
                    case "read":
                        input_token = scanner.NextToken();
                        Identifier();
                        break;
                    case "print":
                        Expression();
                        break;
                    case "assert":
                        Match<LeftParenthesis>();
                        Expression();
                        Match<RightParenthesis>();
                        break;
                    default:
                        throw new SyntaxError("foobar");
                }
            }
            else
            {
                var token = (Identifier)input_token;
                // TODO: handle and get next token
            }
        }

        private void Expression()
        {
            if (input_token is Operator)
            {
                Match<Operator>("!");
                Operand();
            }
            else
            {
                Operand();
                ExpressionTail();
            }
        }

        private void ExpressionTail()
        {
            if (input_token is Operator)
            {
                Match<Operator>();
                Operand();
            }
            // otherwise produce epsilon
        }

        private void Operand()
        {
            if (input_token is IntegerLiteral)
                Match<IntegerLiteral>();
            else if (input_token is StringLiteral)
                Match<StringLiteral>();
            else if (input_token is Identifier)
                Match<Identifier>();
            else if (input_token is LeftParenthesis)
            {
                input_token = scanner.NextToken();
                Expression();
                Match<RightParenthesis>();
            }
            else
                throw new SyntaxError("foobar");
        }

        private void Identifier()
        {
            Match<Identifier>();
        }

        private void Type()
        {
            Match<TokenTypes.Type>();
        }

        private void OptionalAssignment()
        {
            if (input_token is Operator && ((Operator)input_token).Value == ":=")
            {
                input_token = scanner.NextToken();
                Expression();
            }
            // otherwise produce epsilon
        }
    }
}
