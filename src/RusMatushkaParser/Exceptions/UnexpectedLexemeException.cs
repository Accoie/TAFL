namespace RusMatushkaParser.Exceptions;

[Serializable]
#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
public class UnexpectedLexemeException : Exception
{
    public UnexpectedLexemeException(TokenType expected, Token actual)
        : base($"Unexpected lexeme {actual.Type} where expected {expected}")
    {
    }

    public UnexpectedLexemeException(Token actual)
    : base($"Unexpected lexeme {actual.Type}")
    {
    }
}
#pragma warning restore RCS1194