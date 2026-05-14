namespace FileSystemEmulator.Domain.Patterns.Behavioral;

using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Interfaces;

public interface ISearchStrategy
{
    IEnumerable<IFileSystemItem> Search(DirectoryItem root, string query);
}