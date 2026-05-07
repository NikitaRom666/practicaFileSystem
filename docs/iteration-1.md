# Iteration 1: Foundation & Design (05.04 - 18.04)

## Мета Ітерації

Встановити архітектурну основу проекту та реалізувати базові сутності файлової системи з використанням Composite паттерну.

## Завдання

### Архітектура та Структура
- [x] Створити 3-tier архітектуру (Domain, Application, Tests)
- [x] Налаштувати .NET 9 проект структури
- [x] Визначити базові інтерфейси (IFileSystemItem, ISearchable, IPrintable)
- [x] Створити базові винятки

### Основні Сутності
- [x] FileSystemItem - базовий абстрактний клас
- [x] FileItem - представлення файлу
- [x] DirectoryItem - представлення папки (Composite)
- [x] DiskVolume - представлення диска
- [x] FileSystemUser - користувач системи
- [x] UserRole - ролі користувачів
- [x] AccessRight - права доступу (flags enum)

### Реалізація Composite Паттерну
- [x] FileSystemItem як компонент
- [x] DirectoryItem як контейнер
- [x] FileItem як лист
- [x] Методи GetSize(), Print(), Search()
- [x] Рекурсивна обробка структури

### Тестування
- [x] FileItemTests (6 тестів)
- [x] DirectoryItemTests (8 тестів)
- [x] Базові тести структури
- [x] Тести рекурсії

## Результати

### Код
```
Lines of Code:  ~800
Classes:        7 основних класів
Interfaces:     3 інтерфейси
Test Cases:     14 тестів
Coverage:       75%
```

### Завершені Файли
- ✓ Entities/FileSystemItem.cs
- ✓ Entities/FileItem.cs
- ✓ Entities/DirectoryItem.cs
- ✓ Entities/DiskVolume.cs
- ✓ Entities/FileSystemUser.cs
- ✓ Entities/UserRole.cs
- ✓ Entities/AccessRight.cs
- ✓ Interfaces/IFileSystemItem.cs
- ✓ Interfaces/ISearchable.cs
- ✓ Interfaces/IPrintable.cs
- ✓ Exceptions/FileSystemExceptions.cs
- ✓ FileItemTests.cs
- ✓ DirectoryItemTests.cs

## Вивчені Поняття

1. **Design Паттерни**
   - Composite паттерн для древовидних структур
   - Як організувати ієрархію об'єктів

2. **Object-Oriented Principles**
   - Наслідування vs Композиція
   - Поліморфізм через інтерфейси
   - Абстрактні класи як база

3. **C# Особливості**
   - Abstract базові класи
   - Interface контракти
   - Енуми та Flags
   - Records для даних

## Плани для Наступної Ітерації

1. Реалізувати Command паттерн для операцій з Undo
2. Додати Proxy для контролю доступу
3. Реалізувати LINQ запити

## Метрики

- **Успішність**: 100% (всі завдання завершені)
- **Якість**: 75% покриття коду
- **Документація**: Базова документація архітектури
- **Риск**: Низький - стабільна основа

## Висновок

Успішно встановлена архітектурна основа з правильним застосуванням Composite паттерну. Базові сутності готові для наступних ітерацій. Тести забезпечують надійність основи.
