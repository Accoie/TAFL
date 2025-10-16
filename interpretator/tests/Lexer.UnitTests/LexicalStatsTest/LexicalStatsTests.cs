using System.Text;

namespace Lexer.UnitTests.LexicalStatsTest;

public class LexicalStatsTests
{
    [Fact]
    public void CollectFromFile_WithSimpleProgram_ReturnsCorrectStats()
    {
        string testFile = "test_program.txt";
        string programCode = "НАЧАЛО\n    МОЛВИ \"Введите первое число: \"\n    ВНЕМЛИ num1\n    \n    МОЛВИ \"Введите второе число: \"\n    ВНЕМЛИ num2\n    \n    сумма = num1 + num2\n    МОЛВИ \"Сумма: \" сумма\nИСХОД";

        FileStream fileStream = CreateTempFile(testFile, programCode);
        string result = LexicalStats.CollectFromFile(testFile);
        Dictionary<string, int> stats = ParseStats(result);

        Assert.Equal(7, stats["keywords"]);
        Assert.Equal(6, stats["identifier"]);
        Assert.Equal(0, stats["number literals"]);
        Assert.Equal(3, stats["string literals"]);
        Assert.Equal(2, stats["operators"]);
        Assert.True(stats["other lexemes"] == 0);
    }

    [Fact]
    public void CollectFromFile_WithComplexProgram_ReturnsCorrectStats()
    {
        string testFile = "complex_program.txt";
        string programCode = "НАЧАЛО\n    ЧИСЛО число1 = 42\n    ЧИСЛО число2 = 3.14\n    СЛОВО имя = \"Иван\"\n    БУЛЕВО флаг = ИСТИНА\n    \n    ЕЖЕЛИ число1 > 0 ТВОРИ\n        МОЛВИ( \"Положительное число: \"), число1;\n    ИНО\n        МОЛВИ(\"Отрицательное число\");\n    ИСХОД\n    \n    ДЛЯ i ОТ 1 ДО 10 ТВОРИ\n        ЕЖЕЛИ i % 2 == 0 ТВОРИ\n            ПРОДОЛЖИТЬ;\n        ИСХОД\n        МОЛВИ(i);\n    ИСХОД\nИСХОД";

        FileStream fileStream = CreateTempFile(testFile, programCode);
        string result = LexicalStats.CollectFromFile(testFile);
        Dictionary<string, int> stats = ParseStats(result);

        Assert.Equal(22, stats["keywords"]);
        Assert.Equal(9, stats["identifier"]);
        Assert.Equal(7, stats["number literals"]);
        Assert.Equal(3, stats["string literals"]);
        Assert.Equal(7, stats["operators"]);
        Assert.True(stats["other lexemes"] == 12);
    }

    [Fact]
    public void CollectFromFile_WithEmptyFile_ReturnsZeroStats()
    {
        string testFile = "empty.txt";

        FileStream fileStream = CreateTempFile(testFile, "");
        string result = LexicalStats.CollectFromFile(testFile);
        Dictionary<string, int> stats = ParseStats(result);

        Assert.Equal(0, stats["keywords"]);
        Assert.Equal(0, stats["identifier"]);
        Assert.Equal(0, stats["number literals"]);
        Assert.Equal(0, stats["string literals"]);
        Assert.Equal(0, stats["operators"]);
        Assert.Equal(0, stats["other lexemes"]);
    }

    [Fact]
    public void CollectFromFile_WithNumbersAndOperators_ReturnsCorrectStats()
    {
        string testFile = "math.txt";
        string programCode = "НАЧАЛО\n    результат = (5 + 3) * 2 - 10 / 2\n    МОЛВИ \"Результат: \" результат\nИСХОД";

        FileStream fileStream = CreateTempFile(testFile, programCode);
        string result = LexicalStats.CollectFromFile(testFile);
        Dictionary<string, int> stats = ParseStats(result);

        Assert.Equal(3, stats["keywords"]);
        Assert.Equal(2, stats["identifier"]);
        Assert.Equal(5, stats["number literals"]);
        Assert.Equal(1, stats["string literals"]);
        Assert.Equal(5, stats["operators"]);
    }

    [Fact]
    public void CollectFromFile_WithComments_IgnoresCommentsInStats()
    {
        string testFile = "with_comments.txt";
        string programCode = "НАЧАЛО\n    ## Это однострочный комментарий\n    ЧИСЛО x = 10 ## Комментарий после кода\n    /# \n        Это многострочный комментарий  \n    #/\n    МОЛВИ \"Результат: \" x\nИСХОД";

        FileStream fileStream = CreateTempFile(testFile, programCode);
        string result = LexicalStats.CollectFromFile(testFile);
        Dictionary<string, int> stats = ParseStats(result);

        Assert.Equal(4, stats["keywords"]);
        Assert.Equal(2, stats["identifier"]);
        Assert.Equal(1, stats["number literals"]);
        Assert.Equal(1, stats["string literals"]);
        Assert.Equal(1, stats["operators"]);
    }

    [Fact]
    public void CollectFromFile_WithBooleanLogic_ReturnsCorrectStats()
    {
        string testFile = "boolean.txt";
        string programCode = "НАЧАЛО\n    БУЛЕВО a = ИСТИНА\n    БУЛЕВО b = ЛОЖЬ\n    ЕЖЕЛИ a @ b ТВОРИ\n        МОЛВИ \"Оба истинны\"\n    ИНО ЕЖЕЛИ a || b ТВОРИ\n        МОЛВИ \"Хотя бы один истинен\"\n    ИСХОД\nИСХОД";

        FileStream fileStream = CreateTempFile(testFile, programCode);
        string result = LexicalStats.CollectFromFile(testFile);
        Dictionary<string, int> stats = ParseStats(result);

        Assert.Equal(12, stats["keywords"]);
        Assert.Equal(6, stats["identifier"]);
        Assert.Equal(0, stats["number literals"]);
        Assert.Equal(2, stats["string literals"]);
        Assert.Equal(4, stats["operators"]);
        Assert.Equal(2, stats["other lexemes"]);
    }

    [Fact]
    public void CollectFromFile_WithEscapeSequences_InStringLiterals()
    {
        string testFile = "escape.txt";
        string programCode = "НАЧАЛО\n    СЛОВО текст = \"Привет\\нМир\\отаб\\квозврат\"\n    МОЛВИ текст\nИСХОД";

        FileStream fileStream = CreateTempFile(testFile, programCode);
        string result = LexicalStats.CollectFromFile(testFile);
        Dictionary<string, int> stats = ParseStats(result);

        Assert.Equal(4, stats["keywords"]);
        Assert.Equal(2, stats["identifier"]);
        Assert.Equal(0, stats["number literals"]);
        Assert.Equal(1, stats["string literals"]);
        Assert.Equal(1, stats["operators"]);
    }

    private Dictionary<string, int> ParseStats(string statsResult)
    {
        return statsResult.Split('\n')
            .Select(line => line.Split(':'))
            .ToDictionary(parts => parts[0].Trim(), parts => int.Parse(parts[1].Trim()));
    }

    private FileStream CreateTempFile(string fileName, string content)
    {
        using (StreamWriter writer = new(fileName, false, Encoding.Default))
        {
            writer.Write(content);
        }

        return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}