using System.Text;

namespace ExampleLib;

public static class TextUtil
{
    private const int MaxRomanValue = 3000;

    private static readonly IReadOnlyDictionary<char, int> RomanNumerals = new Dictionary<char, int>
        {
            { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 },
            { 'C', 100 }, { 'D', 500 }, { 'M', 1000 },
        };

    // Символы Unicode, которые мы принимаем как дефис.
    private static readonly Rune[] Hyphens = [new Rune('‐'), new Rune('-')];

    // Символы Unicode, которые мы принимаем как апостроф.
    private static readonly Rune[] Apostrophes = [new Rune('\''), new Rune('`')];

    // Состояния распознавателя слов.
    private enum WordState
    {
        NoWord,
        Letter,
        Hyphen,
        Apostrophe,
    }

    /// <summary>
    ///  Распознаёт слова в тексте. Поддерживает Unicode, в том числе английский и русский языки.
    ///  Слово состоит из букв, может содержать дефис в середине и апостроф в середине либо в конце.
    /// </summary>
    /// <remarks>
    ///  Функция использует автомат-распознаватель с четырьмя состояниями:
    ///   1. NoWord — автомат находится вне слова;
    ///   2. Letter — автомат находится в буквенной части слова;
    ///   3. Hyphen — автомат обработал дефис;
    ///   4. Apostrophe — автомат обработал апостроф.
    ///
    ///  Переходы между состояниями:
    ///   - NoWord → Letter — при получении буквы;
    ///   - Letter → Hyphen — при получении дефиса;
    ///   - Letter → Apostrophe — при получении апострофа;
    ///   - Letter → NoWord — при получении любого символа, кроме буквы, дефиса или апострофа;
    ///   - Hyphen → Letter — при получении буквы;
    ///   - Hyphen → NoWord — при получении любого символа, кроме буквы;
    ///   - Apostrophe → Letter — при получении буквы;
    ///   - Apostrophe → NoWord — при получении любого символа, кроме буквы.
    ///
    ///  Разница между состояниями Hyphen и Apostrophe в том, что дефис не может стоять в конце слова.
    /// </remarks>
    public static List<string> ExtractWords(string text)
    {
        WordState state = WordState.NoWord;

        List<string> results = [];
        StringBuilder currentWord = new();
        foreach (Rune ch in text.EnumerateRunes())
        {
            switch (state)
            {
                case WordState.NoWord:
                    if (Rune.IsLetter(ch))
                    {
                        PushCurrentWord();
                        currentWord.Append(ch);
                        state = WordState.Letter;
                    }

                    break;

                case WordState.Letter:
                    if (Rune.IsLetter(ch))
                    {
                        currentWord.Append(ch);
                    }
                    else if (Hyphens.Contains(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Hyphen;
                    }
                    else if (Apostrophes.Contains(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Apostrophe;
                    }
                    else
                    {
                        state = WordState.NoWord;
                    }

                    break;

                case WordState.Hyphen:
                    if (Rune.IsLetter(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Letter;
                    }
                    else
                    {
                        // Убираем дефис, которого не должно быть в конце слова.
                        currentWord.Remove(currentWord.Length - 1, 1);
                        state = WordState.NoWord;
                    }

                    break;

                case WordState.Apostrophe:
                    if (Rune.IsLetter(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Letter;
                    }
                    else
                    {
                        state = WordState.NoWord;
                    }

                    break;
            }
        }

        PushCurrentWord();

        return results;

        void PushCurrentWord()
        {
            if (currentWord.Length > 0)
            {
                results.Add(currentWord.ToString());
                currentWord.Clear();
            }
        }
    }

    public static int ParseRoman(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Пустая строка не является римским числом");
        }

        text = text.Trim().ToUpper();

        if (text == "0")
        {
            return 0;
        }

        ValidateRomanCharacters(text);
        ValidateRepetitionRules(text);

        int result = ParseRomanValue(text);
        ValidateRange(result, text);

        return result;
    }

    private static void ValidateRomanCharacters(string text)
    {
        foreach (char ch in text)
        {
            if (!RomanNumerals.ContainsKey(ch))
            {
                throw new ArgumentException($"Недопустимый символ '{ch}' в римском числе");
            }
        }
    }

    private static void ValidateRepetitionRules(string text)
    {
        int repeatCount = 0;
        char lastChar = '\0';

        foreach (char currentChar in text)
        {
            if (currentChar == lastChar)
            {
                repeatCount++;
            }
            else
            {
                repeatCount = 1;
            }

            ValidateCharacterRepetition(currentChar, repeatCount);
            lastChar = currentChar;
        }
    }

    private static void ValidateCharacterRepetition(char currentChar, int repeatCount)
    {
        if ((currentChar == 'V' || currentChar == 'L' || currentChar == 'D') && repeatCount > 1)
        {
            throw new ArgumentException($"Символ '{currentChar}' не может повторяться");
        }

        if (repeatCount > 3)
        {
            throw new ArgumentException($"Символ '{currentChar}' не может повторяться более 3 раз");
        }
    }

    private static int ParseRomanValue(string text)
    {
        int result = 0;

        for (int i = 0; i < text.Length; i++)
        {
            int currentValue = RomanNumerals[text[i]];

            if (i == text.Length - 1)
            {
                result += currentValue;

                continue;
            }

            int nextValue = RomanNumerals[text[i + 1]];

            if (currentValue < nextValue)
            {
                ValidateSubtractionRules(text, i);
                result -= currentValue;
            }
            else
            {
                result += currentValue;
            }
        }

        return result;
    }

    private static void ValidateSubtractionRules(string text, int currentIndex)
    {
        char currentChar = text[currentIndex];
        char nextChar = text[currentIndex + 1];

        if (currentChar == 'I' && nextChar != 'V' && nextChar != 'X')
        {
            throw new ArgumentException($"Недопустимое вычитание: 'I' перед '{nextChar}'");
        }

        if (currentChar == 'X' && nextChar != 'L' && nextChar != 'C')
        {
            throw new ArgumentException($"Недопустимое вычитание: 'X' перед '{nextChar}'");
        }

        if (currentChar == 'C' && nextChar != 'D' && nextChar != 'M')
        {
            throw new ArgumentException($"Недопустимое вычитание: 'C' перед '{nextChar}'");
        }

        if (currentIndex > 0 && text[currentIndex] == text[currentIndex - 1])
        {
            throw new ArgumentException($"Вычитаемый символ '{text[currentIndex]}' не может повторяться");
        }
    }

    private static void ValidateRange(int result, string originalText)
    {
        if (result > MaxRomanValue)
        {
            throw new ArgumentException($"Римское число {originalText} превышает максимально допустимое значение {MaxRomanValue}");
        }
    }
}
