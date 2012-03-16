using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniPLInterpreter.Support.AbstractSyntaxTree;
using MiniPLInterpreter.Errors.Interpreter;
using MiniPLInterpreter.Support.Symbols;
using MiniPLInterpreter.Support;

namespace MiniPLInterpreter
{
    namespace TypeCheck
    {
        // This visitor checks that the types of all operands are
        // appropriate and builds a symbol table.
        //
        // Typechecking is implemented using a string stack. Each
        // time an operand (e.g. variable or literal) is encountered,
        // its type in string format ("int", "string" or "bool") is
        // pushed onto the stack. Each time an operator or keyword
        // that takes paremeters is encountered, the right number
        // of type strings is popped out from the top of the stack
        // and checked (e.g. are operands to an arithmetic operator
        // ints, are operands to logical "=" both of the same type
        // etc.). A stack is easy to use here because usually in
        // typechecking the ordering of the operands does not
        // matter very much. Incorrect types cause a SemanticError
        // to be thrown.
        //
        // When building the symbol table, the visitor ensures that
        // no variable is declared twice and that each variable
        // reference actually refers to a variable that is already
        // defined in the symbol table. Otherwise a SemanticError
        // is thrown.
        //
        // Note that for loop bodies are traversed twice to prevent
        // any variable declarations within them, because these are
        // not compatible with singular scope (as defined in the
        // Mini-PL specification). Of course technically for loops
        // that iterate over the loop body only once could allow
        // variable declarations inside the loop body but since
        // these make no sense, I decided to forbid variable
        // declarations within loop bodies altogether.
        public class TypeCheckingVisitor : NodeVisitor
        {
            SymbolTable symboltable;
            Stack<string> operandtypes;

            public TypeCheckingVisitor()
            {
                symboltable = new SymbolTable();
                operandtypes = new Stack<string>();
            }

            public SymbolTable BuildSymbolTableAndTypeCheck(Program node)
            {
                node.accept(this);
                return symboltable;
            }

            public void visit(VariableDeclaration node)
            {
                if (symboltable.resolve(node.Name) != null)
                    throw new SemanticError("Variable " + node.Name +
                        " is already defined (row " + node.Row + ").");
                Symbol symbol = new Symbol(node.Name, node.Type);
                symboltable.define(symbol);
            }

            public void visit(VariableReference node)
            {
                Symbol var = symboltable.resolve(node.Name);
                if (var == null)
                    throw new SemanticError("Reference to undefined identifier "
                        + node.Name + " on row " + node.Row + ".");
                else
                    operandtypes.Push(var.Type);
            }

            public void visit(Loop node)
            {
                if (symboltable.resolve(node.VarName).Type != "int")
                    throw new SemanticError("Loop variable " + node.VarName +
                        " on row " + node.Row + " is not an int.");

                for (int i = 0; i < 2; i++)
                { // Check twice to prevent variable declarations inside the loop body
                    // (would cause problems because of singular scope).
                    foreach (Statement statement in node.LoopBody)
                        statement.accept(this);
                }
            }

            public void visit(ArithmeticOp node)
            {
                string optype1 = operandtypes.Pop();
                string optype2 = operandtypes.Pop();
                if (optype1 == "int" && optype2 == "int")
                    operandtypes.Push("int");
                else
                    throw new SemanticError("Non-integer arguments to arithmetic operator on row " +
                        node.Row + ".");
            }

            public void visit(LogicalOp node)
            {
                string optype1 = operandtypes.Pop();
                string optype2 = operandtypes.Pop();

                switch (node.OpSymbol)
                {
                    case "&":
                        if (optype1 != "bool" || optype2 != "bool")
                            throw new SemanticError("Non-boolean arguments to logical and operator \"&\"" +
                                " on row " + node.Row + ".");
                        break;
                    case "=":
                        if (optype1 != optype2)
                            throw new SemanticError("Logical operator \"=\" cannot be applied to types \"" +
                                optype1 + "\" and \"" + optype2 + "\" on row " + node.Row + ".");
                        break;
                }

                operandtypes.Push("bool");
            }

            public void visit(Range node)
            {
                string optype1 = operandtypes.Pop();
                string optype2 = operandtypes.Pop();
                if (optype1 != "int" || optype2 != "int")
                    throw new SemanticError("Invalid argument types for range operator \"..\" on row " +
                        node.Row + ".");
            }

            public void visit(Assignment node)
            {
                string variableType;
                if (node.Variable is VariableReference)
                    variableType = operandtypes.Pop();
                else // variable declaration types are not pushed into the stack
                    variableType = symboltable.resolve(node.VarName).Type;
                string expressionType = operandtypes.Pop();
                if (variableType != expressionType)
                    throw new SemanticError("Attempting to assign expression of type " +
                        "\"" + expressionType + "\" to variable \"" + node.VarName + "\"" +
                        " of type \"" + variableType + "\" on row " + node.Row + ".");
            }

            public void visit(UnaryNot node)
            {
                if (operandtypes.Pop() == "bool")
                    operandtypes.Push("bool");
                else
                    throw new SemanticError("Invalid argument type for unary not operator \"!\" on row " +
                        node.Row + ".");
            }

            public void visit(ExpressionStatement node)
            {
                string exprType = operandtypes.Pop();
                if (node.Keyword == "assert" && exprType != "bool")
                    throw new SemanticError("Invalid argument type \"" + exprType +
                        "\" for assert statement on row " + node.Row + ".");
                else if (node.Keyword == "print" && exprType == "bool")
                    throw new SemanticError("Invalid argument type \"" + exprType +
                        "\" for print statement on row " + node.Row + ".");
            }

            public void visit(IntegerLiteral node)
            {
                try
                {
                    Convert.ToInt32(node.Value);
                    operandtypes.Push("int");
                }
                catch (OverflowException)
                {
                    throw new SemanticError("Integer overflow on row " + node.Row +
                        ", in literal " + node.Value + ".");
                }
            }

            public void visit(StringLiteral node)
            {
                operandtypes.Push("string");
            }

            public void visit(ReadStatement node)
            {
                string vartype = operandtypes.Pop();
                if (vartype == "bool")
                    throw new SemanticError("Invalid argument type \"" + vartype +
                        "\" for read statement on row " + node.Row + ".");
            }

            public void visit(Program node) { }
        }
    }
}