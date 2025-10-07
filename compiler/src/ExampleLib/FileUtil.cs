using System.Text;

namespace ExampleLib;

public static class FileUtil
{
    /// <summary>
    /// Сортирует строки в указанном файле.
    /// Перезаписывает файл, но не атомарно: ошибка ввода-вывода при записи приведёт к потере данных.
    /// </summary>
    public static void SortFileLines(string path)
    {
        // Читаем и сортируем строки файла.
        List<string> lines = File.ReadLines(path, Encoding.UTF8).ToList();
        lines.Sort();

        // Перезаписываем файл с нуля (режим Truncate).
        using FileStream file = File.Open(path, FileMode.Truncate, FileAccess.Write);
        for (int i = 0, iMax = lines.Count; i < iMax; ++i)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(lines[i]);
            file.Write(bytes);
            if (i != iMax - 1)
            {
                file.Write("\n"u8);
            }
        }
    }

    public static void AddLineNumbers(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty", nameof(path));
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found: {path}");
        }

        List<string> lines = File.ReadLines(path, Encoding.UTF8).ToList();

        using FileStream file = File.Open(path, FileMode.Truncate, FileAccess.Write);
        using StreamWriter writer = new StreamWriter(file, Encoding.UTF8);

        for (int i = 0; i < lines.Count; i++)
        {
            string numberedLine = $"{i + 1}. {lines[i]}";
            writer.Write(numberedLine);

            if (i != lines.Count - 1)
            {
                writer.Write(Environment.NewLine);
            }
        }
    }
}