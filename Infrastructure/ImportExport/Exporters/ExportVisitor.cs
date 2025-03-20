using System.Text;

namespace AccountingForFinances;

public class ExportVisitor : IVisitor
{
    private readonly StringBuilder _sb = new StringBuilder();

    public void VisitBankAccount(BankAccount account)
    {
        _sb.AppendLine($"BankAccount;{account.Id};{account.Name};{account.Balance}");
    }

    public void VisitCategory(Category category)
    {
        _sb.AppendLine($"Category;{category.Id};{category.Type};{category.Name}");
    }

    public void VisitOperation(Operation operation)
    {
        _sb.AppendLine($"Operation;{operation.Id};{operation.Type};{operation.BankAccountId};"
                       + $"{operation.Amount};{operation.DateTime};{operation.Description};{operation.CategoryId}");
    }

    public string GetResult() => _sb.ToString();
}