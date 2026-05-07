# Результати тестування

## Загальний результат

Passed: 27 | Failed: 0 | Duration: ~44ms
Framework: xUnit + Moq | Target: net8.0

## По групах тестів

| Група | Кількість тестів | Результат |
|-------|------------------|-----------|
| CommandTests | 8 | Passed |
| CommandHistoryTests | 4 | Passed |
| DirectoryItemTests | 6 | Passed |
| FileItemTests | 5 | Passed |
| FileSystemProxyTests | 4 | Passed |

## Що покривають тести

- Виконання та скасування команд (Copy, Move, Delete)
- Обмеження стеку CommandHistory (до 20 команд)
- Рекурсивний розрахунок розміру каталогу
- Пошук файлів по шаблону
- Клонування файлів (Clone)
- Перевірка прав доступу через Proxy
- AccessDeniedException при порушенні прав
- Права адміністратора без обмежень
