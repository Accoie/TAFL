using Ast.Expressions;
using Ast.Statements;

namespace Ast.Declarations;

public sealed class FunctionDeclaration : Declaration
{
    public FunctionDeclaration(string name, List<string> parameters, BlockStatement body)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
    }

    public string Name { get; }

    public List<string> Parameters { get; }

    public BlockStatement Body { get; }

    public Expression? Result { get; set; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}