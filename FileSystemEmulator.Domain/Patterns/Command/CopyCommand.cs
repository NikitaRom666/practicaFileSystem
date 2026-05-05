namespace FileSystemEmulator.Domain.Patterns.Command;

using FileSystemEmulator.Domain.Entities;

/// <summary>
/// Команда копіювання файлу або каталогу
/// </summary>
public class CopyCommand : ICommand
{
    private FileSystemItem _source;
    private DirectoryItem _destination;
    private FileSystemItem? _copiedItem;

    public string Description => $"Copy {_source.Name} to {_destination.Name}";

    public CopyCommand(FileSystemItem source, DirectoryItem destination)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _destination = destination ?? throw new ArgumentNullException(nameof(destination));
    }

    public void Execute()
    {
        // глибока копія
        if (_source is FileItem file)
        {
            _copiedItem = new FileItem(file);
        }
        else if (_source is DirectoryItem dir)
        {
            _copiedItem = new DirectoryItem(dir);
        }

        if (_copiedItem != null)
        {
            _destination.Add(_copiedItem);
        }
    }

    public void Undo()
    {
        if (_copiedItem != null)
        {
            _destination.Remove(_copiedItem);
        }
    }
}
