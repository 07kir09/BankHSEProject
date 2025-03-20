namespace AccountingForFinances;

public interface IVisitable
{
    void Accept(IVisitor visitor);
}