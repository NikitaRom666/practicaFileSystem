using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Services;
using FileSystemEmulator.Domain.Patterns.Command;
using FileSystemEmulator.Domain.Patterns.Proxy;
using System.Diagnostics;

// Демонстрація роботи системи
Console.WriteLine("=== Емулятор файлової системи ===\n");

// Створення користувачів
var admin = new FileSystemUser("root", UserRole.Admin);
var user = new FileSystemUser("alice", UserRole.User);
var guest = new FileSystemUser("guest", UserRole.Guest);

// Створення диска
var disk = new DiskVolume("C:\\", 1000000);

// Додавання файлів та папок
var docDir = new DirectoryItem("Documents");
var file1 = new FileItem("readme.txt", System.Text.Encoding.UTF8.GetBytes("Hello World"));
var file2 = new FileItem("notes.txt", System.Text.Encoding.UTF8.GetBytes("Important notes"));
var file3 = new FileItem("config.json", System.Text.Encoding.UTF8.GetBytes("{\"version\": 1}"));

disk.Root.Add(docDir);
docDir.Add(file1);
docDir.Add(file2);
docDir.Add(file3);

Console.WriteLine("Структура диска:");
disk.PrintTree();

Console.WriteLine($"\nСтатистика:");
Console.WriteLine($"Використано: {disk.UsedSpace} байт");
Console.WriteLine($"Вільно: {disk.FreeSpace} байт");

// === ДЕМОНСТРАЦІЯ GENERICS (Repository<T>) ===
Console.WriteLine("\n--- GENERICS: Repository<T> ---");
var repo = new FileSystemEmulator.Domain.Repository.FileSystemRepository<FileItem>();
repo.Add(file1);
repo.Add(file2);
Console.WriteLine($"✓ Repository<FileItem> збереріг {repo.GetAll().Count} файли");

// === ДЕМОНСТРАЦІЯ COMMAND PATTERN + MOVE & DELETE ===
Console.WriteLine("\n--- COMMAND PATTERN: Copy, Move, Delete ---");
var history = new CommandHistory();
var backupDir = new DirectoryItem("Backup");
disk.Root.Add(backupDir);

// Copy
var copyCmd = new CopyCommand(file1, backupDir);
history.Execute(copyCmd);
Console.WriteLine("[CMD] Executed: Copy readme.txt to Backup");

// Move
var archiveDir = new DirectoryItem("Archive");
disk.Root.Add(archiveDir);
var moveCmd = new MoveCommand(file3, archiveDir);
history.Execute(moveCmd);
Console.WriteLine("[CMD] Executed: Move config.json to Archive");

// Delete
var deleteCmd = new DeleteCommand(file2);
history.Execute(deleteCmd);
Console.WriteLine("[CMD] Executed: Delete notes.txt");

Console.WriteLine("\nПісля всіх операцій:");
disk.PrintTree();

// === ДЕМОНСТРАЦІЯ UNDO ===
Console.WriteLine("\n--- UNDO операції ---");
Console.WriteLine($"Команд в історії: {history.History.Count}");
history.Undo();
Console.WriteLine("[UNDO] Reverted Delete notes.txt");
history.Undo();
Console.WriteLine("[UNDO] Reverted Move config.json");
history.Undo();
Console.WriteLine("[UNDO] Reverted Copy readme.txt");
Console.WriteLine("✓ Всі операції скасовані!\n");

// === ДЕМОНСТРАЦІЯ PROXY + ОБРОБКА ВИНЯТКІВ ===
Console.WriteLine("--- PROXY & ОБРОБКА ВИНЯТКІВ ---");
var proxy = new FileSystemProxy(history);

// Надання прав
proxy.GrantPermission(user, docDir, AccessRight.Read);
proxy.GrantPermission(admin, docDir, AccessRight.Read | AccessRight.Write);
Console.WriteLine("✓ Права надані користувачам\n");

// Спроба операції без прав (обробка винятків)
try
{
    // Гість намагається писати (не має прав)
    proxy.WriteContent(file1, System.Text.Encoding.UTF8.GetBytes("Hacked!"), guest);
    Console.WriteLine("✓ Гість може писати (неправильно)");
}
catch (FileSystemEmulator.Domain.Exceptions.AccessDeniedException ex)
{
    Console.WriteLine($"✗ Винятком перехоплено: {ex.Message}");
}

// === ДЕМОНСТРАЦІЯ LINQ ===
Console.WriteLine("\n--- LINQ: ПОШУК ТА ФІЛЬТРАЦІЯ ---");
Console.WriteLine("Усі файли в Documents (LINQ Select):");
var allFiles = docDir.Search("").OfType<FileItem>().ToList();
foreach (var f in allFiles)
{
    Console.WriteLine($"  - {f.Name}: {f.GetSize()} байт");
}

Console.WriteLine("\nПошук за іменем 'note' (Composite pattern):");
var noteFiles = docDir.Search("note").ToList();
foreach (var result in noteFiles)
{
    Console.WriteLine($"  - Знайдено: {result.Name}");
}

Console.WriteLine("\nУсі .txt файли (LINQ Where):");
var txtFiles = allFiles.Where(f => f.Extension == "txt").OrderByDescending(f => f.GetSize()).ToList();
foreach (var f in txtFiles)
{
    Console.WriteLine($"  - {f.Name} ({f.GetSize()} байт)");
}

// === ДЕМОНСТРАЦІЯ COMPOSITE ===
Console.WriteLine("\n--- COMPOSITE PATTERN ---");
Console.WriteLine($"Загальний розмір Documents каталогу (рекурсивно): {docDir.GetSize()} байт");

// === ДЕМОНСТРАЦІЯ СЕРІАЛІЗАЦІЇ ===
Console.WriteLine("\n--- СЕРІАЛІЗАЦІЯ (JSON) ---");
var jsonPath = "disk_backup.json";
SerializationService.SaveToJson(disk, jsonPath);
Console.WriteLine($"✓ Диск збережено в {jsonPath}");

if (File.Exists(jsonPath))
{
    var fileInfo = new FileInfo(jsonPath);
    Console.WriteLine($"  Розмір JSON файлу: {fileInfo.Length} байт");
}

var loadedDisk = SerializationService.LoadFromJson(jsonPath);
Console.WriteLine($"✓ Диск завантажено з {jsonPath}");
Console.WriteLine("Структура завантаженого диска:");
loadedDisk.PrintTree();

// === ДЕМОНСТРАЦІЯ ВАЛІДАЦІЇ ===
Console.WriteLine("\n--- ВАЛІДАЦІЯ ---");
Console.WriteLine($"Диск валідний: {disk.Root.Validate()}");

// === ЗАПУСК ЮНІТ-ТЕСТІВ ===
Console.WriteLine("\n--- ЮНІТ-ТЕСТИ (xUnit + Moq) ---");
Console.WriteLine("Запуск тестів...\n");
var startTest = DateTime.Now;
var processInfo = new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = "test --no-build -v minimal",
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    CreateNoWindow = true
};

using (var process = Process.Start(processInfo))
{
    if (process != null)
    {
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        
        // Вивести результати тестів
        var lines = output.Split('\n');
        foreach (var line in lines)
        {
            if (line.Contains("Passed") || line.Contains("passed") || line.Contains("xUnit"))
                Console.WriteLine(line);
        }
    }
}

Console.WriteLine("\n=== Демонстрація завершена ===");

