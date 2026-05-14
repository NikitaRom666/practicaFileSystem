namespace FileSystemEmulator.Domain.Patterns.Structural;

using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Exceptions;
using FileSystemEmulator.Domain.Interfaces;
using FileSystemEmulator.Domain.Patterns.Behavioral;
using FileSystemEmulator.Domain.Patterns.Creational;
using FileSystemEmulator.Domain.Patterns.Command;
using FileSystemEmulator.Domain.Patterns.Proxy;

public class FileSystemFacade
{
    private readonly IFileSystemProxy _proxy;
    private readonly FileSystemSearcher _searcher;
    private readonly ISerializationService _serialization;
    private readonly CommandHistory _history;
    private readonly FileSystemUser _operatorUser = new("facade", UserRole.Admin);

    // приймаємо абстракції а не конкретні класи — легше тестувати і міняти реалізацію
    public FileSystemFacade(
        IFileSystemProxy proxy,
        FileSystemSearcher searcher,
        ISerializationService serialization,
        CommandHistory history)
    {
        _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
        _searcher = searcher ?? throw new ArgumentNullException(nameof(searcher));
        _serialization = serialization ?? throw new ArgumentNullException(nameof(serialization));
        _history = history ?? throw new ArgumentNullException(nameof(history));
    }

    // фасад зручний тим, що Program не мусить знати де там proxy, history чи serialization — викликає один об'єкт і все
    public void CopyItem(Guid sourceId, Guid targetDirId)
    {
        var source = ResolveItem(sourceId) as FileSystemItem
            ?? throw new ItemNotFoundException($"Елемент не знайдено: {sourceId}");
        var target = ResolveItem(targetDirId) as DirectoryItem
            ?? throw new ItemNotFoundException($"Каталог не знайдено: {targetDirId}");

        _proxy.Copy(source, target, _operatorUser);
    }

    public void MoveItem(Guid sourceId, Guid targetDirId)
    {
        var source = ResolveItem(sourceId) as FileSystemItem
            ?? throw new ItemNotFoundException($"Елемент не знайдено: {sourceId}");
        var target = ResolveItem(targetDirId) as DirectoryItem
            ?? throw new ItemNotFoundException($"Каталог не знайдено: {targetDirId}");

        _proxy.Move(source, target, _operatorUser);
    }

    public void DeleteItem(Guid itemId)
    {
        var item = ResolveItem(itemId) as FileSystemItem
            ?? throw new ItemNotFoundException($"Елемент не знайдено: {itemId}");

        _proxy.Delete(item, _operatorUser);
    }

    public void UndoLastOperation()
    {
        _history.Undo();
    }

    public IEnumerable<IFileSystemItem> Search(string query)
    {
        var roots = FileSystemRegistry.Instance.GetAll()
            .OfType<DirectoryItem>()
            .Where(directory => directory.Parent == null);

        return roots
            .SelectMany(root => _searcher.Search(root, query))
            .GroupBy(item => item.Id)
            .Select(group => group.First());
    }

    public void SaveDisk(DiskVolume disk, string path)
    {
        _serialization.SaveToJson(disk, path);
    }

    public DiskVolume LoadDisk(string path)
    {
        return _serialization.LoadFromJson(path);
    }

    public void PrintTree(DirectoryItem root)
    {
        root.Print();
    }

    private static IFileSystemItem? ResolveItem(Guid id)
    {
        return FileSystemRegistry.Instance.GetById(id);
    }
}