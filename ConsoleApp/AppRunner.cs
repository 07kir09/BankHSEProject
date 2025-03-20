using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using AccountingForFinances;
using AccountingForFinances.Analytics;
using AccountingForFinances.data;
using AccountingForFinances.DataManagement;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BankHSEProject.ConsoleApp
{
    public class AppRunner
    {
        private readonly IRepository<BankAccount> _bankAccountRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Operation> _operationRepo;
        private readonly AnalyticsService _analyticsService;
        private readonly DataManagementService _dataManagementService;
        private readonly CsvImporter _csvImporter;
        private readonly JsonImporter _jsonImporter;
        private readonly YamlImporter _yamlImporter;

        FinanceData financeData;

        public AppRunner(
            IRepository<BankAccount> bankAccountRepo,
            IRepository<Category> categoryRepo,
            IRepository<Operation> operationRepo,
            AnalyticsService analyticsService,
            DataManagementService dataManagementService,
            CsvImporter csvImporter,
            JsonImporter jsonImporter,
            YamlImporter yamlImporter)
        {
            _bankAccountRepo = bankAccountRepo;
            _categoryRepo = categoryRepo;
            _operationRepo = operationRepo;
            _analyticsService = analyticsService;
            _dataManagementService = dataManagementService;
            _csvImporter = csvImporter;
            _jsonImporter = jsonImporter;
            _yamlImporter = yamlImporter;
        }

        public void Run()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                DisplayFancyHeader();
                DisplayMenuOptions();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("Выберите действие: ");
                Console.ResetColor();
                // Если ввод равен null (например, при нажатии Command+D), возвращаем пустую строку.
                string choice = ReadUserInput().Trim();

                // Если команда пустая или не распознана, выводим сообщение и запрашиваем ввод заново.
                if (string.IsNullOrEmpty(choice))
                {
                    Console.WriteLine("Неизвестная команда, повторите попытку.");
                    Pause();
                    continue;
                }

                switch (choice)
                {
                    case "1":
                        ScenarioTimer.ExecuteWithTiming(ImportDataMenu, "Импорт данных");
                        break;
                    case "2":
                        ScenarioTimer.ExecuteWithTiming(RecalculateBalances, "Пересчёт баланса");
                        break;
                    case "3":
                        ScenarioTimer.ExecuteWithTiming(RunAnalytics, "Аналитика");
                        break;
                    case "4":
                        ScenarioTimer.ExecuteWithTiming(ExportDataMenu, "Экспорт данных");
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда, повторите попытку.");
                        Pause();
                        break;
                }
            }
            Console.WriteLine("Программа завершена. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        // Метод для красивого ASCII‑хедера
        void DisplayFancyHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║              HSE BANK FINANCE MANAGEMENT               ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        // Метод для вывода вариантов меню
        void DisplayMenuOptions()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1. Импорт данных (CSV, JSON, YAML)");
            Console.WriteLine("2. Пересчёт баланса");
            Console.WriteLine("3. Аналитика");
            Console.WriteLine("4. Экспорт данных");
            Console.WriteLine("5. Выход");
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
        }

        // Метод чтения ввода пользователя. Если Console.ReadLine возвращает null, возвращаем пустую строку.
        private string ReadUserInput()
        {
            try
            {
                string input = Console.ReadLine();
                return input ?? "";
            }
            catch (IOException)
            {
                return "";
            }
        }

        void Pause()
        {
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        void ImportDataMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("ВЫБЕРИТЕ ФОРМАТ ФАЙЛА ДЛЯ ИМПОРТА:");
                Console.WriteLine("1. CSV  (finance_data.csv)");
                Console.WriteLine("2. JSON (finance_data.json)");
                Console.WriteLine("3. YAML (finance_data.yaml)");
                Console.WriteLine("4. Отмена");
                Console.ResetColor();
                Console.Write("\nВаш выбор: ");
                string formatChoice = ReadUserInput().Trim();
                if (string.IsNullOrEmpty(formatChoice))
                {
                    Console.WriteLine("Неизвестная команда, повторите попытку.");
                    Pause();
                    continue;
                }
                if (formatChoice == "4") return;

                string filePath = formatChoice switch
                {
                    "1" => "finance_data.csv",
                    "2" => "finance_data.json",
                    "3" => "finance_data.yaml",
                    _ => null
                };

                if (string.IsNullOrEmpty(filePath))
                {
                    Console.WriteLine("Неверный выбор формата! Попробуйте ещё раз.");
                    Pause();
                    continue;
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Файл '{filePath}' не найден! Попробуйте ещё раз.");
                    Pause();
                    continue;
                }

                ClearAllData();

                ImporterBase importer = formatChoice switch
                {
                    "1" => _csvImporter,
                    "2" => _jsonImporter,
                    "3" => _yamlImporter,
                    _ => null
                };

                if (importer == null)
                {
                    Console.WriteLine("Ошибка: не удалось определить импортёр. Попробуйте ещё раз.");
                    Pause();
                    continue;
                }

                try
                {
                    importer.ImportData(filePath);
                    BuildFinanceDataSnapshot();
                    PrintFinanceData();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при импорте: " + ex.Message);
                    Console.WriteLine("Попробуйте ещё раз.");
                    Pause();
                }
            }
            Pause();
        }

        void RecalculateBalances()
        {
            Console.Clear();
            if (IsEmptyData())
            {
                Console.WriteLine("Данные отсутствуют, сначала выполните импорт!");
                Pause();
                return;
            }
            try
            {
                _dataManagementService.RecalculateBalances();
                Console.WriteLine("Пересчёт баланса завершён!");
                BuildFinanceDataSnapshot();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при пересчёте баланса: " + ex.Message);
            }
            Pause();
        }

        void RunAnalytics()
        {
            Console.Clear();
            if (IsEmptyData())
            {
                Console.WriteLine("Данные отсутствуют, сначала выполните импорт!");
                Pause();
                return;
            }

            DateTime startDate;
            while (true)
            {
                Console.Write("Введите начальную дату (например, 2025-03-01): ");
                string input = ReadUserInput().Trim();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Неизвестная команда, повторите попытку.");
                    continue;
                }
                if (DateTime.TryParse(input, out startDate))
                    break;
                Console.WriteLine("Неверный формат даты. Попробуйте ещё раз.");
            }

            DateTime endDate;
            while (true)
            {
                Console.Write("Введите конечную дату (например, 2025-03-31): ");
                string input = ReadUserInput().Trim();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Неизвестная команда, повторите попытку.");
                    continue;
                }
                if (DateTime.TryParse(input, out endDate))
                    break;
                Console.WriteLine("Неверный формат даты. Попробуйте ещё раз.");
            }

            try
            {
                decimal diff = _analyticsService.CalculateIncomeExpenseDifference(financeData.operations, startDate, endDate);
                var grouped = _analyticsService.GroupOperationsByCategory(financeData.operations, financeData.categories);
                var monthlyStats = _analyticsService.GetMonthlyOperationStatistics(financeData.operations);

                financeData.Analytics = new AnalyticsResult
                {
                    IncomeExpenseDifference = diff,
                    GroupedByCategory = grouped,
                    MonthlyStats = monthlyStats
                };

                PrintAnalytics();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при выполнении аналитики: " + ex.Message);
            }
            Pause();
        }

        void ExportDataMenu()
        {
            while (true)
            {
                Console.Clear();
                if (IsEmptyData())
                {
                    Console.WriteLine("Данные отсутствуют, ничего экспортировать.");
                    Pause();
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("ВЫБЕРИТЕ ФОРМАТ ДЛЯ ЭКСПОРТА:");
                Console.WriteLine("1. CSV  (export.csv)");
                Console.WriteLine("2. JSON (export.json)");
                Console.WriteLine("3. YAML (export.yaml)");
                Console.WriteLine("4. Отмена");
                Console.ResetColor();
                Console.Write("\nВаш выбор: ");
                string choice = ReadUserInput().Trim();
                if (string.IsNullOrEmpty(choice))
                {
                    Console.WriteLine("Неизвестная команда, повторите попытку.");
                    Pause();
                    continue;
                }
                if (choice == "4") return;

                try
                {
                    switch (choice)
                    {
                        case "1":
                            {
                                string fileName = "export.csv";
                                ExportService.ExportAllData(financeData.bankAccounts, financeData.categories, financeData.operations, fileName);
                                Console.WriteLine($"Данные экспортированы в {fileName}.");
                                break;
                            }
                        case "2":
                            {
                                string fileName = "export.json";
                                var options = new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } };
                                string json = JsonSerializer.Serialize(financeData, options);
                                File.WriteAllText(fileName, json);
                                Console.WriteLine($"Данные экспортированы в {fileName}.");
                                break;
                            }
                        case "3":
                            {
                                string fileName = "export.yaml";
                                var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                                string yamlOutput = serializer.Serialize(financeData);
                                File.WriteAllText(fileName, yamlOutput);
                                Console.WriteLine($"Данные экспортированы в {fileName}.");
                                break;
                            }
                        default:
                            Console.WriteLine("Неверный выбор, повторите ввод.");
                            continue;
                    }
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при экспорте: " + ex.Message);
                    Console.WriteLine("Попробуйте снова.");
                    Pause();
                }
            }
            Pause();
        }

        bool IsEmptyData()
        {
            return financeData == null ||
                   (financeData.bankAccounts.Count == 0 &&
                    financeData.categories.Count == 0 &&
                    financeData.operations.Count == 0);
        }

        void ClearAllData()
        {
            financeData = null;
        }

        void BuildFinanceDataSnapshot()
        {
            financeData = new FinanceData
            {
                bankAccounts = new List<BankAccount>(_bankAccountRepo.GetAll()),
                categories = new List<Category>(_categoryRepo.GetAll()),
                operations = new List<Operation>(_operationRepo.GetAll())
            };
        }

        void PrintFinanceData()
        {
            if (financeData == null)
            {
                Console.WriteLine("Данные отсутствуют.");
                return;
            }
            var options = new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } };
            string json = JsonSerializer.Serialize(financeData, options);
            Console.WriteLine("\nТекущие данные:\n" + json);
        }

        void PrintAnalytics()
        {
            if (financeData.Analytics == null)
            {
                Console.WriteLine("Аналитика ещё не выполнена.");
                return;
            }
            Console.WriteLine($"\nРазница доходов/расходов: {financeData.Analytics.IncomeExpenseDifference}");
            Console.WriteLine("\nГруппировка по категориям:");
            foreach (var kvp in financeData.Analytics.GroupedByCategory)
            {
                Console.WriteLine($"Категория: {kvp.Key}, Доход: {kvp.Value.Income}, Расход: {kvp.Value.Expense}");
            }
            Console.WriteLine("\nСтатистика по месяцам:");
            foreach (var ms in financeData.Analytics.MonthlyStats)
            {
                Console.WriteLine($"Месяц: {ms.Key}, Количество: {ms.Value.Count}, Сумма: {ms.Value.Total}, Среднее: {ms.Value.Average}");
            }
        }
    }
}