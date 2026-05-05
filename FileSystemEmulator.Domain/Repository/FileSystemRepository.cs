namespace FileSystemEmulator.Domain.Repository;

using FileSystemEmulator.Domain.Entities;

/// <summary>
/// Generic сховище для файлових елементів
/// </summary>
public class FileSystemRepository<T> where T : FileSystemItem
{
    private List<T> _items = [];

    public void Add(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        _items.Add(item);
    }

    public bool Remove(T item)
    {
        return _items.Remove(item);
    }

    /// <summary>
    /// Пошук по предикату
    /// </summary>
    public T? GetById(Func<T, bool> predicate)
    {
        return _items.FirstOrDefault(predicate);
    }

    /// <summary>
    /// Повертає усі елементи
    /// </summary>
    public IReadOnlyList<T> GetAll()
    {
        return _items.AsReadOnly();
    }

    /// <summary>
    /// Фільтрування по умові
    /// </summary>
    public IEnumerable<T> Filter(Func<T, bool> predicate)
    {
        return _items.Where(predicate);
    }

    /// <summary>
    /// Виконати дію для кожного елемента
    /// </summary>
    public void ForEach(Action<T> action)
    {
        _items.ForEach(action);
    }

    /// <summary>
    /// Трансформування в інший тип
    /// </summary>
    public IEnumerable<TResult> Map<TResult>(Func<T, TResult> selector)
    {
        return _items.Select(selector);
    }

    public int Count => _items.Count;
}
