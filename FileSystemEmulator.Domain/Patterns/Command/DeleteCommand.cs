namespace FileSystemEmulator.Domain.Patterns.Command;

using FileSystemEmulator.Domain.Entities;

/// <summary>
/// Команда видалення файлу або каталогу
/// </summary>
public class DeleteCommand : ICommand
{
    private FileSystemItem _item;
    private DirectoryItem? _parent;

    public string Description => $"Delete {_item.Name}";

    public DeleteCommand(FileSystemItem item)
    {
        _item = item ?? throw new ArgumentNullException(nameof(item));
    }

    public void Execute()
    {
        _parent = _item.Parent;
        if (_parent != null)
        {
            _parent.Remove(_item);
        }
    }

    public void Undo()
    {
        if (_parent != null)
        {
            _parent.Add(_item);
        }
    }
}
