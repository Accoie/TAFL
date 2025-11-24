using Ast.Expressions;
using Ast.Statements;

using RusMatushkaParser;

namespace Ast;

public interface IAstVisitor
{
    void Visit(BinaryOperationExpression e);

    void Visit(UnaryOperationExpression e);

    void Visit(LiteralExpression e);

    void Visit(VariableExpression e);

    void Visit(FunctionCallExpression s);

    void Visit(AssignmentStatement s);

    void Visit(IfElseStatement s);

    void Visit(ForLoopStatement s);

    void Visit(InputStatement s);

    void Visit(OutputStatement s);

    void Visit(BlockStatement s);

    void Visit(ReturnStatement s);

    void Visit(VariableDeclarationStatement s);

    void Visit(FunctionDeclarationStatement s);

    void Visit(WhileLoopStatement whileLoopStatement);

    void Visit(BreakStatement breakStatement);

    void Visit(ContinueStatement continueStatement);
}