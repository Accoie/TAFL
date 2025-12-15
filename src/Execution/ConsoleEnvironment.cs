using System.Globalization;

namespace Execution;

public class ConsoleEnvironment : IEnvironment
{
    public bool ReadBool()
    {
        bool.TryParse(Console.ReadLine() ?? "", out bool n);

        return n;
    }

    public decimal ReadNumber()
    {
        decimal.TryParse(Console.ReadLine() ?? "", out decimal n);

        return n;
    }

    public string ReadString()
    {
        return Console.ReadLine() ?? "";
    }

    public void WriteBool(bool b)
    {
        Console.WriteLine(b ? "ИСТИНА" : "ЛОЖЬ");
    }

    public void WriteLine()
    {
        Console.WriteLine();
    }

    public void WriteNumber(decimal result)
    {
        Console.Write(result.ToString("0.#####", CultureInfo.InvariantCulture));
    }

    public void WriteString(string str)
    {
        Console.Write(str);
    }
}