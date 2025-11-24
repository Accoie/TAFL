using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

namespace Ast;

public interface IAstVisitor
{
    public void Visit(BinaryOperationExpression e);

    public void Visit(UnaryOperationExpression e);

    public void Visit(LiteralExpression e);

    public void Visit(VariableExpression e);

    public void Visit(FunctionCallExpression s );

    void Visit(AssignmentStatement s);

    public void Visit(IfElseStatement s);

    public void Visit(ForLoopStatement s);

    public void Visit(InputStatement s);

    public void Visit(OutputStatement s);

    public void Visit(BlockStatement s);

    void Visit(ReturnStatement s);

    public void Visit(VariableDeclaration d);

    public void Visit(FunctionDeclaration d);
}