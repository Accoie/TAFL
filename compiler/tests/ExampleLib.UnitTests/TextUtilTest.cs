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

    [Fact]
    public void Can_parse_simple_roman_numerals()
    {
        Assert.Equal(1, TextUtil.ParseRoman("I"));
        Assert.Equal(5, TextUtil.ParseRoman("V"));
        Assert.Equal(10, TextUtil.ParseRoman("X"));
        Assert.Equal(50, TextUtil.ParseRoman("L"));
        Assert.Equal(100, TextUtil.ParseRoman("C"));
        Assert.Equal(500, TextUtil.ParseRoman("D"));
        Assert.Equal(1000, TextUtil.ParseRoman("M"));
    }

    [Fact]
    public void Can_parse_roman_numerals_with_addition()
    {
        Assert.Equal(2, TextUtil.ParseRoman("II"));
        Assert.Equal(3, TextUtil.ParseRoman("III"));
        Assert.Equal(7, TextUtil.ParseRoman("VII"));
        Assert.Equal(8, TextUtil.ParseRoman("VIII"));
        Assert.Equal(2025, TextUtil.ParseRoman("MMXXV"));
    }

    [Fact]
    public void Can_parse_roman_numerals_with_subtraction()
    {
        Assert.Equal(4, TextUtil.ParseRoman("IV"));
        Assert.Equal(9, TextUtil.ParseRoman("IX"));
        Assert.Equal(40, TextUtil.ParseRoman("XL"));
        Assert.Equal(90, TextUtil.ParseRoman("XC"));
        Assert.Equal(400, TextUtil.ParseRoman("CD"));
        Assert.Equal(900, TextUtil.ParseRoman("CM"));
        Assert.Equal(1990, TextUtil.ParseRoman("MCMXC"));
    }

    [Fact]
    public void Can_parse_roman_numerals_case_insensitive()
    {
        Assert.Equal(4, TextUtil.ParseRoman("iv"));
        Assert.Equal(9, TextUtil.ParseRoman("ix"));
        Assert.Equal(14, TextUtil.ParseRoman("xiv"));
        Assert.Equal(1990, TextUtil.ParseRoman("mcmxc"));
    }

    [Fact]
    public void Can_parse_zero_representations()
    {
        Assert.Equal(0, TextUtil.ParseRoman("nulla"));
        Assert.Equal(0, TextUtil.ParseRoman("NULLA"));
        Assert.Equal(0, TextUtil.ParseRoman("0"));
    }

    [Fact]
    public void Can_parse_maximum_roman_number()
    {
        Assert.Equal(3000, TextUtil.ParseRoman("MMM"));
    }

    [Fact]
    public void Can_handle_invalid_roman_numerals()
    {
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman("IIII"));
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman("VV"));
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman("IL"));
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman("A"));
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman("MMMI"));
    }

    [Fact]
    public void Can_handle_empty_or_null_input()
    {
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(""));
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(null));
    }
}