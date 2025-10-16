public class FileScanner : IScanner
{
    private readonly char[] fileContent;
    private int position;

    public FileScanner(string path)
    {
        fileContent = File.ReadAllText(path).ToCharArray();
        position = 0;
    }

    public char Peek(int n = 0)
    {
        int peekPosition = position + n;

        if (peekPosition < 0 || peekPosition >= fileContent.Length)
        {
            return '\0';
        }

        return fileContent[peekPosition];
    }

    public char Read()
    {
        if (position >= fileContent.Length)
        {
            return '\0';
        }

        return fileContent[position++];
    }

    public void Advance()
    {
        position = Math.Min(position + 1, fileContent.Length);
    }

    public int GetPosition() => position;

    public bool IsEnd() => position >= fileContent.Length;
}