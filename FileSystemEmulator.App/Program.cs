using System.Text;
using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Interfaces;
using FileSystemEmulator.Domain.Patterns.Behavioral;
using FileSystemEmulator.Domain.Patterns.Creational;
using FileSystemEmulator.Domain.Patterns.Command;
using FileSystemEmulator.Domain.Patterns.Proxy;
using FileSystemEmulator.Domain.Patterns.Structural;
using FileSystemEmulator.Domain.Services;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("=== FileSystemEmulator demo ===");

var history = new CommandHistory();
var eventSource = new FileSystemEventSource();
var proxy = new FileSystemProxy(history, eventSource);
var searcher = new FileSystemSearcher();
var serialization = new SerializationService();
var facade = new FileSystemFacade(proxy, searcher, serialization, history);
var factory = new FileSystemItemFactory();

var consoleLogger = new ConsoleLogger();
var auditLog = new AuditLog();
eventSource.Subscribe(consoleLogger);
eventSource.Subscribe(auditLog);

var admin = new FileSystemUser("root", UserRole.Admin);
var user = new FileSystemUser("alice", UserRole.User);
var guest = new FileSystemUser("guest", UserRole.Guest);

Console.WriteLine($"Користувачі: {admin}, {user}, {guest}");

Console.WriteLine("\n--- Factory Method ---");
var disk = new DiskVolume("C:\\", 1_000_000);
var documents = (DirectoryItem)factory.Create("directory", "Documents");
var backups = (DirectoryItem)factory.CreateDirectory("Backup");
var archive = (DirectoryItem)factory.CreateDirectory("Archive");
var configFolder = (DirectoryItem)FileSystemFactory.CreateFromConfig("{\"type\":\"directory\",\"name\":\"Config\"}");
var readme = (FileItem)factory.CreateFile("readme", "txt", Encoding.UTF8.GetBytes("Hello from factory"));
var notes = (FileItem)factory.CreateFile("notes", ".md", Encoding.UTF8.GetBytes("Markdown note"));
var settings = (FileItem)FileSystemFactory.CreateFromConfig("{\"type\":\"file\",\"name\":\"settings\",\"extension\":\"json\",\"content\":\"{\\\"theme\\\":\\\"forest\\\"}\"}");
var template = (FileItem)FileSystemFactory.CreateFromConfig("{\"type\":\"file\",\"name\":\"template\",\"extension\":\".txt\",\"content\":\"factory config\"}");

disk.Root.Add(documents);
disk.Root.Add(backups);
disk.Root.Add(archive);
disk.Root.Add(configFolder);
documents.Add(readme);
documents.Add(notes);
documents.Add(settings);
documents.Add(template);

Console.WriteLine($"Factory створив: {readme.Name}, {notes.Name}, {settings.Name}, {template.Name}");
Console.WriteLine($"Singleton registry зараз бачить {FileSystemRegistry.Instance.GetAll().Count} елементів");
Console.WriteLine($"Registry lookup для readme: {FileSystemRegistry.Instance.GetById(readme.Id)?.Name}");

Console.WriteLine("\n--- Decorator ---");
var compressedReadme = new CompressedFileDecorator(readme);
var encryptedNotes = new EncryptedFileDecorator(notes);
Console.WriteLine($"{compressedReadme.Name} -> {compressedReadme.Size} байт");
Console.WriteLine($"{encryptedNotes.Name} -> {Encoding.UTF8.GetString(encryptedNotes.GetContent())}");

Console.WriteLine("\n--- Стартова структура ---");
facade.PrintTree(disk.Root);
Console.WriteLine($"Використано: {disk.UsedSpace} байт");
Console.WriteLine($"Вільно: {disk.FreeSpace} байт");

Console.WriteLine("\n--- Strategy ---");
searcher.SetStrategy(new SearchByNameStrategy());
PrintSearchResults("Пошук за іменем 'settings.json'", facade.Search("settings.json"));

searcher.SetStrategy(new SearchByExtensionStrategy());
PrintSearchResults("Пошук за розширенням 'txt'", facade.Search("txt"));

searcher.SetStrategy(new SearchByPatternStrategy());
PrintSearchResults("Пошук за шаблоном '*.md'", facade.Search("*.md"));

Console.WriteLine("\n--- Facade + Command + Undo ---");
facade.CopyItem(readme.Id, backups.Id);
facade.MoveItem(settings.Id, archive.Id);
facade.DeleteItem(notes.Id);
Console.WriteLine("Після копіювання, переміщення і видалення:");
facade.PrintTree(disk.Root);

Console.WriteLine("\nСкасовуємо останню операцію...");
facade.UndoLastOperation();
facade.PrintTree(disk.Root);

Console.WriteLine("\n--- Serialization ---");
var backupPath = "disk_backup.json";
facade.SaveDisk(disk, backupPath);
Console.WriteLine($"Диск збережено у {backupPath}");

var loadedDisk = facade.LoadDisk(backupPath);
Console.WriteLine("Завантажена структура:");
facade.PrintTree(loadedDisk.Root);

Console.WriteLine("\n--- Audit log ---");
foreach (var entry in auditLog.GetLog())
{
    Console.WriteLine(entry);
}

Console.WriteLine("\n=== Демонстрація завершена ===");

static void PrintSearchResults(string title, IEnumerable<IFileSystemItem> results)
{
    Console.WriteLine(title);

    var items = results.ToList();
    if (items.Count == 0)
    {
        Console.WriteLine("  нічого не знайшли");
        return;
    }

    foreach (var item in items)
    {
        Console.WriteLine($"  - {item.Name} ({item.GetSize()} байт)");
    }
}