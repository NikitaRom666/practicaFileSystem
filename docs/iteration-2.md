# Ітерація 2 — Патерни та запити

## Що зроблено

- Реалізовано ICommand, CopyCommand, MoveCommand, DeleteCommand
- Реалізовано CommandHistory зі стеком до 20 команд
- Реалізовано FileSystemProxy та контроль доступу
- Додано ролі UserRole та права AccessRight з [Flags]
- Реалізовано FileSystemRepository<T>
- Написано LINQ запити в FileSystemQueryService
- Додано власні винятки FileSystemException, AccessDeniedException

## Результат

Система отримала можливість виконувати операції з файлами (Copy, Move, Delete) та скасовувати їх, а також контролювати доступ за допомогою ролей.
