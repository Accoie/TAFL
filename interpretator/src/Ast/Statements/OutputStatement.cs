namespace Ast.Statements;

public class OutputStatement : Statement
{
    public OutputStatement(List<object> arguments)
    {
        Arguments = arguments;
    }

    public List<object> Arguments { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}