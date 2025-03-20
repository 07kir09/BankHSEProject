using AccountingForFinances;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

public class CsvImporter : ImporterBase
{
    public CsvImporter(
        IRepository<BankAccount> bankAccountRepo,
        IRepository<Category> categoryRepo,
        IRepository<Operation> operationRepo)
        : base(bankAccountRepo, categoryRepo, operationRepo)
    { }

    /// <summary>
    /// Парсит содержимое CSV-файла и возвращает кортеж списков объектов.
    /// Ожидается, что CSV содержит заголовок с полями:
    /// RecordType;id;name;balance;catType;bankAccountId;opType;amount;date;description;categoryId
    /// </summary>
    /// <param name="fileContent">Содержимое CSV-файла</param>
    /// <returns>Кортеж списков BankAccount, Category, Operation</returns>
    protected override (List<BankAccount>, List<Category>, List<Operation>) Parse(string fileContent)
    {
        // Предварительная обработка содержимого (удаление пустых строк и завершающих ';')
        string processedContent = PreprocessCsvContent(fileContent);

        var bankAccounts = new List<BankAccount>();
        var categories = new List<Category>();
        var operations = new List<Operation>();

        using (var reader = new StringReader(processedContent))
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                BadDataFound = null
            };

            using (var csv = new CsvReader(reader, csvConfig))
            {
                var records = csv.GetRecords<dynamic>().ToList();
                foreach (var record in records)
                {
                    var dict = record as IDictionary<string, object>;
                    if (dict == null)
                        continue;

                    // Определяем тип записи: ищем RecordType или Type
                    string recordType = dict.ContainsKey("RecordType") 
                        ? dict["RecordType"]?.ToString()?.Trim() 
                        : dict.ContainsKey("Type") ? dict["Type"]?.ToString()?.Trim() : null;
                    if (string.IsNullOrEmpty(recordType))
                        continue;

                    if (recordType.Equals("BankAccount", StringComparison.OrdinalIgnoreCase))
                    {
                        // Чтение полей: id, name, balance
                        string idStr = dict.ContainsKey("id") ? dict["id"]?.ToString() : "";
                        Guid id = (!string.IsNullOrWhiteSpace(idStr) && Guid.TryParse(idStr, out Guid parsedId))
                            ? parsedId : Guid.NewGuid();

                        string name = dict.ContainsKey("name") ? dict["name"]?.ToString() : string.Empty;
                        decimal balance = 0m;
                        if (dict.ContainsKey("balance"))
                        {
                            decimal.TryParse(dict["balance"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out balance);
                        }
                        // Создаем объект с использованием конструктора
                        var account = new BankAccount(name, balance, id);
                        bankAccounts.Add(account);
                    }
                    else if (recordType.Equals("Category", StringComparison.OrdinalIgnoreCase))
                    {
                        // Чтение полей для Category: id, name, catType (тип категории)
                        string idStr = dict.ContainsKey("id") ? dict["id"]?.ToString() : "";
                        Guid id = (!string.IsNullOrWhiteSpace(idStr) && Guid.TryParse(idStr, out Guid parsedId))
                            ? parsedId : Guid.NewGuid();

                        string name = dict.ContainsKey("name") ? dict["name"]?.ToString() : string.Empty;
                        string catType = dict.ContainsKey("catType") ? dict["catType"]?.ToString() : string.Empty;
                        FinanceType financeType = (!string.IsNullOrEmpty(catType) && catType.Equals("Income", StringComparison.OrdinalIgnoreCase))
                            ? FinanceType.Income : FinanceType.Expense;
                        // Создаем объект Category
                        var category = new Category(id, financeType, name);
                        categories.Add(category);
                    }
                    else if (recordType.Equals("Operation", StringComparison.OrdinalIgnoreCase))
                    {
                        // Чтение полей для Operation: id, opType, bankAccountId, amount, date, description, categoryId
                        string idStr = dict.ContainsKey("id") ? dict["id"]?.ToString() : "";
                        Guid id = (!string.IsNullOrWhiteSpace(idStr) && Guid.TryParse(idStr, out Guid parsedId))
                            ? parsedId : Guid.NewGuid();

                        string opType = dict.ContainsKey("opType") ? dict["opType"]?.ToString() : string.Empty;
                        FinanceType financeType = (!string.IsNullOrEmpty(opType) && opType.Equals("Income", StringComparison.OrdinalIgnoreCase))
                            ? FinanceType.Income : FinanceType.Expense;

                        decimal amount = 0m;
                        if (dict.ContainsKey("amount"))
                        {
                            decimal.TryParse(dict["amount"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out amount);
                        }
                        if (financeType == FinanceType.Expense)
                        {
                            amount = Math.Abs(amount);
                        }

                        Guid bankAccountId = Guid.Empty;
                        if (dict.ContainsKey("bankAccountId"))
                        {
                            Guid.TryParse(dict["bankAccountId"]?.ToString(), out bankAccountId);
                        }

                        DateTime date = DateTime.MinValue;
                        if (dict.ContainsKey("date"))
                        {
                            DateTime.TryParse(dict["date"]?.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                        }

                        string description = dict.ContainsKey("description") ? dict["description"]?.ToString() : string.Empty;

                        Guid categoryId = Guid.Empty;
                        if (dict.ContainsKey("categoryId"))
                        {
                            Guid.TryParse(dict["categoryId"]?.ToString(), out categoryId);
                        }

                        var operation = new Operation(id, financeType, bankAccountId, amount, date, description, categoryId);
                        operations.Add(operation);
                    }
                }
            }
        }

        return (bankAccounts, categories, operations);
    }

    /// <summary>
    /// Предварительная обработка содержимого CSV-файла:
    /// удаляет пустые строки и завершающие символы ';'.
    /// </summary>
    /// <param name="content">Исходное содержимое CSV-файла</param>
    /// <returns>Обработанное содержимое</returns>
    private string PreprocessCsvContent(string content)
    {
        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var processedLines = new List<string>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            processedLines.Add(line.TrimEnd(';'));
        }

        return string.Join(Environment.NewLine, processedLines);
    }
}