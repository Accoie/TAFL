public class TextScanner(string str)
{
    private readonly string text = str;
    private int position;

    /// <summary>
    ///  Читает на N символов вперёд текущей позиции (по умолчанию N=0).
    /// </summary>
    public char Peek(int n = 0)
    {
        int position = this.position + n;
        return position >= text.Length ? '\0' : text[position];
    }

    /// <summary>
    ///  Сдвигает текущую позицию на один символ.
    /// </summary>
    public void Advance()
    {
        position++;
    }

    public bool IsEnd()
    {
        return position >= text.Length;
    }
}