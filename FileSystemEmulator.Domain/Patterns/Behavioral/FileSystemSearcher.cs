namespace FileSystemEmulator.Domain.Patterns.Behavioral;

using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Interfaces;

public class FileSystemSearcher
{
    private ISearchStrategy _strategy = new SearchByNameStrategy();

    // Strategy тут кращий за if/switch, бо можна просто підмінити алгоритм без роздування одного здорового методу
    public void SetStrategy(ISearchStrategy strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public IEnumerable<IFileSystemItem> Search(DirectoryItem root, string query)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));

        return _strategy.Search(root, query);
    }
}