using System;

namespace AccountingForFinances.Analytics;

public static class ScenarioTimer
{
    /// <summary>
    /// Выполняет переданный сценарий (Action) и измеряет время его выполнения.
    /// </summary>
    /// <param name="scenario">Сценарий для выполнения.</param>
    /// <param name="scenarioName">Название сценария для вывода.</param>
    public static void ExecuteWithTiming(Action scenario, string scenarioName)
    {
        Console.WriteLine($"Начало сценария: {scenarioName}");
        var start = DateTime.Now;
        scenario();
        var end = DateTime.Now;
        Console.WriteLine($"Сценарий \"{scenarioName}\" выполнен за {(end - start).TotalMilliseconds} мс.");
    }
}