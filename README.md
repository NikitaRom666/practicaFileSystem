# File System Emulator

Навчальний міні-проєкт з ООП на C#/.NET 9. Емулює файлову систему з деревом каталогів,
контролем доступу, скасуванням операцій і набором класичних патернів проєктування.

## Можливості
- Ієрархія файлів і каталогів з рекурсивними операціями.
- Копіювання, переміщення, видалення з підтримкою Undo (до 20 операцій).
- Контроль доступу через ролі: Admin, User, Guest.
- Пошук і статистика через LINQ, включно з `GroupBy`, `Join`, `Aggregate` і `ToDictionary`.
- Збереження та відновлення структури диску у файл `disk_backup.json`.
- Observer-події для створення, видалення, переміщення та блокування доступу.
- Generic repository, RetryPolicy та DTO-serializing для JSON.

## Архітектура

```
FileSystemEmulator.Domain/
├── Entities/         — FileItem, DirectoryItem, DiskVolume, FileSystemUser
├── Interfaces/       — IFileSystemItem, ISearchable, IPrintable
├── Patterns/
│   ├── Behavioral/   — Observer, Strategy
│   ├── Command/      — CopyCommand, MoveCommand, DeleteCommand, CommandHistory
│   ├── Creational/   — FileSystemItemFactory, FileSystemRegistry, FileSystemFactory
│   ├── Proxy/        — FileSystemProxy, IFileSystemProxy
│   └── Structural/   — FileSystemFacade, CompressedFileDecorator, EncryptedFileDecorator, IFileDecorator
├── Repository/       — FileSystemRepository<T>
├── Services/         — FileSystemQueryService, SerializationService, RetryPolicy
└── Exceptions/       — FileSystemException, AccessDeniedException
FileSystemEmulator.App/
└── Program.cs
FileSystemEmulator.Tests/
└── xUnit + Moq тести
```
## Патерни

| Патерн | Де у коді | Що реалізує |
|---|---|---|
| Composite | [FileSystemItem.cs](FileSystemEmulator.Domain/Entities/FileSystemItem.cs), [FileItem.cs](FileSystemEmulator.Domain/Entities/FileItem.cs), [DirectoryItem.cs](FileSystemEmulator.Domain/Entities/DirectoryItem.cs), [DiskVolume.cs](FileSystemEmulator.Domain/Entities/DiskVolume.cs) | Дерево каталогів, рекурсивний розмір, друк і пошук. |
| Command | [ICommand.cs](FileSystemEmulator.Domain/Patterns/Command/ICommand.cs), [CopyCommand.cs](FileSystemEmulator.Domain/Patterns/Command/CopyCommand.cs), [MoveCommand.cs](FileSystemEmulator.Domain/Patterns/Command/MoveCommand.cs), [DeleteCommand.cs](FileSystemEmulator.Domain/Patterns/Command/DeleteCommand.cs), [CommandHistory.cs](FileSystemEmulator.Domain/Patterns/Command/CommandHistory.cs) | Інкапсуляція операцій і Undo через стек. |
| Proxy | [IFileSystemProxy.cs](FileSystemEmulator.Domain/Patterns/Proxy/IFileSystemProxy.cs), [FileSystemProxy.cs](FileSystemEmulator.Domain/Patterns/Proxy/FileSystemProxy.cs) | Перевірка прав доступу перед операціями та публікація подій. |
| Factory Method | [FileSystemItemFactory.cs](FileSystemEmulator.Domain/Patterns/Creational/FileSystemItemFactory.cs), [FileSystemFactory.cs](FileSystemEmulator.Domain/Patterns/Creational/FileSystemFactory.cs) | Створення файлів і каталогів вручну або з JSON-конфігу. |
| Singleton | [FileSystemRegistry.cs](FileSystemEmulator.Domain/Patterns/Creational/FileSystemRegistry.cs) | Єдиний реєстр елементів по `Guid Id`. |
| Observer | [IFileSystemObserver.cs](FileSystemEmulator.Domain/Patterns/Behavioral/IFileSystemObserver.cs), [FileSystemEventSource.cs](FileSystemEmulator.Domain/Patterns/Behavioral/FileSystemEventSource.cs), [ConsoleLogger.cs](FileSystemEmulator.Domain/Patterns/Behavioral/ConsoleLogger.cs), [AuditLog.cs](FileSystemEmulator.Domain/Patterns/Behavioral/AuditLog.cs) | Підписка на події через `HashSet` і окремий `ItemCreated` event. |
| Strategy | [ISearchStrategy.cs](FileSystemEmulator.Domain/Patterns/Behavioral/ISearchStrategy.cs), [SearchByNameStrategy.cs](FileSystemEmulator.Domain/Patterns/Behavioral/SearchByNameStrategy.cs), [SearchByExtensionStrategy.cs](FileSystemEmulator.Domain/Patterns/Behavioral/SearchByExtensionStrategy.cs), [SearchByPatternStrategy.cs](FileSystemEmulator.Domain/Patterns/Behavioral/SearchByPatternStrategy.cs), [FileSystemSearcher.cs](FileSystemEmulator.Domain/Patterns/Behavioral/FileSystemSearcher.cs) | Підміна алгоритму пошуку без `switch`-логіки. |
| Decorator | [IFileDecorator.cs](FileSystemEmulator.Domain/Patterns/Structural/IFileDecorator.cs), [CompressedFileDecorator.cs](FileSystemEmulator.Domain/Patterns/Structural/CompressedFileDecorator.cs), [EncryptedFileDecorator.cs](FileSystemEmulator.Domain/Patterns/Structural/EncryptedFileDecorator.cs) | Додавання поведінки до файлу без зміни `FileItem`. |
| Facade | [FileSystemFacade.cs](FileSystemEmulator.Domain/Patterns/Structural/FileSystemFacade.cs) | Єдина точка входу для `Program.cs`. |

## Додаткові механізми

- [FileSystemRepository.cs](FileSystemEmulator.Domain/Repository/FileSystemRepository.cs) має `Reduce<TAccum>()` поряд з `Map`, `Filter` і `ForEach`.
- [FileSystemQueryService.cs](FileSystemEmulator.Domain/Services/FileSystemQueryService.cs) містить `JoinFilesWithDirectories()` та `GetTotalFileSize()` поверх вже наявних LINQ-запитів.
- [FileSystemEventSource.cs](FileSystemEmulator.Domain/Patterns/Behavioral/FileSystemEventSource.cs) використовує `HashSet<IFileSystemObserver>` і додатково публікує `ItemCreated` через `EventHandler<T>`.
- [RetryPolicy.cs](FileSystemEmulator.Domain/Services/RetryPolicy.cs) повторює асинхронну дію з експоненційною затримкою.

## Бенчмарк структур даних

| Структура | Середній пошук | Сильна сторона | Де доречно використовувати |
|---|---|---|---|
| `List<T>` | `O(n)` | Простий порядок і дешеве додавання в кінець | Коли елементів мало або потрібен стабільний порядок, як у `DirectoryItem.Children`. |
| `Dictionary<TKey, TValue>` | `O(1)` | Швидкий доступ за ключем | Коли є унікальний ключ, як у `FileSystemRegistry`. |
| `HashSet<T>` | `O(1)` | Унікальність і швидка перевірка membership | Коли треба прибрати дублікати, як у `FileSystemEventSource` для підписників. |

## override vs new

`override` змінює поведінку базового virtual-методу і працює поліморфно. `new` лише ховає базового класу й залежить від статичного типу змінної.

```csharp
public abstract class FileSystemItem
{
	public virtual string Describe() => Name;
}

public class DirectoryItem : FileSystemItem
{
	public override string Describe() => $"[DIR] {Name}/";
}

public class LegacyDirectoryItem : FileSystemItem
{
	public new string Describe() => $"legacy {Name}";
}
```

У цьому проєкті `DirectoryItem` і `FileItem` використовують `override`, а `new` не застосовується в production-коді. Це означає, що дерево об'єктів поводиться передбачувано через базовий тип `FileSystemItem`.

## Збирання та запуск

### Вимоги

- .NET 9 SDK
- xUnit
- Moq

### Збирання

```bash
dotnet build
```

### Запуск програми

```bash
dotnet run --project FileSystemEmulator.App
```

### Запуск тестів

```bash
dotnet test
```

## Бізнес-правила

### Файли та каталоги

1. Файл зберігає ім'я, розширення та вміст у байтах.
2. Каталог містить файли та підкаталоги (Composite pattern).
3. Видалення каталогу видаляє всі вкладені елементи.
4. Копіювання створює новий елемент з новим Id.

### Права доступу

1. Admin має всі права без перевірок.
2. User має права на читання та запис.
3. Guest має тільки право на читання.
4. Операція без прав кидає AccessDeniedException.

### Команди з Undo

1. Copy, Move, Delete реалізують інтерфейс ICommand.
2. CommandHistory зберігає до 20 команд у стеку.
3. Undo скасовує останню операцію та відновлює стан.

## Документація

- [Vision](docs/vision.md)
- [Backlog](docs/backlog.md)
- [Iteration 1](docs/iteration-1.md)
- [Iteration 2](docs/iteration-2.md)
- [Iteration 3](docs/iteration-3.md)
- [Class Diagram](docs/class-diagram.md)
- [Sequence Diagrams](docs/sequence-diagrams.md)
- [TESTING](docs/TESTING.md)
- [test-matrix](docs/test-matrix.md)
- [test-strategy](docs/test-strategy.md)
# File System Emulator

Навчальний міні-проєкт з ООП на C#. Емулює файлову систему з деревом каталогів,
контролем доступу та скасуванням операцій. Демонструє патерни Composite, Command та Proxy.

## Можливості

- Ієрархія файлів і каталогів з рекурсивними операціями.
- Копіювання, переміщення, видалення з підтримкою Undo (до 20 операцій).
- Контроль доступу через ролі: Admin, User, Guest.
- Пошук файлів по імені та розширенню (LINQ).
- Збереження та відновлення структури диску у файл `disk_backup.json`.

## Архітектура

```
FileSystemEmulator.Domain/
├── Entities/         — FileItem, DirectoryItem, DiskVolume, FileSystemUser
├── Interfaces/       — IFileSystemItem, ISearchable, IPrintable
├── Patterns/
│   ├── Command/      — CopyCommand, MoveCommand, DeleteCommand, CommandHistory
│   └── Proxy/        — FileSystemProxy, IFileSystemProxy
├── Repository/       — FileSystemRepository<T>
├── Services/         — FileSystemQueryService, SerializationService
└── Exceptions/       — FileSystemException, AccessDeniedException
FileSystemEmulator.App/
└── Program.cs
FileSystemEmulator.Tests/
└── xUnit тести з Moq
```

## Збирання та запуск

### Вимоги

- .NET 8 SDK
- xUnit
- Moq

### Збирання

```bash
dotnet build
```

### Запуск програми

```bash
dotnet run --project FileSystemEmulator.App
```

### Запуск тестів

```bash
dotnet test
```

## Бізнес-правила

### Файли та каталоги

1. Файл зберігає ім'я, розширення та вміст у байтах.
2. Каталог містить файли та підкаталоги (Composite pattern).
3. Видалення каталогу видаляє всі вкладені елементи.
4. Копіювання створює новий елемент з новим Id.

### Права доступу

1. Admin має всі права без перевірок.
2. User має права на читання та запис.
3. Guest має тільки право на читання.
4. Операція без прав кидає AccessDeniedException.

### Команди з Undo

1. Copy, Move, Delete реалізують інтерфейс ICommand.
2. CommandHistory зберігає до 20 команд у стеку.
3. Undo скасовує останню операцію та відновлює стан.

## Документація

- [Vision](docs/vision.md)
- [Backlog](docs/backlog.md)
- [Iteration 1](docs/iteration-1.md)
- [Iteration 2](docs/iteration-2.md)
- [Iteration 3](docs/iteration-3.md)
- [Class Diagram](docs/class-diagram.md)
- [Sequence Diagrams](docs/sequence-diagrams.md)
- [TESTING](docs/TESTING.md)
- [test-matrix](docs/test-matrix.md)
- [test-strategy](docs/test-strategy.md)
