namespace FileSystemEmulator.Domain.Patterns.Proxy;

using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Exceptions;
using FileSystemEmulator.Domain.Patterns.Command;

/// <summary>
/// Proxy для перевірки прав доступу до файлової системи
/// </summary>
public class FileSystemProxy : IFileSystemProxy
{
    private List<UserPermission> _permissions = [];
    private CommandHistory _history;

    public FileSystemProxy(CommandHistory? history = null)
    {
        _history = history ?? new CommandHistory();
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
    private void CheckAccess(FileSystemUser user, FileSystemItem item, AccessRight requiredRight)
    {
        // Admin завжди має всі права
        if (user.IsAdmin)
            return;

        var permission = _permissions.FirstOrDefault(p => 
            p.User.Name == user.Name && p.Item == item);

        if (permission == null || !permission.HasRight(requiredRight))
        {
            throw new AccessDeniedException(
                $"Користувач '{user.Name}' не має прав '{requiredRight}' на '{item.Name}'",
                user.Name,
                requiredRight.ToString()
            );
        }
    }

    public FileSystemItem? GetItem(string path, FileSystemUser user)
    {
        // пошук елемента (спрощена реалізація)
        // TODO: реалізувати повний парсинг шляху
        CheckAccess(user, new FileItem(path), AccessRight.Read);
        Console.WriteLine($"[LOG] {user.Name} читає {path}");
        return null;
    }

    public void WriteContent(FileItem file, byte[] content, FileSystemUser user)
    {
        CheckAccess(user, file, AccessRight.Write);
        file.Content = content;
        Console.WriteLine($"[LOG] {user.Name} записав у {file.Name}");
    }

    public void Delete(FileSystemItem item, FileSystemUser user)
    {
        CheckAccess(user, item, AccessRight.Write);
        var deleteCmd = new DeleteCommand(item);
        _history.Execute(deleteCmd);
        Console.WriteLine($"[LOG] {user.Name} видалив {item.Name}");
    }

    public void Copy(FileSystemItem item, DirectoryItem destination, FileSystemUser user)
    {
        CheckAccess(user, item, AccessRight.Read);
        CheckAccess(user, destination, AccessRight.Write);
        var copyCmd = new CopyCommand(item, destination);
        _history.Execute(copyCmd);
        Console.WriteLine($"[LOG] {user.Name} скопіював {item.Name}");
    }

    public void Move(FileSystemItem item, DirectoryItem destination, FileSystemUser user)
    {
        CheckAccess(user, item, AccessRight.Write);
        CheckAccess(user, destination, AccessRight.Write);
        var moveCmd = new MoveCommand(item, destination);
        _history.Execute(moveCmd);
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
