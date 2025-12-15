using Ast;
using Ast.Expressions;
using Ast.Statements;

namespace Semantics.Passes;

/// <summary>
/// Базовый класс для проходов по AST с целью вычисления атрибутов и семантических проверок.
/// </summary>
public abstract class AbstractPass : IAstVisitor
{
    public virtual void Visit(LiteralExpression e)
    {
    }

    public virtual void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
    }

    public virtual void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
    }

    public virtual void Visit(VariableExpression e)
    {
    }

    public virtual void Visit(FunctionCallExpression e)
    {
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
        }
    }

    public virtual void Visit(AssignmentStatement s)
    {
        s.Value.Accept(this);
    }

    public virtual void Visit(IfElseStatement s)
    {
        s.Condition.Accept(this);
        s.ThenBranch.Accept(this);
        s.ElseBranch?.Accept(this);
    }

    public virtual void Visit(ForLoopStatement s)
    {
        s.Iterator.Accept(this);
        s.EndValue.Accept(this);
        s.Body.Accept(this);
    }

    public virtual void Visit(WhileLoopStatement s)
    {
        s.Condition.Accept(this);
        s.Body.Accept(this);
    }

    public virtual void Visit(InputStatement s)
    {
    }

    public virtual void Visit(OutputStatement s)
    {
        foreach (Expression arg in s.Arguments)
        {
            arg.Accept(this);
        }
    }

    public virtual void Visit(BlockStatement s)
    {
        foreach (Statement statement in s.Statements)
        {
            statement.Accept(this);
        }
    }

    public virtual void Visit(ReturnStatement s)
    {
        s.Value?.Accept(this);
    }

    public virtual void Visit(VariableDeclarationStatement s)
    {
        s.Value?.Accept(this);
    }

    public virtual void Visit(FunctionDeclarationStatement s)
    {
        foreach (ParameterDeclaration parameter in s.Parameters)
        {
            parameter.Accept(this);
        }

        s.Body.Accept(this);
    }

    public virtual void Visit(BreakStatement s)
    {
    }

    public virtual void Visit(ContinueStatement s)
    {
    }

    public virtual void Visit(FunctionCallStatement s)
    {
        foreach (Expression argument in s.Arguments)
        {
            argument.Accept(this);
        }
    }

    public virtual void Visit(ParameterDeclaration d)
    {
    }

    public virtual void Visit(IteratorDeclaration iteratorDeclaration)
    {
        iteratorDeclaration.StartValue.Accept(this);
    }
}