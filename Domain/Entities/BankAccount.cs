using System;
using System.Text.Json.Serialization;

namespace AccountingForFinances;

public class BankAccount : Entity, IVisitable
{
    [JsonInclude]
    public string Name { get; private set; }
    [JsonInclude]
    public decimal Balance { get; set; }

    public BankAccount() { }
    public BankAccount(string name, decimal balance, Guid id)
    {
        Name = name;
        Balance = balance;
        Id = id;
    }
    
    public void UpdateBalance(decimal amount)
    {
        Balance += amount;
    }

    public void Accept(IVisitor visitor)
    {
        visitor.VisitBankAccount(this);
    }
}