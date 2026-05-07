# File System Emulator

Емулятор файлової системи з ієрархією каталогів, системою прав доступу та історією операцій. Демонструє патерни Composite, Command та Proxy в C#.

## Можливості

- Ієрархія файлів та каталогів з рекурсивними операціями
- Копіювання, переміщення, видалення з підтримкою Undo до 20 операцій
- Система прав доступу з ролями (Адміністратор, Користувач, Гість)
- Пошук файлів по імені та розширенню
- Серіалізація структури та команд у JSON

## Архітектура

```
FileSystemEmulator.Domain/
├── Entities/ (FileItem, DirectoryItem, DiskVolume, користувачі та ролі)
├── Interfaces/ (IFileSystemItem, ISearchable, IPrintable)
├── Patterns/
│   ├── Command/ (CopyCommand, MoveCommand, DeleteCommand, CommandHistory з Undo)
│   ├── Composite/ (ієрархія через FileSystemItem)
│   └── Proxy/ (FileSystemProxy з перевіркою прав доступу)
├── Repository/ (FileSystemRepository<T>)
├── Services/ (QueryService для пошуку, SerializationService для JSON)
└── Exceptions/ (спеціальні виключення)

FileSystemEmulator.App/
└── Program.cs (демонстрація роботи)

FileSystemEmulator.Tests/
└── xUnit тести з Moq (~90% покриття)
```

## Збирання та запуск

### Вимоги

- .NET 8 SDK
- xUnit для тестів
- Moq для мокування

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

1. Файл має ім'я, розширення, розмір та дату створення
2. Каталог рекурсивно містить файли та інші каталоги (Composite паттерн)
3. Видалення каталога видаляє всі вложені елементи
4. Копіювання створює нову ієрархію з новими ID

### Права доступу

1. Користувач має роль та набір прав (Read, Write, Execute, Delete)
2. FileSystemProxy перевіряє права перед кожною операцією
3. Адміністратор має всі права без перевірок
4. Гість має тільки права на читання
5. Операції без прав кидають AccessDeniedException

### Команди з Undo

1. Кожна операція (Copy, Move, Delete) реалізує ICommand
2. CommandHistory зберігає до 20 команд в стеку
3. Undo скасовує операцію та відновлює попередній стан
4. Команди можуть бути скасовані в будь-якому порядку

## Документація

- [Архітектура](docs/architecture.md)
- [Диаграма класів](docs/class-diagram.md)
- [Діаграми послідовності](docs/sequence-diagrams.md)
- [Стратегія тестування](docs/test-strategy.md)
- [Результати тестів](docs/TESTING.md)
- [Бэклог](docs/backlog.md)
