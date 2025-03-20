using System;

namespace AccountingForFinances.Commands;

public class TimeMeasurementCommandDecorator : ICommand
{
    private readonly ICommand _innerCommand;
    private readonly string _commandName;

    public TimeMeasurementCommandDecorator(ICommand innerCommand, string commandName)
    {
        _innerCommand = innerCommand;
        _commandName = commandName;
    }
    
    public void Execute()
    {
        var start = DateTime.Now;
        _innerCommand.Execute();
        var end = DateTime.Now;
        var duration = end - start;
        
        Console.WriteLine($"Команда \"{_commandName}\" выполнена за {(end - start).TotalMilliseconds} мс.");
    }
}