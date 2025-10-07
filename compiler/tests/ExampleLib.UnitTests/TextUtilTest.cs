using Xunit;

namespace ExampleLib.UnitTests;

public class TextUtilTest
{
    [Fact]
    public void Can_extract_russian_words()
    {
        const string text = """
                            Играют волны — ветер свищет,
                            И мачта гнётся и скрыпит…
                            Увы! он счастия не ищет
                            И не от счастия бежит!
                            """;
        List<string> expected =
        [
            "Играют",
            "волны",
            "ветер",
            "свищет",
            "И",
            "мачта",
            "гнётся",
            "и",
            "скрыпит",
            "Увы",
            "он",
            "счастия",
            "не",
            "ищет",
            "И",
            "не",
            "от",
            "счастия",
            "бежит",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Can_extract_words_with_hyphens()
    {
        const string text = "Что-нибудь да как-нибудь, и +/- что- то ещё";
        List<string> expected =
        [
            "Что-нибудь",
            "да",
            "как-нибудь",
            "и",
            "что",
            "то",
            "ещё",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Can_extract_words_with_apostrophes()
    {
        const string text = "Children's toys and three cats' toys";
        List<string> expected =
        [
            "Children's",
            "toys",
            "and",
            "three",
            "cats'",
            "toys",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Can_extract_words_with_grave_accent()
    {
        const string text = "Children`s toys and three cats` toys, all of''them are green";
        List<string> expected =
        [
            "Children`s",
            "toys",
            "and",
            "three",
            "cats`",
            "toys",
            "all",
            "of'",
            "them",
            "are",
            "green",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, int> SimpleRomanNumeralsData()
    {
        return new TheoryData<string, int>
        {
            { "I", 1 },
            { "V", 5 },
            { "X", 10 },
            { "L", 50 },
            { "C", 100 },
            { "D", 500 },
            { "M", 1000 },
        };
    }

    [Theory]
    [MemberData(nameof(SimpleRomanNumeralsData))]
    public void Can_parse_simple_roman_numerals(string roman, int expected)
    {
        // Arrange
        // Act
        int result = TextUtil.ParseRoman(roman);

        // Assert
        Assert.Equal(expected, result);
    }

    public static TheoryData<string, int> RomanNumeralsWithAdditionData()
    {
        return new TheoryData<string, int>
        {
            { "II", 2 },
            { "III", 3 },
            { "VII", 7 },
            { "VIII", 8 },
            { "MMXXV", 2025 },
        };
    }

    [Theory]
    [MemberData(nameof(RomanNumeralsWithAdditionData))]
    public void Can_parse_roman_numerals_with_addition(string roman, int expected)
    {
        // Arrange
        // Act
        int result = TextUtil.ParseRoman(roman);

        // Assert
        Assert.Equal(expected, result);
    }

    public static TheoryData<string, int> RomanNumeralsWithSubtractionData()
    {
        return new TheoryData<string, int>
        {
            { "IV", 4 },
            { "IX", 9 },
            { "XL", 40 },
            { "XC", 90 },
            { "CD", 400 },
            { "CM", 900 },
            { "MCMXC", 1990 },
        };
    }

    [Theory]
    [MemberData(nameof(RomanNumeralsWithSubtractionData))]
    public void Can_parse_roman_numerals_with_subtraction(string roman, int expected)
    {
        // Arrange
        // Act
        int result = TextUtil.ParseRoman(roman);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(RomanNumeralsCaseInsensitiveData))]
    public void Can_parse_roman_numerals_case_insensitive(string roman, int expected)
    {
        // Arrange
        // Act
        int result = TextUtil.ParseRoman(roman);

        // Assert
        Assert.Equal(expected, result);
    }

    public static TheoryData<string, int> RomanNumeralsCaseInsensitiveData()
    {
        return new TheoryData<string, int>
        {
            { "iv", 4 },
            { "ix", 9 },
            { "xiv", 14 },
            { "mcmxc", 1990 },
        };
    }

    [Theory]
    [MemberData(nameof(ZeroRepresentationsData))]
    public void Can_parse_zero_representations(string roman, int expected)
    {
        // Arrange
        // Act
        int result = TextUtil.ParseRoman(roman);

        // Assert
        Assert.Equal(expected, result);
    }

    public static TheoryData<string, int> ZeroRepresentationsData()
    {
        return new TheoryData<string, int>
        {
            { "0", 0 },
        };
    }

    [Theory]
    [MemberData(nameof(MaximumRomanNumberData))]
    public void Can_parse_maximum_roman_number(string roman, int expected)
    {
        // Arrange
        // Act
        int result = TextUtil.ParseRoman(roman);

        // Assert
        Assert.Equal(expected, result);
    }

    public static TheoryData<string, int> MaximumRomanNumberData()
    {
        return new TheoryData<string, int>
        {
            { "MMM", 3000 },
        };
    }

    [Theory]
    [MemberData(nameof(InvalidRomanNumeralsData))]
    public void Can_handle_invalid_roman_numerals(string invalidRoman)
    {
        // Arrange
        // Act & Assert
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(invalidRoman));
    }

    public static TheoryData<string> InvalidRomanNumeralsData()
    {
        return new TheoryData<string>
        {
            "IIII",
            "VV",
            "IL",
            "A",
            "MMMI",
        };
    }

    [Fact]
    public void Can_handle_empty_input()
    {
        // Arrange
        string emptyInput = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(emptyInput));
    }
}