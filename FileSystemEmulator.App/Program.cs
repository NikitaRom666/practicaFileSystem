using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Services;
using FileSystemEmulator.Domain.Patterns.Command;
using FileSystemEmulator.Domain.Patterns.Proxy;

// Демонстрація роботи системи
Console.WriteLine("=== Емулятор файлової системи ===\n");

// Створення користувачів
var admin = new FileSystemUser("root", UserRole.Admin);
var user = new FileSystemUser("alice", UserRole.User);

// Створення диска
var disk = new DiskVolume("C:\\", 1000000);

// Додавання файлів та папок
var docDir = new DirectoryItem("Documents");
var file1 = new FileItem("readme.txt", System.Text.Encoding.UTF8.GetBytes("Hello World"));
var file2 = new FileItem("notes.txt", System.Text.Encoding.UTF8.GetBytes("Important notes"));

disk.Root.Add(docDir);
docDir.Add(file1);
docDir.Add(file2);

Console.WriteLine("Структура диска:");
disk.PrintTree();

Console.WriteLine($"\nСтатистика:");
Console.WriteLine($"Використано: {disk.UsedSpace} байт");
Console.WriteLine($"Вільно: {disk.FreeSpace} байт");

// Демонстрація Command патерну
Console.WriteLine("\nКоманди (Command Pattern)");
var history = new CommandHistory();
var backupDir = new DirectoryItem("Backup");
disk.Root.Add(backupDir);
var copyCmd = new CopyCommand(file1, backupDir);
history.Execute(copyCmd);

Console.WriteLine("\nПісля копіювання файлу:");
disk.PrintTree();

// Демонстрація Proxy та прав доступу
Console.WriteLine("\nProxy та права доступу:");
var proxy = new FileSystemProxy(history);
proxy.GrantPermission(user, docDir, AccessRight.Read);
proxy.GrantPermission(user, file1, AccessRight.Read);

Console.WriteLine("✓ Права надані користувачу alice\n");

// Пошук (Composite + LINQ)
Console.WriteLine("Пошук файлів за іменем 'note':");
var searchResults = docDir.Search("note").ToList();
foreach (var result in searchResults)
{
    Console.WriteLine($"  - Знайдено: {result.Name} ({result.GetFullPath()})");
}

// Демонстрація Validate
Console.WriteLine("\nВалідація:");
Console.WriteLine($"Диск валідний: {disk.Root.Validate()}");

// Демонстрація Undo
Console.WriteLine("\nUndo операція:");
if (history.CanUndo)
{
    history.Undo();
    Console.WriteLine("Копіювання скасовано!");
    disk.PrintTree();
}

Console.WriteLine("\nДемонстрація завершена!");

