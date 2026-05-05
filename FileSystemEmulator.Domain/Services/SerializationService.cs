namespace FileSystemEmulator.Domain.Services;

using System.Text.Json;
using FileSystemEmulator.Domain.Entities;

/// <summary>
/// Сервіс для серіалізації та десеріалізації файлової системи
/// </summary>
public class SerializationService
{
    private static JsonSerializerOptions GetJsonOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>
    /// Зберігає диск у JSON
    /// </summary>
    public static void SaveToJson(DiskVolume volume, string filePath)
    {
        var dto = ToDiskVolumeDto(volume);
        var json = JsonSerializer.Serialize(dto, GetJsonOptions());
        File.WriteAllText(filePath, json);
        Console.WriteLine($"[SAVE] Диск збережено: {filePath}");
    }

    /// <summary>
    /// Завантажує диск з JSON
    /// </summary>
    public static DiskVolume LoadFromJson(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не знайдено: {filePath}");

        var json = File.ReadAllText(filePath);
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
        return new FileItem(dto.Name);
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
