using System;
using System.Collections.Generic;
using AccountingForFinances.Analytics;

namespace AccountingForFinances.Application.Facades;

public class AnalyticsFacade
{
    private readonly AnalyticsService _analytics;

    public AnalyticsFacade(AnalyticsService analytics)
    {
        _analytics = analytics;
    }

    public decimal GetIncomeExpenseDifference(
        IEnumerable<Operation> operations,
        DateTime startDate, DateTime endDate)
    {
        return _analytics.CalculateIncomeExpenseDifference(operations, startDate, endDate);
    }

    public IDictionary<string, (decimal Income, decimal Expense)> GroupByCategory(
        IEnumerable<Operation> operations,
        IEnumerable<Category> categories)
    {
        return _analytics.GroupOperationsByCategory(operations, categories);
    }

    public IDictionary<string, (int Count, decimal Total, decimal Average)> GetMonthlyStats(
        IEnumerable<Operation> operations)
    {
        return _analytics.GetMonthlyOperationStatistics(operations);
    }
}