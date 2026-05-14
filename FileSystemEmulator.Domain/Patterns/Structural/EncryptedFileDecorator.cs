namespace FileSystemEmulator.Domain.Patterns.Structural;

using FileSystemEmulator.Domain.Entities;

public class EncryptedFileDecorator : IFileDecorator
{
    private readonly FileItem _file;

    public EncryptedFileDecorator(FileItem file)
    {
        _file = file ?? throw new ArgumentNullException(nameof(file));
    }

    public Guid Id => _file.Id;
    public string Name => $"{_file.Name}.enc";
    public DateTime CreatedAt => _file.CreatedAt;
    public DateTime ModifiedAt => _file.ModifiedAt;
    public DirectoryItem? Parent => _file.Parent;

    public byte[] GetContent()
    {
        var buffer = _file.Content.ToArray();
        Array.Reverse(buffer);
        return buffer;
    }

    public long GetSize() => _file.GetSize();

    public void Print(int indent = 0)
    {
        var spaces = new string(' ', indent);
        Console.WriteLine($"{spaces}[FILE] {Name} ({GetSize()} байт)");
    }

    public string GetFullPath() => AppendSuffix(_file.GetFullPath(), Name);

    private static string AppendSuffix(string path, string fileName)
    {
        var lastSlash = path.LastIndexOf('/');
        return lastSlash >= 0 ? $"{path[..(lastSlash + 1)]}{fileName}" : fileName;
    }
}