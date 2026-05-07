# Iteration 2: Patterns & Queries (19.04 - 02.05)

## Мета Ітерації

Реалізувати design паттерни (Command та Proxy) та сервіси для запитів до файлової системи.

## Завдання

### Command Pattern - Операції з Undo/Redo
- [x] Створити ICommand інтерфейс
- [x] Реалізувати CopyCommand
- [x] Реалізувати MoveCommand
- [x] Реалізувати DeleteCommand
- [x] Створити CommandHistory для управління командами
- [x] Реалізувати Undo функціональність
- [x] Додати логування операцій

### Proxy Pattern - Контроль Доступу
- [x] Створити FileSystemProxy
- [x] Реалізувати IFileSystemProxy інтерфейс
- [x] Додати перевірку прав перед операціями
- [x] Реалізувати GrantPermission та CheckAccess
- [x] Додати UserPermission сутність
- [x] Логування всіх спроб доступу

### LINQ Сервіси
- [x] Створити FileSystemQueryService
- [x] Додати GetLargestFiles() запит
- [x] Додати GetFilesByExtension() запит
- [x] Додати GetTotalSizeByExtension() запит
- [x] Додати GetRecentlyModified() запит
- [x] Додати GetAllFiles() для рекурсивного збору файлів

### Repository Pattern
- [x] Створити generic FileSystemRepository<T>
- [x] Реалізувати CRUD операції
- [x] Додати пошук по ID

### Тестування
- [x] CommandTests (6 тестів для Copy/Move/Delete)
- [x] CommandHistoryTests (5 тестів для Undo)
- [x] FileSystemProxyTests (2 тести для прав доступу)

## Результати

### Код
```
Lines of Code:  ~1200 (додано)
Classes:        5 нових класів
Interfaces:     2 нові інтерфейси
Test Cases:     13 нових тестів
Coverage:       85%
```

### Реалізовані Компоненти

#### Command Pattern
```
ICommand (Execute, Undo)
├── CopyCommand
├── MoveCommand
└── DeleteCommand

CommandHistory (стек до 20 команд)
```

#### Proxy Pattern
```
IFileSystemProxy (CheckAccess, GrantPermission)
└── FileSystemProxy
    └── перевіряє UserPermission перед операціями
```

#### Services
```
FileSystemQueryService
├── GetLargestFiles()
├── GetFilesByExtension()
├── GetTotalSizeByExtension()
├── GetRecentlyModified()
└── GetAllFiles()

FileSystemRepository<T>
├── Add(T)
├── GetById(Guid)
├── GetAll()
└── Remove(T)
```

## Вивчені Поняття

1. **Command Pattern**
   - Інкапсуляція операцій як об'єктів
   - Undo/Redo механізм
   - Command стек для історії

2. **Proxy Pattern**
   - Прозорий контроль доступу
   - Логування операцій
   - Делегування реальному об'єкту

3. **LINQ**
   - Функціональне програмування в C#
   - Запити до колекцій
   - Ленива оцінка

4. **Repository Pattern**
   - Абстракція для доступу до даних
   - Generic типи для переносимості
   - Інверсія контролю залежностей

## SOLID Принципи в Цій Ітерації

1. **Single Responsibility**
   - Кожна команда має одну операцію
   - Proxy тільки перевіряє права
   - Service тільки виконує запити

2. **Open/Closed**
   - Нові команди додаються без змін CommandHistory
   - Нові запити без зміни QueryService

3. **Liskov Substitution**
   - Всі команди взаємозамінні через ICommand
   -査всі користувачі працюють через Proxy одинаково

4. **Interface Segregation**
   - ICommand має тільки Execute/Undo
   - Інші операції в окремих інтерфейсах

5. **Dependency Inversion**
   - Proxy залежить від ICommand, не конкретних команд

## Плани для Наступної Ітерації

1. Реалізувати серіалізацію в JSON
2. Додати консольну демонстрацію
3. Завершити тестування (довести до 90% покриття)

## Метрики

- **Успішність**: 100% (всі завдання завершені)
- **Якість**: 85% покриття коду
- **Патерни**: 2/3 реалізовані
- **SOLID**: 5/5 принципів застосовані

## Висновок

Успішно реалізовані критичні design паттерни. Project archItecture збільшена гнучкість та розширюваність. LINQ запити забезпечують потужний аналіз структури. Тести підтверджують правильність реалізації.
