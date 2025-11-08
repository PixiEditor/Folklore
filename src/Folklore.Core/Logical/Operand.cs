namespace Folklore.Logical;

public class Operand
{
    public bool IsLiteral { get; set; }
    public Literal? LiteralValue { get; set; }
    public Reference? ReferenceValue { get; set; }
    
    public Operand(Literal literal)
    {
        IsLiteral = true;
        LiteralValue = literal;
    }
    
    public Operand(Reference reference)
    {
        IsLiteral = false;
        ReferenceValue = reference;
    }
}