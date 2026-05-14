namespace FileSystemEmulator.Domain.Patterns.Behavioral;

using FileSystemEmulator.Domain.Interfaces;

public class ConsoleLogger : IFileSystemObserver
{
    public void OnItemCreated(IFileSystemItem item)
    {
        WriteLog($"створено елемент {item.Name}");
    }

    public void OnItemDeleted(IFileSystemItem item)
    {
        WriteLog($"видалено елемент {item.Name}");
    }

    public void OnItemMoved(IFileSystemItem item, string fromPath, string toPath)
    {
        WriteLog($"переміщено {item.Name} з {fromPath} у {toPath}");
    }

    public void OnAccessDenied(string operation, string userName)
    {
        WriteLog($"доступ заборонено для {userName} під час {operation}");
    }

    private static void WriteLog(string message)
    {
        Console.WriteLine($"[LOG] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
    }
}