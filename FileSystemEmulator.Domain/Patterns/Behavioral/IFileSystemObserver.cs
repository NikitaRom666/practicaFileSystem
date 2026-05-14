namespace FileSystemEmulator.Domain.Patterns.Behavioral;

using FileSystemEmulator.Domain.Interfaces;

public interface IFileSystemObserver
{
    void OnItemCreated(IFileSystemItem item);
    void OnItemDeleted(IFileSystemItem item);
    void OnItemMoved(IFileSystemItem item, string fromPath, string toPath);
    void OnAccessDenied(string operation, string userName);
}