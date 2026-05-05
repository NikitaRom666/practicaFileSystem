namespace FileSystemEmulator.Domain.Patterns.Proxy;

using FileSystemEmulator.Domain.Entities;

/// <summary>
/// Інтерфейс для Proxy доступу до файлової системи
/// </summary>
public interface IFileSystemProxy
{
    FileSystemItem? GetItem(string path, FileSystemUser user);
    void WriteContent(FileItem file, byte[] content, FileSystemUser user);
    void Delete(FileSystemItem item, FileSystemUser user);
    void Copy(FileSystemItem item, DirectoryItem destination, FileSystemUser user);
    void Move(FileSystemItem item, DirectoryItem destination, FileSystemUser user);
}
