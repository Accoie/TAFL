namespace Ast.Statements;

public class BlockStatement : Statement
{
    public BlockStatement(List<AstNode> statements)
    {
        Statements = statements;
    }

    public List<AstNode> Statements { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}