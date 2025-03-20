using System;
using System.Collections.Generic;
using System.Linq;
using AccountingForFinances;

namespace AccountingForFinances.Analytics
{
    public class AnalyticsService
    {
        public decimal CalculateIncomeExpenseDifference(IEnumerable<Operation> operations, DateTime startDate, DateTime endDate)
        {
            var filteredOps = operations.Where(o => o.DateTime >= startDate && o.DateTime <= endDate);
            decimal totalIncome = filteredOps.Where(o => o.Type == FinanceType.Income).Sum(o => o.Amount);
            decimal totalExpense = filteredOps.Where(o => o.Type == FinanceType.Expense).Sum(o => o.Amount);
            return totalIncome - totalExpense;
        }

        public IDictionary<string, (decimal Income, decimal Expense)> GroupOperationsByCategory(IEnumerable<Operation> operations, IEnumerable<Category> categories)
        {
            var result = new Dictionary<string, (decimal Income, decimal Expense)>();

            foreach (var category in categories)
            {
                var ops = operations.Where(o => o.CategoryId == category.Id);
                decimal incomeSum = ops.Where(o => o.Type == FinanceType.Income).Sum(o => o.Amount);
                decimal expenseSum = ops.Where(o => o.Type == FinanceType.Expense).Sum(o => o.Amount);
                result[category.Name] = (incomeSum, expenseSum);
            }
            return result;
        }

        public IDictionary<string, (int Count, decimal Total, decimal Average)> GetMonthlyOperationStatistics(IEnumerable<Operation> operations)
        {
            var groups = operations.GroupBy(o => o.DateTime.ToString("yyyy-MM"));
            var result = new Dictionary<string, (int Count, decimal Total, decimal Average)>();

            foreach (var group in groups)
            {
                int count = group.Count();
                decimal total = group.Sum(o => o.Amount);
                decimal average = count > 0 ? total / count : 0;
                result[group.Key] = (count, total, average);
            }

            return result;
        }
    }
}