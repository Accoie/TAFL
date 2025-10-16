public interface IScanner
{
    public void Advance();

    public bool IsEnd();

    public char Peek(int n = 0);
}