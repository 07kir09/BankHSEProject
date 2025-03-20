using System;
using System.Text.Json.Serialization;

namespace AccountingForFinances;

public class Operation : Entity, IVisitable
{
    [JsonInclude]
    public FinanceType Type { get; set; }
    [JsonInclude]
    public Guid BankAccountId { get; set; }
    [JsonInclude]
    public decimal Amount { get; set; }
    [JsonInclude]
    public DateTime DateTime { get; private set; }
    [JsonInclude]
    public string Description { get; private set; }
    [JsonInclude]
    public Guid CategoryId { get; private set; }
    
    public Operation() { }

    public Operation(Guid id, FinanceType type, Guid bankAccountId, decimal amount, DateTime date, string description, Guid categoryId)
    {
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        DateTime = date;
        Description = description;
        CategoryId = categoryId;
        Id = id;
    }
    
    public void Accept(IVisitor visitor)
    {
        visitor.VisitOperation(this);
    }
}