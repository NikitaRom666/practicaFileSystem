namespace FileSystemEmulator.Domain.Entities;

using FileSystemEmulator.Domain.Exceptions;

/// <summary>
/// Том диска (C:\, D:\, тощо)
/// </summary>
public class DiskVolume
{
    private static readonly Dictionary<Guid, DiskVolume> _volumes = [];
    private static readonly object _sync = new();

    private string _label = "";
    private long _totalSpace;

    public string Label
    {
        get => _label;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Назва диска не може бути порожною");
            _label = value;
        }
    }

    public long TotalSpace
    {
        get => _totalSpace;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Розмір диска повинен бути більший за нуль");
            _totalSpace = value;
        }
    }

    public DirectoryItem Root { get; }

    public long Capacity => TotalSpace;
    public long UsedSpace => Root.GetSize();
    public long FreeSpace => TotalSpace - UsedSpace;

    public long GetUsedSpace() => UsedSpace;

    public DiskVolume(string label, long totalSpace)
    {
        Label = label;
        TotalSpace = totalSpace;
        Root = new DirectoryItem(label.TrimEnd('\\', '/'));

        lock (_sync)
        {
            _volumes[Root.Id] = this;
        }
    }

    public static DiskVolume? TryGetFor(DirectoryItem directory)
    {
        if (directory == null)
            throw new ArgumentNullException(nameof(directory));

        var root = directory;
        while (root.Parent != null)
            root = root.Parent;

        lock (_sync)
        {
            return _volumes.TryGetValue(root.Id, out var volume) ? volume : null;
        }
    }

    /// <summary>
    /// Виводить дерево каталогів через Composite
    /// </summary>
    public void PrintTree()
    {
        Console.WriteLine($"\n--- Диск {_label} ({FreeSpace}/{_totalSpace} байт вільно) ---");
        Root.Print();
    }

    public override string ToString() => $"{_label} ({UsedSpace}/{_totalSpace} байт)";
}
