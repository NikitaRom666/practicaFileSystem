namespace FileSystemEmulator.Domain.Services;

using FileSystemEmulator.Domain.Entities;

/// <summary>
/// Сервіс для LINQ запитів до файлової системи
/// </summary>
public class FileSystemQueryService
{
    /// <summary>
    /// Повертає топ N найбільших файлів
    /// </summary>
    public static IEnumerable<FileItem> GetLargestFiles(DirectoryItem root, int count)
    {
        return root.Search("")
            .OfType<FileItem>()
            .OrderByDescending(f => f.GetSize())
            .Take(count);
    }

    /// <summary>
    /// Знаходить файли за розширенням
    /// </summary>
    public static IEnumerable<FileItem> GetFilesByExtension(DirectoryItem root, string ext)
    {
        return root.Search("")
            .OfType<FileItem>()
            .Where(f => f.Extension.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Розраховує загальний розмір за розширенням
    /// </summary>
    public static Dictionary<string, long> GetTotalSizeByExtension(DirectoryItem root)
    {
        return root.Search("")
            .OfType<FileItem>()
            .GroupBy(f => f.Extension)
            .ToDictionary(g => g.Key, g => g.Sum(f => f.GetSize()));
    }

    /// <summary>
    /// Знаходить файли модифіковані після дати
    /// </summary>
    public static IEnumerable<FileSystemItem> GetRecentlyModified(DirectoryItem root, DateTime since)
    {
        return root.Search("")
            .Where(item => item.ModifiedAt >= since);
    }

    /// <summary>
    /// Пошук за шаблоном із групуванням за розширенням
    /// </summary>
    public static IEnumerable<IGrouping<string, FileSystemItem>> SearchByPatternGrouped(
        DirectoryItem root, string pattern)
    {
        return root.Search(pattern)
            .GroupBy(item => item is FileItem f ? f.Extension : "folder");
    }
}

/// <summary>
/// Extension методи для файлової системи
/// </summary>
public static class FileSystemExtensions
{
    /// <summary>
    /// Перевіряє чи елемент більший за розмір
    /// </summary>
    public static Func<FileSystemItem, bool> IsLargerThan(long sizeBytes)
    {
        return item => item.GetSize() > sizeBytes;
    }

    /// <summary>
    /// Перевіряє чи це прихований файл (починається з крапки)
    /// </summary>
    public static Predicate<FileSystemItem> IsHidden = 
        item => item.Name.StartsWith('.');

    /// <summary>
    /// Рекурсивно видаляє всі прихувані елементи
    /// </summary>
    public static void RemoveHidden(DirectoryItem root)
    {
        var hidden = root.Children
            .Where(c => c.Name.StartsWith('.'))
            .ToList();
        
        foreach (var item in hidden)
        {
            root.Remove(item);
        }

        foreach (var subdir in root.Children.OfType<DirectoryItem>())
        {
            RemoveHidden(subdir);
        }
    }

    /// <summary>
    /// Рекурсивно знаходить найглибший каталог
    /// </summary>
    public static DirectoryItem? FindDeepest(DirectoryItem root)
    {
        var subdirs = root.Children.OfType<DirectoryItem>().ToList();
        if (!subdirs.Any())
            return root;

        return subdirs
            .Select(FindDeepest)
            .OrderByDescending(d => d?.GetFullPath().Count(c => c == '/'))
            .First();
    }
}
