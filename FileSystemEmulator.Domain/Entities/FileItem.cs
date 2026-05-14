namespace FileSystemEmulator.Domain.Entities;

/// <summary>
/// Файл з вмістом та розширенням
/// </summary>
public class FileItem : FileSystemItem
{
    private byte[] _content;

    public byte[] Content
    {
        get => _content;
        set
        {
            _content = value ?? throw new ArgumentNullException(nameof(value));
            Touch();
        }
    }

    public string Extension
    {
        get
        {
            var lastDot = _name.LastIndexOf('.');
            return lastDot > 0 ? _name.Substring(lastDot + 1) : "";
        }
    }

    public FileItem(string name) : base(name)
    {
        _content = [];
    }

    public FileItem(string name, byte[] content) : base(name)
    {
        _content = content ?? throw new ArgumentNullException(nameof(content));
    }

    // копіювальний конструктор
    public FileItem(FileItem other) : base(other._name)
    {
        _content = (byte[])other._content.Clone();
        Parent = other.Parent;
    }

    public override long GetSize() => _content.Length;

    public override void Print(int indent = 0)
    {
        var spaces = new string(' ', indent);
        Console.WriteLine($"{spaces}[FILE] {_name} ({_content.Length} байт)");
    }

    public override bool Validate()
    {
        // перевіряємо що ім'я не порожне та content не null
        return !string.IsNullOrWhiteSpace(_name) && _content != null;
    }

    public override void AppendTreeRepresentation(System.Text.StringBuilder sb, int indent)
    {
        var spaces = new string(' ', indent);
        sb.AppendLine($"{spaces}[FILE] {_name}");
    }

    // рівність по імені та розширенню, бо Id тут взагалі не має грати ролі
    public static bool operator ==(FileItem? left, FileItem? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;

        return string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase)
            && string.Equals(left.Extension, right.Extension, StringComparison.OrdinalIgnoreCase);
    }

    public static bool operator !=(FileItem? left, FileItem? right) => !(left == right);

    public override bool Equals(object? obj) => obj is FileItem other && this == other;

    public override int GetHashCode()
    {
        return HashCode.Combine(
            StringComparer.OrdinalIgnoreCase.GetHashCode(Name),
            StringComparer.OrdinalIgnoreCase.GetHashCode(Extension));
    }
}
