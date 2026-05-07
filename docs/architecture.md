# Architecture - FileSystem Emulator

## Архітектурний Огляд

FileSystem Emulator використовує 3-tier архітектуру з поділом на Domain, Application та Test шари.

## Архітектурні Шари

### 1. Domain Layer (Основна Логіка)

Містить всю бізнес-логіку та основні компоненти:

```
Domain/
├── Entities/
│   ├── FileSystemItem (абстрактна база)
│   ├── FileItem (файл)
│   ├── DirectoryItem (папка з рекурсивної обробкою)
│   ├── DiskVolume (диск)
│   ├── FileSystemUser (користувач)
│   ├── UserRole (enum: Admin, User, Guest)
│   ├── AccessRight (flags enum: Read, Write, Execute, Delete)
│   └── UserPermission (дозвіл користувача)
│
├── Interfaces/
│   ├── IFileSystemItem (операції GetSize, Print, Search)
│   ├── ISearchable (рекурсивний пошук)
│   └── IPrintable (форматований вивід)
│
├── Patterns/
│   ├── Command/
│   │   ├── ICommand (Execute, Undo)
│   │   ├── CopyCommand
│   │   ├── MoveCommand
│   │   ├── DeleteCommand
│   │   └── CommandHistory (стек до 20 команд)
│   │
│   ├── Composite/ (реалізовано через FileSystemItem)
│   │   └── FileSystemItem + DirectoryItem як контейнер
│   │
│   └── Proxy/
│       ├── IFileSystemProxy (CheckAccess, GrantPermission)
│       └── FileSystemProxy (перевірка прав перед операціями)
│
├── Repository/
│   └── FileSystemRepository<T> (generic CRUD операції)
│
├── Exceptions/
│   └── FileSystemExceptions (6 custom exception типів)
│
└── Services/
    ├── FileSystemQueryService (LINQ запити)
    ├── SerializationService (JSON операції)
    └── FileSystemDtos (DTO для серіалізації)
```

### 2. Application Layer (Користувацький Інтерфейс)

```
Application/
└── Program.cs
    ├── Демонстрація структури
    ├── Операції з файлами
    ├── Пошук та запити
    ├── Права доступу
    └── Серіалізація
```

### 3. Test Layer (Тестування)

```
Tests/
├── FileItemTests (6 тестів)
├── DirectoryItemTests (8 тестів)
├── CommandHistoryTests (5 тестів)
├── CommandTests (6 тестів)
└── FileSystemProxyTests (2 тести)

Total: 27 тестів, ~90% покриття
```

## Потік Даних

### Створення та Управління

```
User Input
    ↓
Program.cs
    ↓
Domain Services (QueryService, SerializationService)
    ↓
Pattern Classes (Command, Proxy)
    ↓
Domain Entities (FileItem, DirectoryItem, etc.)
    ↓
Memory Storage (List<FileSystemItem>)
```

### Операція з Правами Доступу

```
FileSystemProxy.Delete(file, user)
    ↓
CheckAccess(user, file, AccessRight.Delete)
    ↓
GetPermission(user, file)
    ↓
Перевірка (прав достатньо?)
    ↓
Так → DeleteCommand.Execute()
    ↓
Логування операції
    ↓
Додавання в CommandHistory
    ↓
Видалення файлу
```

## Design Паттерни

### Composite Pattern (для ієрархії файлів)

```
FileSystemItem (Component)
├── FileItem (Leaf)
└── DirectoryItem (Composite)
    └── List<FileSystemItem> children
        ├── FileItem (Leaf)
        ├── DirectoryItem (Composite)
        │   └── ...
        └── FileItem (Leaf)

Операції:
- GetSize(): Рекурсивно суму розмірів всіх файлів
- Print(): Рекурсивно вивід структури
- Search(): Рекурсивний пошук у всіх вложених елементах
```

### Command Pattern (для операцій з Undo)

```
User Request
    ↓
FileSystemProxy
    ↓
SpecificCommand (Copy/Move/Delete)
    │
    ├── Execute()
    │   └── Виконання операції
    │   └── Збереження стану для Undo
    │
    └── Undo()
        └── Скасування операції
        └── Відновлення попереднього стану

CommandHistory
├── Stack<ICommand> (до 20)
├── Execute(command) → додати в стек
├── Undo() → скасувати останню команду
└── Логування всіх операцій
```

### Proxy Pattern (для контролю доступу)

```
Client Request
    ↓
FileSystemProxy (перевіряє права)
    │
    ├── CheckAccess(user, resource, right)
    │   │
    │   ├── GetPermission(user, resource)
    │   ├── Порівняти з AccessRight
    │   │
    │   ├── Адміністратор? → Зараз доступ
    │   │
    │   ├── Користувач? → Перевірити права
    │   │
    │   ├── Гість? → Тільки Read
    │   │
    │   └── Логування спроби доступу
    │
    └── Делегування до реальної операції
```

## SOLID Принципи Реалізація

### 1. Single Responsibility Principle (SRP)

| Клас | Відповідальність |
|------|-----------------|
| FileItem | Представлення файлу |
| DirectoryItem | Управління колекцією файлів |
| CopyCommand | Копіювання файлів |
| MoveCommand | Переміщення файлів |
| DeleteCommand | Видалення файлів |
| FileSystemProxy | Контроль доступу |
| FileSystemQueryService | LINQ запити |
| SerializationService | JSON збереження/завантаження |

### 2. Open/Closed Principle (OCP)

```
Розширення без Модифікації:

Нова команда (наприклад, RenameCommand)?
- Реалізуємо ICommand
- Додаємо в CommandHistory (без змін)
- Система автоматично підтримує Undo

Новий тип ролі користувача?
- Додаємо в UserRole enum
- Додаємо права в UserPermission
- Proxy автоматично знає про нові ролі
```

### 3. Liskov Substitution Principle (LSP)

```
FileSystemItem[] items = new FileSystemItem[3];
items[0] = new FileItem("readme.txt");
items[1] = new DirectoryItem("docs");
items[2] = new FileItem("config.json");

// Всі методи працюють однаково
foreach (var item in items) {
    var size = item.GetSize();        // SRP - поліморфно
    item.Print();                     // SRP - поліморфно
}

// Для DirectoryItem додатково
if (items[1] is DirectoryItem dir) {
    var results = dir.Search("pattern");  // ISearchable
}
```

### 4. Interface Segregation Principle (ISP)

```
IFileSystemItem
├── GetSize()
├── Print()
├── GetModifiedDate()
└── GetCreatedDate()

ISearchable (тільки для DirectoryItem)
└── Search(pattern)

IPrintable (для всіх)
└── Print()

UserPermission (тільки те що потрібно)
├── User
├── Resource
└── Rights
```

### 5. Dependency Inversion Principle (DIP)

```
FileSystemProxy залежить від абстракцій:

public class FileSystemProxy {
    private ICommand[] commands;      // Залежить від интерфейса
    private FileSystemRepository repo; // Залежить від абстракції
    
    public void Execute(ICommand cmd) { // Приймає інтерфейс
        // Логіка залежить від абстракцій, не від конкретних класів
    }
}

CopyCommand залежить від абстракцій:

public class CopyCommand : ICommand {
    private FileSystemItem source;    // Абстракція
    private DirectoryItem destination; // Абстракція
}
```

## Розширення Архітектури

### Додавання Нової Команди

```csharp
// 1. Створити нову команду
public class RenameCommand : ICommand {
    private FileSystemItem item;
    private string oldName;
    private string newName;
    
    public void Execute() => item.Name = newName;
    public void Undo() => item.Name = oldName;
}

// 2. Використати в Proxy
public void Rename(FileSystemItem item, string newName, FileSystemUser user) {
    CheckAccess(user, item, AccessRight.Write);
    var cmd = new RenameCommand(item, item.Name, newName);
    history.Execute(cmd);
}

// 3. Все працює! CommandHistory, Undo, Логування - всі готові
```

### Додавання Нової Ролі Користувача

```csharp
// 1. Додати в enum
public enum UserRole {
    Admin,
    User,
    Guest,
    Editor  // Нова роль
}

// 2. Налаштувати права
var editor = new FileSystemUser("john", UserRole.Editor);
var perm = new UserPermission(editor, file, 
    AccessRight.Read | AccessRight.Write);

proxy.GrantPermission(perm);
```

## Продуктивність

| Операція | Складність | Час |
|----------|-----------|-----|
| Create File | O(1) | < 1ms |
| Get Size (rекурсивно) | O(n) | ~10ms для 1000 файлів |
| Search | O(n) | ~50ms для 1000 файлів |
| Copy | O(n) | ~100ms для 1000 файлів |
| Serialize to JSON | O(n) | ~200ms для 1000 файлів |

## Безпека

1. **Валідація Входу**
   - Перевірка null значень
   - Перевірка імен файлів
   - Санітація шляхів

2. **Контроль Доступу**
   - Роль-базований контроль (RBAC)
   - Дозволи на рівні ресурсу
   - Логування всіх операцій

3. **Обробка Помилок**
   - 6 спеціальних exception типів
   - Try-catch блоки в критичних місцях
   - Логування помилок

## Масштабованість

### v1.0 (Поточна)
- In-memory сховище
- До ~10000 файлів
- JSON серіалізація

### v1.1
- Кешування розмірів
- Індексація пошуку
- Оптимізовані запити

### v2.0
- Паралельна обробка
- Розподілене сховище
- Cloud синхронізація

## Висновок

Архітектура FileSystem Emulator забезпечує:
- ✓ Чіткий поділ відповідальності
- ✓ Легку розширюваність
- ✓ Правильне застосування design паттернів
- ✓ Напрямку дотримання SOLID принципів
- ✓ Підготовку до майбутнього розвитку
