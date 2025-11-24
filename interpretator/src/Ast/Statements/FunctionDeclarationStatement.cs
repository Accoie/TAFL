using Ast.Expressions;

namespace Ast.Statements;

public sealed class FunctionDeclarationStatement : Statement
{
    public FunctionDeclarationStatement(string name, List<string> parameters, BlockStatement body)
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