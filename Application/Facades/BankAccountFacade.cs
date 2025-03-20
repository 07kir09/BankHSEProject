using System;
using System.Collections.Generic;
using AccountingForFinances;

public class BankAccountFacade
{
    private readonly IRepository<BankAccount> _bankAccountRepository;

    public BankAccountFacade(IRepository<BankAccount> bankAccountRepository)
    {
        _bankAccountRepository = bankAccountRepository;
    }

    public void CreateBankAccount(string bankName, decimal initialBalance)
    {
        var account = new BankAccount
        {
            Id = Guid.NewGuid(),
            Name = bankName,
            Balance = initialBalance
            // При необходимости можно добавить AccountNumber и другие свойства.
        };
        _bankAccountRepository.Add(account);
        Console.WriteLine("Банковский счёт успешно создан через BankAccountFacade.");
    }

    public BankAccount GetBankAccount(Guid id)
    {
        return _bankAccountRepository.GetById(id);
    }

    public IList<BankAccount> GetAllBankAccounts()
    {
        return _bankAccountRepository.GetAll();
    }
}