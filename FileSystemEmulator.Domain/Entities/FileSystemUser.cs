namespace FileSystemEmulator.Domain.Entities;

/// <summary>
/// Користувач файлової системи з ім'ям та роллю
/// </summary>
public class FileSystemUser
{
    private string _name = "";
    private UserRole _role;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ім'я не може бути порожним");
            _name = value;
        }
    }

    public UserRole Role
    {
        get => _role;
        set => _role = value;
    }

    public bool IsAdmin => _role == UserRole.Admin;

    public FileSystemUser(string name, UserRole role = UserRole.User)
    {
        Name = name;
        Role = role;
    }

    // копіювальний конструктор
    public FileSystemUser(FileSystemUser other)
    {
        _name = other._name;
        _role = other._role;
    }

    public override string ToString() => $"{_name} ({_role})";

    public override bool Equals(object? obj)
    {
        if (obj is not FileSystemUser other)
            return false;
        return _name == other._name && _role == other._role;
    }

    public override int GetHashCode() => HashCode.Combine(_name, _role);
}
