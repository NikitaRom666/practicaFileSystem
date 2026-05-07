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
