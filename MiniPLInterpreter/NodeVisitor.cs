using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AST;

namespace MiniPlInterpreter
{
    public interface NodeVisitor
    {
        void visit(Program node);
        void visit(IntegerLiteral node);
        void visit(StringLiteral node);
        void visit(VariableDeclaration node);
        void visit(Variable node);
        void visit(Keyword node);
        void visit(BinaryOp node);
        void visit(UnaryNot node);
        void visit(Loop node);
        void visit(Range node);
        void visit(Assignment node);
        void visit(ExpressionStatement node);
        void visit(ReadStatement node);
    }
}
