namespace FileSystemEmulator.Domain.Patterns.Behavioral;

using System.Text.RegularExpressions;
using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Interfaces;

public class SearchByPatternStrategy : ISearchStrategy
{
    public IEnumerable<IFileSystemItem> Search(DirectoryItem root, string query)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));

        var regex = BuildRegex(query);

        return Traverse(root)
            .Where(item => regex.IsMatch(item.Name));
    }

    private static Regex BuildRegex(string? query)
    {
        var pattern = Regex.Escape(query?.Trim() ?? string.Empty)
            .Replace(@"\*", ".*");

        return new Regex($"^{pattern}$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
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