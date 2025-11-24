using Ast;
using Ast.Statements;

namespace RusMatushkaParser;

public class ContinueStatement : Statement
{
    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}