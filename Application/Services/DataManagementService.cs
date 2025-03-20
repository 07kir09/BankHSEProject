using System;
using System.Linq;

namespace AccountingForFinances.DataManagement;

public class DataManagementService
{
    private readonly IRepository<BankAccount> _accountRepo;
    private readonly IRepository<Operation> _operationRepo;

    public DataManagementService(IRepository<BankAccount> accountRepo, IRepository<Operation> operationRepo)
    {
        _accountRepo = accountRepo;
        _operationRepo = operationRepo;
    }

    /// <summary>
    /// Пересчитывает баланс для каждого банковского счета на основе операций.
    /// Если рассчитанный баланс отличается от текущего, обновляет его.
    /// </summary>
    public void RecalculateBalances()
    {
        var accounts = _accountRepo.GetAll();
        var operations = _operationRepo.GetAll();

        foreach (var account in accounts)
        {
            // Суммируем операции для данного счета:
            decimal calculatedBalance = operations
                .Where(o => o.BankAccountId == account.Id)
                .Sum(o => o.Type == FinanceType.Income ? o.Amount : -o.Amount);

            if (account.Balance != calculatedBalance)
            {
                Console.WriteLine($"Пересчет баланса для '{account.Name}': старый баланс = {account.Balance}, новый баланс = {calculatedBalance}");
                account.Balance = calculatedBalance; // Обновляем баланс (предполагается, что свойство set доступно)
                _accountRepo.Update(account);
            }
            else
            {
                Console.WriteLine($"Баланс счета '{account.Name}' корректен ({account.Balance}).");
            }
        }
    }
}