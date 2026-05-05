namespace FileSystemEmulator.Domain.Patterns.Command;

/// <summary>
/// Інтерфейс для команд з можливістю Undo
/// </summary>
public interface ICommand
{
    void Execute();
    void Undo();
    string Description { get; }
}
