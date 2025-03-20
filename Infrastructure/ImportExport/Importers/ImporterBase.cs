using AccountingForFinances;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public abstract class ImporterBase
{
    private readonly IRepository<BankAccount> _bankAccountRepository;
    private readonly IRepository<Operation> _operationRepository;
    private readonly IRepository<Category> _categoryRepository;

    protected ImporterBase(IRepository<BankAccount> bankAccountRepository, IRepository<Category> categoryRepository,
        IRepository<Operation> operationRepository)
    {
        _bankAccountRepository = bankAccountRepository;
        _categoryRepository = categoryRepository;
        _operationRepository = operationRepository;
    }

    public void ImportData(string filePath)
    {
        var fileContent = File.ReadAllText(filePath);
        var (accounts, categories, operations) = Parse(fileContent);

        foreach (var acc in accounts)
            _bankAccountRepository.Add(acc);

        foreach (var cat in categories)
            _categoryRepository.Add(cat);

        foreach (var oper in operations)
            _operationRepository.Add(oper);

        Console.WriteLine("Импорт успешно завершён!");
    }
    
    protected abstract (List<BankAccount> accounts, List<Category> categories, List<Operation> operations) Parse(string fileContent);
}