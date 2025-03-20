using System.Text.Json.Serialization;

namespace AccountingForFinances;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FinanceType
{
    Income,
    Expense
}