namespace FileSystemEmulator.Domain.Patterns.Command;

/// <summary>
/// Історія команд з можливістю скасування
/// </summary>
public class CommandHistory
{
    private Stack<ICommand> _history = [];
    private int _maxSize = 20;

    public IReadOnlyCollection<ICommand> History => _history.ToList().AsReadOnly();
    public bool CanUndo => _history.Count > 0;

    /// <summary>
    /// Виконує команду та зберігає у стек
    /// </summary>
    public void Execute(ICommand command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        command.Execute();
        _history.Push(command);

        // видаляємо найстарішу команду якщо перевищили ліміт
        if (_history.Count > _maxSize)
        {
            var temp = _history.ToList();
            _history = new Stack<ICommand>(temp.Take(_maxSize));
        }

        Console.WriteLine($"[CMD] Executed: {command.Description}");
    }

    /// <summary>
    /// Скасовує останню команду
    /// </summary>
    public void Undo()
    {
        if (!CanUndo)
            throw new InvalidOperationException("Нічого не можна скасувати");

        var command = _history.Pop();
        command.Undo();
        Console.WriteLine($"[UNDO] Reverted: {command.Description}");
    }

    public void Clear()
    {
        _history.Clear();
    }
}
