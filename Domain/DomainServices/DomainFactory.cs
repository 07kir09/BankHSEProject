using System;
using AccountingForFinances;

public static class DomainFactory
{
    public static BankAccount CreateBankAccount(string name, decimal initialBalance)
    {
        if (initialBalance < 0)
            throw new ArgumentException("Начальный баланс не может быть меньше нуля!!!");

        return new BankAccount(name, initialBalance, Guid.NewGuid());
    }

    public static Category CreateCategory(FinanceType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название категории не может быть пустым!!!");

        return new Category(Guid.NewGuid(),type, name);
    }

    public static Operation CreateOperation(
        FinanceType type,
        Guid bankAccountId,
        decimal amount,
        DateTime dateTime,
        string description,
        Guid categoryId)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма операции должна быть строго больше нуля!!!");

        return new Operation(
            Guid.NewGuid(),
            type,
            bankAccountId,
            amount,
            dateTime,
            description,
            categoryId
        );
    }
}