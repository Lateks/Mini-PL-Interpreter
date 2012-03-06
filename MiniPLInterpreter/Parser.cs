using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis;
using TokenTypes;

namespace SyntaxAnalysis
{
    class Parser
    {
        private Scanner scanner;
        private Token input_token;

        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
            this.input_token = null;
        }

        private void GetInputToken()
        {
            if (input_token == null)
                input_token = scanner.NextToken();
        }

        private void Program()
        {
            GetInputToken();
            if (input_token is Keyword || input_token is Identifier)
            {
                Statement();
                Match<EndLine>();
                StatementList();
                Match<EOF>();
            }
        }

        private void Match<T>(string value = null) where T : Token
        {
            if (input_token is T)
                if (value == null || (input_token is ValueToken<string> &&
                    ((ValueToken<string>) input_token).Value == value))
                    ConsumeToken();
        }

        private void ConsumeToken()
        {
            throw new NotImplementedException();
        }

        private void StatementList()
        {
            throw new NotImplementedException();
        }

        private void Statement()
        {
            throw new NotImplementedException();
        }
    }
}
