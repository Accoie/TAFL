using ExampleLib.UnitTests.Helpers;

using Xunit;

namespace ExampleLib.UnitTests;

public class FileUtilTests
{
    [Fact]
    public void CanSortTextFile()
    {
        const string unsorted = """
                                Играют волны — ветер свищет,
                                И мачта гнется и скрыпит…
                                Увы! он счастия не ищет
                                И не от счастия бежит!
                                """;
        const string sorted = """
                              И мачта гнется и скрыпит…
                              И не от счастия бежит!
                              Играют волны — ветер свищет,
                              Увы! он счастия не ищет
                              """;

        using TempFile file = TempFile.Create(unsorted);
        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal(sorted.Replace("\r\n", "\n"), actual);
    }

    [Fact]
    public void CanSortOneLineFile()
    {
        using TempFile file = TempFile.Create("Играют волны — ветер свищет,");
        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal("Играют волны — ветер свищет,", actual);
    }

    [Fact]
    public void CanSortEmptyFile()
    {
        using TempFile file = TempFile.Create("");

        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal("", actual);
    }

    [Fact]
    public void CanAddLineNumbersToTextFile()
    {
        const string original = """
                                Первая строка
                                Вторая строка
                                Третья строка
                                """;
        const string expected = """
                                1. Первая строка
                                2. Вторая строка
                                3. Третья строка
                                """;

        using TempFile file = TempFile.Create(original);
        FileUtil.AddLineNumbers(file.Path);

        string actual = File.ReadAllText(file.Path).ReplaceLineEndings(Environment.NewLine);
        string normalizedExpected = expected.ReplaceLineEndings(Environment.NewLine);

        Assert.Equal(normalizedExpected, actual);
    }

    [Fact]
    public void CanAddLineNumbersToOneLineFile()
    {
        using TempFile file = TempFile.Create("Одна строка");
        FileUtil.AddLineNumbers(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal("1. Одна строка", actual);
    }

    [Fact]
    public void CanAddLineNumbersToEmptyFile()
    {
        using TempFile file = TempFile.Create("");
        FileUtil.AddLineNumbers(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal("", actual);
    }

    [Fact]
    public void CanHandleInvalidPaths()
    {
        // Arrange
        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => FileUtil.AddLineNumbers("sdf"));
    }
}