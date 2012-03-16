using System;
using MiniPLInterpreter.LexicalAnalysis;
using MiniPLInterpreter.SyntaxAnalysis;
using MiniPLInterpreter.SemanticAnalysis;
using MiniPLInterpreter.Interpreter;
using MiniPLInterpreter.Support.AbstractSyntaxTree;
using MiniPLInterpreter.Support.Symbols;
using MiniPLInterpreter.Errors.Interpreter;
using MiniPLInterpreter.Errors.MiniPL;

namespace MiniPLInterpreter
{
    namespace Interpreter
    {
        class Interpreter
        {
            public static void Main(string[] args)
            {
                string source;
                try
                {
                    source = System.IO.File.ReadAllText(args[0]);
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Give the name of the source file as a parameter.");
                    return;
                }
                catch (System.IO.FileNotFoundException)
                {
                    Console.WriteLine("File \"" + args[0] + "\" not found.");
                    return;
                }

                try
                {
                    Parser parser = new Parser(new Scanner(source));
                    Program program = parser.Parse();
                    TypeCheckingVisitor typechecker = new TypeCheckingVisitor();
                    SymbolTable symboltable = typechecker.BuildSymbolTableAndTypeCheck(program);
                    InterpretingNodeVisitor interpreter = new InterpretingNodeVisitor(symboltable);
                    interpreter.Run(program);
                }
                catch (LexicalError e)
                {
                    Console.WriteLine("Lexical error:\n" + e.Message + "\n");
                }
                catch (SyntaxError e)
                {
                    Console.WriteLine("Syntax error:\n" + e.Message + "\n");
                }
                catch (SemanticError e)
                {
                    Console.WriteLine("Semantic error:\n" + e.Message + "\n");
                }
                catch (MiniPLAssertionFailed e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (MiniPLReadError e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
