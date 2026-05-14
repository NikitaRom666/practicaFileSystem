namespace FileSystemEmulator.Domain.Patterns.Behavioral;

using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Interfaces;

public class SearchByNameStrategy : ISearchStrategy
{
    public IEnumerable<IFileSystemItem> Search(DirectoryItem root, string query)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));

        if (string.IsNullOrWhiteSpace(query))
            query = string.Empty;

        return Traverse(root).Where(item =>
            string.Equals(item.Name, query, StringComparison.OrdinalIgnoreCase));
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