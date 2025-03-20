using System;
using System.Collections.Generic;
using AccountingForFinances;

public class OperationFacade
{
    public readonly IRepository<Operation> _operationRepository;
    public readonly IRepository<BankAccount> _bankAccountRepository;

    public OperationFacade(IRepository<Operation> operationRepository, IRepository<BankAccount> bankAccountRepository)
    {
        _operationRepository = operationRepository;
        _bankAccountRepository = bankAccountRepository;
    }
    
    public Operation CreateOperation(FinanceType type, Guid bankAccountId, decimal amount, DateTime dateTime, string description, Guid categoryId)
    {
        var account = _bankAccountRepository.GetById(bankAccountId);
        if (account == null)
        {
            throw new Exception("Счёт не найден!!!");
        }

        var sign = (type == FinanceType.Income) ? +1 : -1;
        account.UpdateBalance(sign * amount);
        _bankAccountRepository.Update(account);

        var operation = DomainFactory.CreateOperation(type, bankAccountId, amount, dateTime, description, categoryId);
        _operationRepository.Add(operation);
        return operation;
    }

    public Operation GetOperation(Guid id)
    {
        return _operationRepository.GetById(id);
    }

    public IList<Operation> GetAllOperations()
    {
        return _operationRepository.GetAll();
    }

    public void DeleteCategory(Guid id)
    {
        
    }
}