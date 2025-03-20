using AccountingForFinances;
using System;
using System.Collections.Generic;
using AccountingForFinances.data;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class YamlImporter : ImporterBase
{
    public YamlImporter(IRepository<BankAccount> bankAccountRepo, IRepository<Category> categoryRepo, IRepository<Operation> operationRepo)
        : base(bankAccountRepo, categoryRepo, operationRepo)
    { }

    /// <summary>
    /// Метод, реализующий парсинг содержимого YAML-файла.
    /// </summary>
    /// <param name="fileContent">Строка с содержимым YAML-файла.</param>
    /// <returns>Кортеж со списками BankAccount, Category и Operation.</returns>
    protected override (List<BankAccount> accounts, List<Category> categories, List<Operation> operations) Parse(string fileContent)
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            var financeData = deserializer.Deserialize<FinanceData>(fileContent);
            if (financeData == null)
                throw new Exception("Не удалось десериализовать данные из YAML.");

            var accounts = financeData.bankAccounts ?? new List<BankAccount>();
            var categories = financeData.categories ?? new List<Category>();
            var operations = financeData.operations ?? new List<Operation>();

            return (accounts, categories, operations);
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при разборе YAML: {ex.Message}", ex);
        }
    }

}