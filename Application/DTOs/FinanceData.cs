using System.Collections.Generic;

namespace AccountingForFinances.data;

public class FinanceData
{
    public List<BankAccount> bankAccounts { get; set; }
    public List<Category> categories { get; set; }
    public List<Operation> operations { get; set; }
    public AnalyticsResult Analytics { get; set; }
}
