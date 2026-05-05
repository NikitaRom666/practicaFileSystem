namespace FileSystemEmulator.Domain.Exceptions;

/// <summary>
/// Базова виключення для файлової системи
/// </summary>
public class FileSystemException : Exception
{
    public FileSystemException(string message) : base(message) { }
    public FileSystemException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Доступ заборонено
/// </summary>
public class AccessDeniedException : FileSystemException
{
    public string? User { get; }
    public string? RequiredRight { get; }

    public AccessDeniedException(string message, string? user = null, string? right = null) 
        : base(message)
    {
        User = user;
        RequiredRight = right;
    }
}

/// <summary>
/// Елемент не знайдено
/// </summary>
public class ItemNotFoundException : FileSystemException
{
    public string? ItemName { get; }

    public ItemNotFoundException(string message, string? itemName = null) 
        : base(message)
    {
        ItemName = itemName;
    }
}

/// <summary>
/// Елемент вже існує
/// </summary>
public class ItemAlreadyExistsException : FileSystemException
{
    public ItemAlreadyExistsException(string message) : base(message) { }
}

/// <summary>
/// Некоректна операція
/// </summary>
public class InvalidFileSystemOperationException : FileSystemException
{
    public InvalidFileSystemOperationException(string message) : base(message) { }
}

/// <summary>
/// Диск переповнений
/// </summary>
public class DiskFullException : FileSystemException
{
    public long Available { get; }
    public long Required { get; }

    public DiskFullException(string message, long available, long required) 
        : base(message)
    {
        Available = available;
        Required = required;
    }
}
