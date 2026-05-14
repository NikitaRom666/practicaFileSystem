namespace FileSystemEmulator.Domain.Patterns.Creational;

using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Interfaces;

public class FileSystemItemFactory
{
    private const int MaxNameLength = 255;
    private static readonly char[] ForbiddenCharacters = ['<', '>', ':', '"', '/', '\\', '|', '?', '*'];

    // фабрика потрібна, бо в одному місці легше сховати всю перевірку і не розмазувати new по всьому коду
    public IFileSystemItem Create(string type, string name)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Тип не може бути порожним", nameof(type));

        return type.Trim().ToLowerInvariant() switch
        {
            "file" => CreateFileFromSingleName(name),
            "directory" => CreateDirectory(name),
            _ => throw new ArgumentException($"Невідомий тип елемента: {type}", nameof(type))
        };
    }

    public IFileSystemItem CreateFile(string name, string extension, byte[]? content = null)
    {
        ValidateName(name);

        var normalizedExtension = NormalizeExtension(extension);
        var fullName = string.IsNullOrWhiteSpace(normalizedExtension)
            ? name.Trim()
            : $"{name.Trim()}.{normalizedExtension}";

        ValidateName(fullName);

        return new FileItem(fullName, content ?? []);
    }

    public IFileSystemItem CreateDirectory(string name)
    {
        ValidateName(name);
        return new DirectoryItem(name.Trim());
    }

    private IFileSystemItem CreateFileFromSingleName(string name)
    {
        ValidateName(name);

        var trimmedName = name.Trim();
        var extension = string.Empty;
        var baseName = trimmedName;

        var lastDot = trimmedName.LastIndexOf('.');
        if (lastDot > 0 && lastDot < trimmedName.Length - 1)
        {
            baseName = trimmedName[..lastDot];
            extension = trimmedName[(lastDot + 1)..];
        }

        return CreateFile(baseName, extension);
    }

    private static string NormalizeExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return string.Empty;

        var normalized = extension.Trim().TrimStart('.');

        if (normalized.Length == 0)
            return string.Empty;

        if (normalized.Any(c => ForbiddenCharacters.Contains(c) || char.IsControl(c)))
            throw new ArgumentException("Розширення містить заборонені символи", nameof(extension));

        return normalized;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ім'я не може бути порожним", nameof(name));

        var trimmed = name.Trim();

        if (trimmed.Length > MaxNameLength)
            throw new ArgumentException($"Ім'я не може бути довшим за {MaxNameLength} символів", nameof(name));

        if (trimmed.Any(c => ForbiddenCharacters.Contains(c) || char.IsControl(c)))
            throw new ArgumentException("Ім'я містить заборонені символи", nameof(name));
    }
}