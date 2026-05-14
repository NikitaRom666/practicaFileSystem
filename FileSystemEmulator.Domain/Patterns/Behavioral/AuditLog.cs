namespace FileSystemEmulator.Domain.Patterns.Behavioral;

using FileSystemEmulator.Domain.Interfaces;

public class AuditLog : IFileSystemObserver
{
    public List<string> Entries { get; } = [];

    public void OnItemCreated(IFileSystemItem item)
    {
        Entries.Add(BuildEntry($"створено {item.Name}"));
    }

    public void OnItemDeleted(IFileSystemItem item)
    {
        Entries.Add(BuildEntry($"видалено {item.Name}"));
    }

    public void OnItemMoved(IFileSystemItem item, string fromPath, string toPath)
    {
        Entries.Add(BuildEntry($"переміщено {item.Name}: {fromPath} -> {toPath}"));
    }

    public void OnAccessDenied(string operation, string userName)
    {
        Entries.Add(BuildEntry($"доступ заборонено для {userName} під час {operation}"));
    }

    public IReadOnlyList<string> GetLog()
    {
        return Entries.AsReadOnly();
    }

    private static string BuildEntry(string message)
    {
        return $"[LOG] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
    }
}