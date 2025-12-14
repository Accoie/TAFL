using ValueType = Runtime.ValueType;

namespace Ast.Statements;

public sealed class FunctionDeclarationStatement : AbstractFunctionDeclaration
{
    public FunctionDeclarationStatement(string name, List<ParameterDeclaration> parameters, BlockStatement body, ValueType type)
        : base(name, parameters)
    {
        Body = body;
        ResultType = type;
    }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}