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

    public BankAccount CreateBankAccount(string name, decimal initialBalance)
    {
        var account = DomainFactory.CreateBankAccount(name, initialBalance);
        _bankAccountRepository.Add(account);
        return account;
    }

    public void UpdateBalance(Guid accountId, decimal amount)
    {
        var account = _bankAccountRepository.GetById(accountId);
        if (account == null)
        {
            throw new Exception("Счет не найден!!!");
        }

        account.UpdateBalance(amount);
        _bankAccountRepository.Update(account);
    }

    public BankAccount GetBankAccount(Guid id)
    {
        return _bankAccountRepository.GetById(id);
    }

    public IList<BankAccount> GetAll()
    {
        return _bankAccountRepository.GetAll();
    }

    public void DeleteBankAccount(Guid id)
    {
        
    }
}