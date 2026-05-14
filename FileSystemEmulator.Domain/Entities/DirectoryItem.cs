namespace FileSystemEmulator.Domain.Entities;

using FileSystemEmulator.Domain.Interfaces;
using FileSystemEmulator.Domain.Exceptions;


/// Каталог — реалізація Composite патерну

public class DirectoryItem : FileSystemItem, ISearchable
{
    private List<FileSystemItem> _children = [];

    public IReadOnlyList<FileSystemItem> Children => _children.AsReadOnly();

    public DirectoryItem(string name) : base(name)
    {
    }

    // копіювальний конструктор (глибока копія)
    public DirectoryItem(DirectoryItem other) : base(other._name)
    {
        foreach (var child in other._children)
        {
            if (child is FileItem file)
                Add(new FileItem(file));
            else if (child is DirectoryItem dir)
                Add(new DirectoryItem(dir));
        }
        Parent = other.Parent;
    }

   
    /// Додає елемент до каталогу

    public void Add(FileSystemItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        
        // не додаємо сам себе
        if (item == this)
            throw new InvalidOperationException("Не можна додати каталог самого в себе");
        
        // перевіряємо що такого елемента немає
        if (_children.Any(c => string.Equals(c.Name, item.Name, StringComparison.OrdinalIgnoreCase)))
            throw new ItemAlreadyExistsException($"Елемент '{item.Name}' вже існує у цьому каталозі");
        
        item.Parent = this;
        _children.Add(item);
        Touch();
    }

    
    /// Видаляє елемент з каталогу
    
    public bool Remove(FileSystemItem item)
    {
        var result = _children.Remove(item);
        if (result)
        {
            item.Parent = null;
            Touch();
        }
        return result;
    }

    
    /// Розраховує загальний розмір каталогу включаючи всі піделементи (Composite)
    
    public override long GetSize()
    {
        return _children.Sum(child => child.GetSize());
    }

    
    /// Виводить дерево каталогу з відступами (Composite)
   
    public override void Print(int indent = 0)
    {
        var spaces = new string(' ', indent);
        Console.WriteLine($"{spaces}[DIR] {_name}/");
        foreach (var child in _children)
        {
            child.Print(indent + 2);
        }
    }

    
    /// Рекурсивний пошук за шаблоном імені (Composite)
    
    public IEnumerable<FileSystemItem> Search(string pattern)
    {
        var results = new List<FileSystemItem>();
        
        // перевіряємо дітей на поточному рівні
        foreach (var child in _children)
        {
            if (child.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                results.Add(child);
            
            // рекурсивно шукаємо у підкаталогах
            if (child is DirectoryItem subdir)
                results.AddRange(subdir.Search(pattern));
        }
        
        return results;
    }

    
    /// Доступ до дитини за іменем через індексатор
    
    public IFileSystemItem? this[string name]
    {
        get => _children.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    
    /// Перевантаження оператора + для додавання елемента
    
    public static DirectoryItem operator +(DirectoryItem dir, FileSystemItem item)
    {
        dir.Add(item);
        return dir;
    }

    public override bool Validate()
    {
       
        if (string.IsNullOrWhiteSpace(_name))
            return false;
        
        return _children.All(child => child.Validate());
    }

    public override string GetTreeRepresentation()
    {
        var sb = new System.Text.StringBuilder();
        AppendTreeRepresentation(sb, 0);
        return sb.ToString();
    }

    public override void AppendTreeRepresentation(System.Text.StringBuilder sb, int indent)
    {
        var spaces = new string(' ', indent);
        sb.AppendLine($"{spaces}[DIR] {_name}/");
        foreach (var child in _children)
        {
            child.AppendTreeRepresentation(sb, indent + 2);
        }
    }
}
