namespace PixiScript;

public class Token
{
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public string Text { get; set; }
    public TokenKind Kind { get; set; }
}

[Flags]
public enum TokenKind
{
    Keyword = 1 << 0,
    Identifier = 1 << 1,
    EndOfLine = 1 << 2,
    OpenScope = 1 << 3,
    CloseScope = 1 << 4,
    OpenPassingScope = 1 << 5,
    ClosePassingScope = 1 << 6,
    Separator = 1 << 7,
    Operator = 1 << 8,
    Assignment = 1 << 9,
    Literal = 1 << 10,
}