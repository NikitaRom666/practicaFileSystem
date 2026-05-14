namespace FileSystemEmulator.Domain.Interfaces;

/// <summary>
/// Основний інтерфейс для файлової системи
/// </summary>
public interface IFileSystemItem
{
    Guid Id { get; }
    string Name { get; }
    DateTime CreatedAt { get; }
    DateTime ModifiedAt { get; }
    long GetSize();
    void Print(int indent = 0);
    string GetFullPath();
}
