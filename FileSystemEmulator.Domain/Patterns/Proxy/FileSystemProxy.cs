namespace FileSystemEmulator.Domain.Patterns.Proxy;

using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Exceptions;
using FileSystemEmulator.Domain.Patterns.Behavioral;
using FileSystemEmulator.Domain.Patterns.Command;

/// <summary>
/// Proxy для перевірки прав доступу до файлової системи
/// </summary>
public class FileSystemProxy : IFileSystemProxy
{
    private List<UserPermission> _permissions = [];
    private CommandHistory _history;
    private readonly FileSystemEventSource _eventSource;

    public FileSystemEventSource EventSource => _eventSource;

    public FileSystemProxy(CommandHistory? history = null, FileSystemEventSource? eventSource = null)
    {
        _history = history ?? new CommandHistory();
        _eventSource = eventSource ?? new FileSystemEventSource();
    }

    /// <summary>
    /// Додає дозвіл користувачу
    /// </summary>
    public void GrantPermission(FileSystemUser user, FileSystemItem item, AccessRight rights)
    {
        var existing = _permissions.FirstOrDefault(p => 
            p.User.Name == user.Name && p.Item == item);
        
        if (existing != null)
        {
            existing.Rights |= rights;
        }
        else
        {
            _permissions.Add(new UserPermission(user, item, rights));
        }
    }

    /// <summary>
    /// Перевіряє чи користувач має права на операцію
    /// </summary>
    private void CheckAccess(FileSystemUser user, FileSystemItem item, AccessRight requiredRight, string operation)
    {
        // Admin завжди має всі права
        if (user.IsAdmin)
            return;

        var permission = _permissions.FirstOrDefault(p => 
            p.User.Name == user.Name && p.Item == item);

        if (permission == null || !permission.HasRight(requiredRight))
        {
            _eventSource.PublishAccessDenied(operation, user.Name);
            throw new AccessDeniedException(
                $"Користувач '{user.Name}' не має прав '{requiredRight}' на '{item.Name}'",
                user.Name,
                requiredRight.ToString()
            );
        }
    }

    public FileSystemItem? GetItem(string path, FileSystemUser user)
    {
        // тут поки не буду будувати повний парсер шляху, бо це вже окремий квест
        if (!user.IsAdmin)
        {
            _eventSource.PublishAccessDenied("Read", user.Name);
            throw new AccessDeniedException(
                $"Користувач '{user.Name}' не має прав 'Read' на '{path}'",
                user.Name,
                AccessRight.Read.ToString()
            );
        }

        Console.WriteLine($"[LOG] {user.Name} читає {path}");
        return null;
    }

    public void WriteContent(FileItem file, byte[] content, FileSystemUser user)
    {
        CheckAccess(user, file, AccessRight.Write, "Write");
        file.Content = content;
        Console.WriteLine($"[LOG] {user.Name} записав у {file.Name}");
    }

    public void Delete(FileSystemItem item, FileSystemUser user)
    {
        CheckAccess(user, item, AccessRight.Delete, "Delete");
        var deleteCmd = new DeleteCommand(item);
        _history.Execute(deleteCmd);
        _eventSource.PublishDeleted(item);
        Console.WriteLine($"[LOG] {user.Name} видалив {item.Name}");
    }

    public void Copy(FileSystemItem item, DirectoryItem destination, FileSystemUser user)
    {
        if (ReferenceEquals(item, destination))
            throw new InvalidFileSystemOperationException("Джерело і призначення не можуть бути одним і тим самим об'єктом");

        CheckAccess(user, item, AccessRight.Read, "Copy");
        CheckAccess(user, destination, AccessRight.Write, "Copy");

        var volume = DiskVolume.TryGetFor(destination);
        if (volume != null)
        {
            var requiredSpace = item.GetSize();
            if (volume.GetUsedSpace() + requiredSpace > volume.Capacity)
            {
                throw new DiskFullException(
                    $"На диску '{volume.Label}' недостатньо місця для '{item.Name}'",
                    volume.FreeSpace,
                    requiredSpace);
            }
        }

        var copyCmd = new CopyCommand(item, destination);
        _history.Execute(copyCmd);
        var copiedItem = destination[item.Name];
        if (copiedItem != null)
            _eventSource.PublishCreated(copiedItem);
        Console.WriteLine($"[LOG] {user.Name} скопіював {item.Name}");
    }

    public void Move(FileSystemItem item, DirectoryItem destination, FileSystemUser user)
    {
        CheckAccess(user, item, AccessRight.Write, "Move");
        CheckAccess(user, destination, AccessRight.Write, "Move");
        var fromPath = item.GetFullPath();
        var moveCmd = new MoveCommand(item, destination);
        _history.Execute(moveCmd);
        _eventSource.PublishMoved(item, fromPath, item.GetFullPath());
        Console.WriteLine($"[LOG] {user.Name} перемістив {item.Name}");
    }

    public void Undo(FileSystemUser user)
    {
        // адміністратор може завжди скасовувати
        if (!user.IsAdmin)
            throw new AccessDeniedException("Тільки адміністратор може скасовувати операції");

        _history.Undo();
    }
}
