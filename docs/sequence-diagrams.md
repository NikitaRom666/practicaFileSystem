# Sequence Diagrams

## Copy Operation Flow

```
User -> CopyCommand -> FileSystemProxy -> DirectoryItem
                           |
                           v
                      Check rights
                           |
                           v
                      Execute Copy
```

## Undo Flow

```
User -> CommandHistory.Undo() -> Last Command.Undo()
                                     |
                                     v
                             Restore previous state
```

## Access Control Flow

```
User -> FileSystemProxy -> Check Role & Permissions
                               |
                               +-- Admin? -> Allow
                               +-- User? -> Check specific right
                               +-- Guest? -> Allow only Read
```
