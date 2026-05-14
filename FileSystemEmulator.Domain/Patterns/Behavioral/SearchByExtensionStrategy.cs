namespace FileSystemEmulator.Domain.Patterns.Behavioral;

using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Interfaces;

public class SearchByExtensionStrategy : ISearchStrategy
{
    public IEnumerable<IFileSystemItem> Search(DirectoryItem root, string query)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));

        var normalizedExtension = query?.Trim().TrimStart('.') ?? string.Empty;

        return Traverse(root)
            .OfType<FileItem>()
            .Where(file => string.Equals(file.Extension, normalizedExtension, StringComparison.OrdinalIgnoreCase))
            .Cast<IFileSystemItem>();
    }

    private static IEnumerable<IFileSystemItem> Traverse(DirectoryItem directory)
    {
        foreach (var child in directory.Children)
        {
            yield return child;

            if (child is DirectoryItem nested)
            {
                foreach (var nestedItem in Traverse(nested))
                    yield return nestedItem;
            }
        }
    }
}