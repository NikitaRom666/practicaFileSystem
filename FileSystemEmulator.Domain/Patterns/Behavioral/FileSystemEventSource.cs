namespace FileSystemEmulator.Domain.Patterns.Behavioral;

using FileSystemEmulator.Domain.Interfaces;

public class FileSystemEventSource
{
    private readonly HashSet<IFileSystemObserver> _observers = [];

    public event EventHandler<IFileSystemItem>? ItemCreated;

    public void Subscribe(IFileSystemObserver observer)
    {
        if (observer == null)
            throw new ArgumentNullException(nameof(observer));

        _observers.Add(observer);
    }

    public void Unsubscribe(IFileSystemObserver observer)
    {
        if (observer == null)
            throw new ArgumentNullException(nameof(observer));

        _observers.Remove(observer);
    }

    public void PublishCreated(IFileSystemItem item) => NotifyCreated(item);
    public void PublishDeleted(IFileSystemItem item) => NotifyDeleted(item);
    public void PublishMoved(IFileSystemItem item, string fromPath, string toPath) => NotifyMoved(item, fromPath, toPath);
    public void PublishAccessDenied(string operation, string userName) => NotifyAccessDenied(operation, userName);

    // лишаємо окремі методи, щоб було видно хто за що відповідає, а не одна каша на все
    private void NotifyCreated(IFileSystemItem item)
    {
        ItemCreated?.Invoke(this, item);

        foreach (var observer in _observers.ToList())
            observer.OnItemCreated(item);
    }

    private void NotifyDeleted(IFileSystemItem item)
    {
        foreach (var observer in _observers.ToList())
            observer.OnItemDeleted(item);
    }

    private void NotifyMoved(IFileSystemItem item, string fromPath, string toPath)
    {
        foreach (var observer in _observers.ToList())
            observer.OnItemMoved(item, fromPath, toPath);
    }

    private void NotifyAccessDenied(string operation, string userName)
    {
        foreach (var observer in _observers.ToList())
            observer.OnAccessDenied(operation, userName);
    }
}