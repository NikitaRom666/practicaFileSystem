namespace FileSystemEmulator.Domain.Patterns.Structural;

using FileSystemEmulator.Domain.Entities;

public class CompressedFileDecorator : IFileDecorator
{
    private readonly FileItem _file;

    public CompressedFileDecorator(FileItem file)
    {
        _file = file ?? throw new ArgumentNullException(nameof(file));
    }

    public Guid Id => _file.Id;
    public string Name => $"{_file.Name}.gz";
    public DateTime CreatedAt => _file.CreatedAt;
    public DateTime ModifiedAt => _file.ModifiedAt;
    public DirectoryItem? Parent => _file.Parent;

    public long Size => (long)(_file.GetSize() * 0.7);

    public long GetSize() => Size;

    public void Print(int indent = 0)
    {
        var spaces = new string(' ', indent);
        Console.WriteLine($"{spaces}[FILE] {Name} ({Size} байт)");
    }

    public string GetFullPath() => AppendSuffix(_file.GetFullPath(), Name);

    private static string AppendSuffix(string path, string fileName)
    {
        var lastSlash = path.LastIndexOf('/');
        return lastSlash >= 0 ? $"{path[..(lastSlash + 1)]}{fileName}" : fileName;
    }
}