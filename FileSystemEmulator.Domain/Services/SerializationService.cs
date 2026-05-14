namespace FileSystemEmulator.Domain.Services;

using System.Text.Json;
using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Interfaces;

/// <summary>
/// Сервіс для серіалізації та десеріалізації файлової системи
/// </summary>
public class SerializationService : ISerializationService
{
    private static JsonSerializerOptions GetJsonOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    // instance-обгортки для facade, бо там зручніше працювати через об'єкт
    public void Save(DiskVolume volume, string filePath) => SaveToJson(volume, filePath);
    public DiskVolume Load(string filePath) => LoadFromJson(filePath);

    public void SaveToJson(DiskVolume volume, string filePath)
    {
        var dto = ToDiskVolumeDto(volume);
        var json = JsonSerializer.Serialize(dto, GetJsonOptions());

        using var writer = new StreamWriter(filePath);
        writer.Write(json);
        Console.WriteLine($"[SAVE] Диск збережено: {filePath}");
    }

    public DiskVolume LoadFromJson(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не знайдено: {filePath}");

        using var reader = new StreamReader(filePath);
        var json = reader.ReadToEnd();
        var dto = JsonSerializer.Deserialize<DiskVolumeDto>(json, GetJsonOptions()) 
            ?? throw new InvalidOperationException("Не вдалось десеріалізувати диск");
        
        var volume = FromDiskVolumeDto(dto);
        Console.WriteLine($"[LOAD] Диск завантажено: {filePath}");
        return volume;
    }

    /// <summary>
    /// Трансформує DiskVolume в DTO
    /// </summary>
    private static DiskVolumeDto ToDiskVolumeDto(DiskVolume volume)
    {
        return new DiskVolumeDto
        {
            Label = volume.Label,
            TotalSpace = volume.TotalSpace,
            Root = ToDirectoryDto(volume.Root)
        };
    }

    /// <summary>
    /// Трансформує DirectoryItem в DTO
    /// </summary>
    private static DirectoryItemDto ToDirectoryDto(DirectoryItem dir)
    {
        return new DirectoryItemDto
        {
            Name = dir.Name,
            CreatedAt = dir.CreatedAt,
            ModifiedAt = dir.ModifiedAt,
            Children = dir.Children
                .Select(child => child is FileItem file 
                    ? ToFileSystemItemDto(ToFileDto(file))
                    : ToFileSystemItemDto(ToDirectoryDto((DirectoryItem)child)))
                .ToList()
        };
    }

    /// <summary>
    /// Трансформує FileItem в DTO
    /// </summary>
    private static FileItemDto ToFileDto(FileItem file)
    {
        return new FileItemDto
        {
            Name = file.Name,
            Extension = file.Extension,
            SizeBytes = file.GetSize(),
            CreatedAt = file.CreatedAt,
            ModifiedAt = file.ModifiedAt
        };
    }

    private static FileSystemItemDto ToFileSystemItemDto(FileItemDto file)
    {
        return new FileSystemItemDto
        {
            ItemType = "file",
            File = file
        };
    }

    private static FileSystemItemDto ToFileSystemItemDto(DirectoryItemDto dir)
    {
        return new FileSystemItemDto
        {
            ItemType = "directory",
            Directory = dir
        };
    }

    /// <summary>
    /// Трансформує DTO назад у DiskVolume
    /// </summary>
    private static DiskVolume FromDiskVolumeDto(DiskVolumeDto dto)
    {
        var volume = new DiskVolume(dto.Label, dto.TotalSpace);
        
        // переміщаємо дітей до root каталогу
        foreach (var childDto in dto.Root.Children)
        {
            var child = FromFileSystemItemDto(childDto);
            if (child != null)
                volume.Root.Add(child);
        }

        return volume;
    }

    /// <summary>
    /// Трансформує FileSystemItemDto назад у FileSystemItem
    /// </summary>
    private static FileSystemItem? FromFileSystemItemDto(FileSystemItemDto dto)
    {
        if (dto.ItemType == "file" && dto.File != null)
        {
            return FromFileDto(dto.File);
        }
        else if (dto.ItemType == "directory" && dto.Directory != null)
        {
            return FromDirectoryDto(dto.Directory);
        }

        return null;
    }

    /// <summary>
    /// Трансформує FileItemDto назад у FileItem
    /// </summary>
    private static FileItem FromFileDto(FileItemDto dto)
    {
        // Відновлюємо файл з правильним розміром (спеціальні байти для заповнення)
        byte[] content = new byte[dto.SizeBytes];
        if (dto.SizeBytes > 0)
        {
            // Заповнюємо байти для точного розміру
            for (int i = 0; i < content.Length; i++)
                content[i] = (byte)(i % 256);
        }
        
        var file = new FileItem(dto.Name, content);
        return file;
    }

    /// <summary>
    /// Трансформує DirectoryItemDto назад у DirectoryItem
    /// </summary>
    private static DirectoryItem FromDirectoryDto(DirectoryItemDto dto)
    {
        var dir = new DirectoryItem(dto.Name);

        foreach (var childDto in dto.Children)
        {
            var child = FromFileSystemItemDto(childDto);
            if (child != null)
                dir.Add(child);
        }

        return dir;
    }
}
