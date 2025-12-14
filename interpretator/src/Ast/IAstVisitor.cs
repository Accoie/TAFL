using Ast.Expressions;
using Ast.Statements;

namespace Ast;

public interface IAstVisitor
{
    void Visit(BinaryOperationExpression e);

    void Visit(UnaryOperationExpression e);

    void Visit(LiteralExpression e);

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

    void Visit(WhileLoopStatement s);

    void Visit(BreakStatement s);

    void Visit(ContinueStatement s);

    void Visit(FunctionCallStatement s);

    void Visit(ParameterDeclaration parameterDeclarationStatement);

    void Visit(VariableExpression variableExpression);

    void Visit(IteratorDeclaration iteratorDeclaration);
}