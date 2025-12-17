namespace Execution.Exceptions;

[Serializable]
#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.

public class VariableNotFoundException : Exception
{
    public VariableNotFoundException()
    {
    }

    public VariableNotFoundException(string variable)
        : base($"Variable not found: {variable}")
    {
    }
}

#pragma warning restore RCS1194