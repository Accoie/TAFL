namespace Ast.Statements;

public class BlockStatement : Statement
{
    public BlockStatement(List<Statement> statements, bool isBlock)
    {
        Statements = statements;
        IsNewScope = isBlock;
    }

    public List<Statement> Statements { get; }

    public bool IsNewScope { get; set; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}