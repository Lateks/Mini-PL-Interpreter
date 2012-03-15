using System;
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

        public Program Parse()
        {
            this.input_token = scanner.NextToken();
            return Program();
        }

        private Program Program()
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
                    throw new SyntaxError("Expected \"" + value + "\" but got \"" +
                        ((StringToken)input_token).Value + "\" before col " +
                        input_token.Col + " on row " + input_token.Row + ".");
            }
            else
                throw new SyntaxError("Unexpected token of type " +
                    input_token.GetType().Name + " before col " + input_token.Col
                    + " on row " + input_token.Row + ". Expected token of type " +
                    typeof(T).Name + ".");
        }

        private List<Statement> StatementList()
        {
            var nodes = new List<Statement>();
            nodes.Add(Statement());
            Match<EndLine>();
            nodes.AddRange(StatementListTail());
            return nodes;
        }

        private List<Statement> StatementListTail()
        {
            // Epsilon production if input token is in the Follow set.
            if (input_token is EOF || (input_token is KeywordToken &&
                ((KeywordToken)input_token).Value == "end"))
                return new List<Statement>();
            else
                return StatementList();
        }

        private Statement Statement()
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
                        var var = new VariableDeclaration(variable, type, token.Row);
                        return OptionalAssignment(var);
                    case "for":
                        input_token = scanner.NextToken();
                        VariableReference ident = new VariableReference(Identifier(), token.Row);
                        Match<KeywordToken>("in");
                        Range range = RangeExpr();
                        Match<KeywordToken>("do");
                        var stmts = StatementList();
                        Match<KeywordToken>("end");
                        Match<KeywordToken>("for");
                        return new Loop(ident, range, stmts, token.Row);
                    case "read":
                        input_token = scanner.NextToken();
                        ident = new VariableReference(Identifier(), token.Row);
                        return new ReadStatement(ident, token.Row);
                    case "print":
                        string keyword = token.Value;
                        input_token = scanner.NextToken();
                        return new ExpressionStatement(keyword, Expression(), token.Row);
                    case "assert":
                        keyword = token.Value;
                        input_token = scanner.NextToken();
                        Match<LeftParenthesis>();
                        ExpressionStatement stmt = new ExpressionStatement(keyword, Expression(), token.Row);
                        Match<RightParenthesis>();
                        return stmt;
                    default:
                        throw new SyntaxError("Invalid keyword " + token.Value +
                            " starting a statement before col " + token.Col + " on row " +
                            token.Row + ".");
                }
            }
            else
            {
                Identifier token = Match<Identifier>();
                Match<AssignmentToken>();
                return new Assignment(new VariableReference(token.Value, token.Row), Expression(), token.Row);
            }
        }

        private Range RangeExpr()
        {
            var range_lhs = Expression();
            Match<RangeOperator>();
            var range_rhs = Expression();
            return new Range(range_lhs, range_rhs, range_lhs.Row);
        }

        private Expression Expression()
        {
            if (input_token is UnaryNotToken)
            {
                Token unarynot = input_token;
                input_token = scanner.NextToken();
                return new UnaryNot(Operand(), unarynot.Row);
            }
            else
            {
                Expression op = Operand();
                return ExpressionTail(op);
            }
        }

        private Expression ExpressionTail(Expression lhs)
        {
            if (input_token is BinaryOperator)
            {
                BinaryOperator op = Match<BinaryOperator>();
                if (op.Value == "&" || op.Value == "=")
                    return new LogicalOp(op.Value, lhs, Operand(), op.Row);
                return new ArithmeticOp(op.Value, lhs, Operand(), op.Row);
            }
            return lhs;
        }

        private Expression Operand()
        {
            if (input_token is IntegerLiteralToken)
            {
                IntegerLiteralToken token = Match<IntegerLiteralToken>();
                return new IntegerLiteral(token.Value, token.Row);
            }
            else if (input_token is StringLiteralToken)
            {
                StringLiteralToken token = Match<StringLiteralToken>();
                return new StringLiteral(token.Value, token.Row);
            }
            else if (input_token is Identifier)
            {
                Identifier token = Match<Identifier>();
                return new VariableReference(token.Value, token.Row);
            }
            else if (input_token is LeftParenthesis)
            {
                input_token = scanner.NextToken();
                Expression expr = Expression();
                Match<RightParenthesis>();
                return expr;
            }
            else
                throw new SyntaxError("Invalid operand of type " + input_token.GetType().Name +
                    " before col " + input_token.Col + " on row " + input_token.Row + ".");
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

        private Statement OptionalAssignment(Variable variable)
        {
            if (input_token is AssignmentToken)
            {
                Token assignment = input_token;
                input_token = scanner.NextToken();
                return new Assignment(variable, Expression(), assignment.Row);
            }
            // otherwise produce epsilon
            return (Statement) variable;
        }
    }
}
