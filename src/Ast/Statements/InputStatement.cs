namespace Ast.Statements;

public class InputStatement : Statement
{
    public InputStatement(string variableName)
    {
        VariableName = variableName;
    }

    public string VariableName { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}