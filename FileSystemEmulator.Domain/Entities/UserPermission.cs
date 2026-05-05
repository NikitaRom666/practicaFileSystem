namespace FileSystemEmulator.Domain.Entities;

/// <summary>
/// Права доступу користувача на файловий елемент
/// </summary>
public class UserPermission
{
    public FileSystemUser User { get; }
    public FileSystemItem Item { get; }
    public AccessRight Rights { get; set; }

    public UserPermission(FileSystemUser user, FileSystemItem item, AccessRight rights)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Item = item ?? throw new ArgumentNullException(nameof(item));
        Rights = rights;
    }

    /// <summary>
    /// Перевіряє чи користувач має конкретне право через [Flags]
    /// </summary>
    public bool HasRight(AccessRight right)
    {
        return (Rights & right) == right;
    }

    public override string ToString() => $"{User.Name} -> {Item.Name}: {Rights}";
}
