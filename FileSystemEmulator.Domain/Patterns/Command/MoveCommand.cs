namespace FileSystemEmulator.Domain.Patterns.Command;

using FileSystemEmulator.Domain.Entities;

/// <summary>
/// Команда переміщення файлу або каталогу
/// </summary>
public class MoveCommand : ICommand
{
    private FileSystemItem _item;
    private DirectoryItem _destination;
    private DirectoryItem? _originalParent;

    public string Description => $"Move {_item.Name} to {_destination.Name}";

    public MoveCommand(FileSystemItem item, DirectoryItem destination)
    {
        _item = item ?? throw new ArgumentNullException(nameof(item));
        _destination = destination ?? throw new ArgumentNullException(nameof(destination));
    }

    public void Execute()
    {
        _originalParent = _item.Parent;
        if (_originalParent != null)
        {
            _originalParent.Remove(_item);
        }
        _destination.Add(_item);
    }

    public void Undo()
    {
        _destination.Remove(_item);
        if (_originalParent != null)
        {
            _originalParent.Add(_item);
        }
    }
}
