# Class Diagram

## Основні класи

```
IFileSystemItem (interface)
├── FileItem
└── DirectoryItem

ICommand (interface)
├── CopyCommand
├── MoveCommand
└── DeleteCommand

CommandHistory
└── стек команд (до 20)

FileSystemProxy
└── перевірка прав

FileSystemUser
├── роль: Admin, User, Guest
└── права доступу
```

## Взаємозв'язки

- DirectoryItem містить IFileSystemItem (Composite)
- Команди реалізують ICommand
- FileSystemProxy обертає операції
