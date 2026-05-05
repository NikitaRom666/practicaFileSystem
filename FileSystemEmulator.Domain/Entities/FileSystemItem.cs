namespace FileSystemEmulator.Domain.Entities;

using FileSystemEmulator.Domain.Interfaces;

/// <summary>
/// Абстрактна база для файлів та каталогів
/// </summary>
public abstract class FileSystemItem : IFileSystemItem, IPrintable
{
    protected string _name;
    private DateTime _createdAt;
    private DateTime _modifiedAt;
    
    public DirectoryItem? Parent { get; set; }

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

    public DateTime CreatedAt => _createdAt;
    public DateTime ModifiedAt => _modifiedAt;

    protected FileSystemItem(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ім'я не може бути порожним");
        
        _name = name;
        _createdAt = DateTime.Now;
        _modifiedAt = DateTime.Now;
    }

    /// <summary>
    /// Оновлює час модифікації на теперішній час
    /// </summary>
    public void Touch()
    {
        _modifiedAt = DateTime.Now;
    }

    /// <summary>
    /// Повертає розмір елемента в байтах
    /// </summary>
    public abstract long GetSize();

    /// <summary>
    /// Виводить елемент у консоль з відступом
    /// </summary>
    public abstract void Print(int indent = 0);

    /// <summary>
    /// Повертає повний шлях до елемента через батьківські директорії
    /// </summary>
    public virtual string GetFullPath()
    {
        if (Parent == null)
            return _name;
        
        var parentPath = Parent.GetFullPath();
        return $"{parentPath}/{_name}";
    }

    /// <summary>
    /// Перевіряє інваріанти елемента
    /// </summary>
    public abstract bool Validate();

    /// <summary>
    /// Повертає текстове представлення дерева
    /// </summary>
    public virtual string GetTreeRepresentation()
    {
        var sb = new System.Text.StringBuilder();
        AppendTreeRepresentation(sb, 0);
        return sb.ToString();
    }

    public virtual void AppendTreeRepresentation(System.Text.StringBuilder sb, int indent)
    {
        var spaces = new string(' ', indent);
        sb.AppendLine($"{spaces}{_name}");
    }

    public override string ToString() => $"{_name} ({GetSize()} байт, мод. {_modifiedAt:yyyy-MM-dd HH:mm})";
}
