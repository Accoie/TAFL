using Execution;

using Reqnroll;

namespace Interpreter.Specs;

/// <summary>
/// Имитирует поведение консоли.
/// </summary>
public class TestEnvironment : IEnvironment
{
    private readonly Queue<decimal> inputQueue = new Queue<decimal>();

    public string Output { get; private set; } = string.Empty;

    public void SetInputValues(params decimal[] values)
    {
        inputQueue.Clear();
        foreach (decimal value in values)
        {
            inputQueue.Enqueue(value);
        }
    }

    public void SetInputFromTable(Table table)
    {
        inputQueue.Clear();
        foreach (DataTableRow? row in table.Rows)
        {
            if (decimal.TryParse(row["Value"], out decimal value))
            {
                inputQueue.Enqueue(value);
            }
        }
    }

    public decimal ReadNumber()
    {
        if (inputQueue.Count > 0)
        {
            return inputQueue.Dequeue();
        }

        throw new InvalidOperationException("Нет данных для ввода. Проверьте настройку тестовых данных.");
    }

    public void WriteLine()
    {
        Output += Environment.NewLine;
    }

    public void WriteNumber(decimal result)
    {
        string formatted = result % 1 == 0 ?
            result.ToString("0") :
            result.ToString("0.00");
        Output += formatted;
    }

    public void WriteString(string str)
    {
        Output += str;
    }

    public void ClearOutput()
    {
        Output = string.Empty;
    }
}