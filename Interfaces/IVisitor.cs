using AccountingForFinances;

public interface IVisitor
{
    void VisitBankAccount(BankAccount bankAccount);
    void VisitCategory(Category category);
    void VisitOperation(Operation operation);
}