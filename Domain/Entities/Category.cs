using System;
using System.Text.Json.Serialization;
using AccountingForFinances;

public class Category : Entity, IVisitable
{
    [JsonInclude]
    public FinanceType Type { get; set; }  // Сделано публичным

    [JsonInclude]
    public string Name { get; set; }       // Сделано публичным

    public Category() { }  // Параметрless конструктор для десериализации и создания через инициализатор

    public Category(Guid id, FinanceType type, string name)
    {
        Id = id;
        Type = type;
        Name = name;
    }

    public void Accept(IVisitor visitor)
    {
        visitor.VisitCategory(this);
    }
}