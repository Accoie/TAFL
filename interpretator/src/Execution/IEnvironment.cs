namespace Execution;

/// <summary>
/// Представляет окружение для выполнения программы.
/// Прежде всего это функции ввода/вывода.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// Вызывается после вычисления результата очередной инструкции программы.
    /// </summary>
    public void WriteNumber(decimal result);

    public void WriteLine();

    public void WriteString(string str);

    public decimal ReadNumber();
}