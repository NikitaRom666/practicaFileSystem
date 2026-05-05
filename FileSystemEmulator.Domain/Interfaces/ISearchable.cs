namespace FileSystemEmulator.Domain.Interfaces;

using FileSystemEmulator.Domain.Entities;

/// <summary>
/// Інтерфейс для елементів що можуть шукатись
/// </summary>
public interface ISearchable
{
    IEnumerable<FileSystemItem> Search(string pattern);
}
