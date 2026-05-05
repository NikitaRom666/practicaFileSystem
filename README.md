# Емулятор файлової системи

Проєкт реалізує in-memory емулятор файлової системи на C# .NET 9 з реалізацією основних design патернів та SOLID принципів.

## Структура рішення

```
FileSystemEmulator/
├── FileSystemEmulator.Domain/       (Сутності та логіка)
│   ├── Entities/                   (FileItem, DirectoryItem, DiskVolume)
│   ├── Interfaces/                 (IFileSystemItem, ISearchable, IPrintable)
│   ├── Patterns/
│   │   ├── Command/               (CopyCommand, MoveCommand, DeleteCommand, CommandHistory)
│   │   ├── Composite/             (реалізовано через DirectoryItem)
│   │   └── Proxy/                 (FileSystemProxy з перевіркою прав)
│   ├── Exceptions/                (Спеціальні винятки)
│   ├── Repository/                (Generic FileSystemRepository<T>)
│   └── Services/                  (SerializationService, FileSystemQueryService)
├── FileSystemEmulator.App/         (Консольна демонстрація)
└── FileSystemEmulator.Tests/       (27 xUnit тестів)
```

## Реалізовані патерни

### Composite (Композит)
- `DirectoryItem` містить `List<FileSystemItem>`
- Методи `GetSize()`, `Print()`, `Search()` рекурсивно обробляють вложені елементи
- Дозволяє працювати з файлами та папками через єдиний інтерфейс

### Command + Undo
- `ICommand` інтерфейс з `Execute()` та `Undo()`
- `CopyCommand`, `MoveCommand`, `DeleteCommand` реалізують операції
- `CommandHistory` зберігає до 20 команд в стеку і дозволяє їх скасовувати
- Кожна операція логується: `[CMD] Executed: ...`

### Proxy
- `FileSystemProxy` перевіряє права доступу перед операціями
- `CheckAccess()` кидає `AccessDeniedException` якщо прав недостатньо
- Адміністратор завжди має `FullControl` без додаткових перевірок
- Логування всіх операцій: `[LOG] user читає file`

## SOLID принципи в проєкті

### Single Responsibility Principle (SRP)
- Кожен клас має одну відповідальність:
  - `FileItem` — представлення файлу
  - `DirectoryItem` — управління колекцією файлів
  - `SerializationService` — тільки серіалізація (не змішана з логіком)
  - `FileSystemProxy` — тільки перевірка прав (делегує до реальних об'єктів)

### Open/Closed Principle (OCP)
- Нові команди додаються без змін у `CommandHistory`
- Нові інтерфейси реалізуються через наслідування, не через модифікацію
- Розширення через inheritance: `FileSystemItem` → `FileItem` / `DirectoryItem`

### Liskov Substitution Principle (LSP)
- `FileItem` та `DirectoryItem` взаємозамінні через `FileSystemItem`
- Код може працювати з масивом `FileSystemItem[]` поліморфно
- `IFileSystemItem` дозволяє використовувати будь-який елемент однаково

### Interface Segregation Principle (ISP)
- `IFileSystemItem` — основні операції для всіх елементів
- `ISearchable` — тільки для `DirectoryItem` (не в `FileItem`)
- `IPrintable` — для всіх що мають вивід
- Клієнти залежать від мінімально необхідних інтерфейсів

### Dependency Inversion Principle (DIP)
- `FileSystemProxy` залежить від `ICommand` інтерфейсу, не від конкретних команд
- `CommandHistory` ін'єкується в `Proxy` конструктором (не створюється всередині)
- Бізнес-логіка залежить від абстракцій, не від деталей

## Як запустити

### Запуск демонстрації:
```bash
cd FileSystemEmulator
dotnet run --project FileSystemEmulator.App
```

### Запуск тестів:
```bash
dotnet test
# 27 тестів успішно пройшли
```

### Збірка:
```bash
dotnet build
```

## Приклади використання

### Створення структури:
```csharp
var disk = new DiskVolume("C:\\", 1000000);
var docs = new DirectoryItem("Documents");
var file = new FileItem("readme.txt", Encoding.UTF8.GetBytes("Hello"));

disk.Root.Add(docs);
docs.Add(file);
```

### Пошук файлів:
```csharp
var results = docs.Search("readme").ToList();
// results = [readme.txt]
```

### Команди з Undo:
```csharp
var history = new CommandHistory();
var copyCmd = new CopyCommand(file, backupDir);
history.Execute(copyCmd);  // копіює файл
history.Undo();            // скасовує копіювання
```

### Проверка прав доступу:
```csharp
var proxy = new FileSystemProxy(history);
var user = new FileSystemUser("alice", UserRole.User);

proxy.GrantPermission(user, docs, AccessRight.Read);
proxy.Delete(file, user);  // кидає AccessDeniedException
```

### Серіалізація:
```csharp
SerializationService.SaveToJson(disk, "disk_backup.json");
var loadedDisk = SerializationService.LoadFromJson("disk_backup.json");
```

## Тестування

Проєкт покривається 27 xUnit тестами:
- **FileItemTests** (6 тестів) — тестування файлів
- **DirectoryItemTests** (8 тестів) — рекурсивні операції
- **CommandHistoryTests** (5 тестів) — команди та Undo
- **CommandTests** (6 тестів) — Copy, Move, Delete
- **FileSystemProxyTests** (2 тести) — права доступу

Запуск: `dotnet test`

## Що можна покращити

- [ ] Реалізувати повний парсинг шляхів (C:\Folder\File.txt)
- [ ] Додати серіалізацію у XML
- [ ] Реалізувати більш складні правила перевірки доступу (chmod-style)
- [ ] Додати кешування розмірів каталогів для оптимізації
- [ ] Реалізувати симlinkи та shortcuts
- [ ] Додати trash/recycle для м'якого видалення
- [ ] Реалізувати version control для файлів
- [ ] Додати квоти на простір для користувачів

## Технологічний стек

- C# 13
- .NET 9
- xUnit для тестування
- Moq для мокування
- System.Text.Json для серіалізації
