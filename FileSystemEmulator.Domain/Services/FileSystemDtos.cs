namespace FileSystemEmulator.Domain.Services;

/// <summary>
/// DTO для файлу
/// </summary>
public class FileItemDto
{
    public string Name { get; set; } = "";
    public string Extension { get; set; } = "";
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

/// <summary>
/// DTO для каталогу
/// </summary>
public class DirectoryItemDto
{
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public List<FileSystemItemDto> Children { get; set; } = [];
}

/// <summary>
/// Базова DTO для файлових елементів
/// </summary>
public class FileSystemItemDto
{
    public string ItemType { get; set; } = ""; // "file" або "directory"
    public FileItemDto? File { get; set; }
    public DirectoryItemDto? Directory { get; set; }
}

/// <summary>
/// DTO для диска
/// </summary>
public class DiskVolumeDto
{
    public string Label { get; set; } = "";
    public long TotalSpace { get; set; }
    public DirectoryItemDto Root { get; set; } = new();
}
