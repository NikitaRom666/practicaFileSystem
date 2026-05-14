namespace FileSystemEmulator.Domain.Patterns.Creational;

using System.Text;
using System.Text.Json;
using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Interfaces;

public static class FileSystemFactory
{
    private sealed class FileSystemConfig
    {
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Extension { get; set; }
        public string? Content { get; set; }
    }

    public static IFileSystemItem CreateFromConfig(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON конфіг не може бути порожнім", nameof(json));

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var config = JsonSerializer.Deserialize<FileSystemConfig>(json, options)
            ?? throw new InvalidOperationException("Не вдалось прочитати конфіг");

        var factory = new FileSystemItemFactory();

        return config.Type.Trim().ToLowerInvariant() switch
        {
            "file" => factory.CreateFile(
                config.Name,
                config.Extension ?? string.Empty,
                string.IsNullOrEmpty(config.Content) ? [] : Encoding.UTF8.GetBytes(config.Content)),
            "directory" => factory.CreateDirectory(config.Name),
            _ => throw new ArgumentException($"Невідомий тип елемента: {config.Type}")
        };
    }
}