using System.Collections.Generic;

namespace AccountingForFinances.data;

public class AnalyticsResult
{
    public decimal IncomeExpenseDifference { get; set; }
    public IDictionary<string, (decimal Income, decimal Expense)> GroupedByCategory { get; set; }
    public IDictionary<string, (int Count, decimal Total, decimal Average)> MonthlyStats { get; set; }
}