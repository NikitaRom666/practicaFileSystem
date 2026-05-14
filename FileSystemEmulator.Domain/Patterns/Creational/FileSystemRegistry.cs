namespace FileSystemEmulator.Domain.Patterns.Creational;

using FileSystemEmulator.Domain.Interfaces;

public sealed class FileSystemRegistry
{
    private static readonly Lazy<FileSystemRegistry> _instance = new(() => new FileSystemRegistry());
    private readonly Dictionary<Guid, IFileSystemItem> _items = [];
    private readonly object _sync = new();

    public static FileSystemRegistry Instance => _instance.Value;

    private FileSystemRegistry()
    {
    }

    // singleton тут норм, бо треба один спільний реєстр для всього запуску, а не купа копій по кутках
    public void Register(IFileSystemItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        lock (_sync)
        {
            _items[item.Id] = item;
        }
    }

    public bool Unregister(Guid id)
    {
        lock (_sync)
        {
            return _items.Remove(id);
        }
    }

    public IFileSystemItem? GetById(Guid id)
    {
        lock (_sync)
        {
            return _items.TryGetValue(id, out var item) ? item : null;
        }
    }

    public IReadOnlyCollection<IFileSystemItem> GetAll()
    {
        lock (_sync)
        {
            return _items.Values.ToList().AsReadOnly();
        }
    }
}