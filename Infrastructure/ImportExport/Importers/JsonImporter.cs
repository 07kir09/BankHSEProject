using AccountingForFinances;
using System;
using System.Collections.Generic;
using System.Text.Json;
using AccountingForFinances.data;

public class JsonImporter : ImporterBase
{
    public JsonImporter(IRepository<BankAccount> bankAccountRepo, IRepository<Category> categoryRepo, IRepository<Operation> operationRepo)
        : base(bankAccountRepo, categoryRepo, operationRepo)
    { }

    /// <summary>
    /// Переопределённый метод Parse, который десериализует содержимое JSON-файла в списки объектов.
    /// </summary>
    /// <param name="fileContent">Строка с содержимым JSON-файла.</param>
    /// <returns>Кортеж со списками BankAccount, Category и Operation.</returns>
    protected override (List<BankAccount> accounts, List<Category> categories, List<Operation> operations) Parse(string fileContent)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Десериализуем JSON в FinanceData
            var financeData = JsonSerializer.Deserialize<FinanceData>(fileContent, options);
            if (financeData == null)
                throw new Exception("Не удалось десериализовать данные из JSON.");

            // Если какие-либо списки не определены, создаём пустые
            var accounts = financeData.bankAccounts ?? new List<BankAccount>();
            var categories = financeData.categories ?? new List<Category>();
            var operations = financeData.operations ?? new List<Operation>();

            return (accounts, categories, operations);
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при разборе JSON: {ex.Message}");
        }
    }
}