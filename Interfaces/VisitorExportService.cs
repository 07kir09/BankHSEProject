using System.Collections.Generic;
using System.IO;

namespace AccountingForFinances;

public class VisitorExportService
{
    public static void ExportDataToCsv(
        IEnumerable<BankAccount> accounts,
        IEnumerable<Category> categories,
        IEnumerable<Operation> operations,
        string filePath)
    {
        var visitor = new ExportVisitor();
        foreach (var acc in accounts) acc.Accept(visitor);
        foreach (var cat in categories) cat.Accept(visitor);
        foreach (var op in operations) op.Accept(visitor);

        File.WriteAllText(filePath, visitor.GetResult());
    }
}