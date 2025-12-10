using ValueType = Runtime.ValueType;

namespace Ast.Statements;

public sealed class FunctionDeclarationStatement : Statement
{
    public FunctionDeclarationStatement(string name, List<Parameter> parameters, BlockStatement body, ValueType type)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
        ResultType = type;
    }

    public string Name { get; }

    public List<Parameter> Parameters { get; }

    public BlockStatement Body { get; }

    public ValueType ResultType { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}