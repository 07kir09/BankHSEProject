using System.Collections.Generic;
using System.IO;
using AccountingForFinances;

public static class ExportService
{
    public static void ExportAllData(IList<BankAccount> accounts, IList<Category> categories,
        IList<Operation> operations, string filePath)
    {
        var visitor = new ExportVisitor();
        foreach (var acc in accounts)
            acc.Accept(visitor);
        foreach (var cat in categories)
            cat.Accept(visitor);
        foreach (var oper in operations)
            oper.Accept(visitor);

        var result = visitor.GetResult();
        File.WriteAllText(filePath, result);
    }
}