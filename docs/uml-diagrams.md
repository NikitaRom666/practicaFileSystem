# UML Diagrams - FileSystem Emulator

## Component Diagram - Архітектура

```mermaid
graph TB
    A["📦 Presentation Layer<br/>Program.cs<br/>Demo + Main"]
    
    B["📦 Domain Layer"]
    B1["🔹 Entities<br/>FileItem<br/>DirectoryItem<br/>DiskVolume<br/>User/Permission"]
    B2["🔹 Patterns<br/>Command<br/>Proxy<br/>Composite"]
    B3["🔹 Services<br/>SerializationService<br/>QueryService"]
    B4["🔹 Repository<br/>FileSystemRepository&lt;T&gt;"]
    B5["🔹 Exceptions<br/>AccessDeniedException<br/>FileSystemException"]
    
    C["📦 Tests Layer<br/>xUnit<br/>Moq<br/>27 tests"]
    
    D["💾 Persistence<br/>JSON Files<br/>disk_backup.json"]
    
    A -->|uses| B
    A -->|runs| C
    
    B --> B1
    B --> B2
    B --> B3
    B --> B4
    B --> B5
    
    C -->|mocks| B
    B3 -->|serialize/deserialize| D
    B4 -->|CRUD operations| B1
    B2 -->|uses| B1
```

## Use Case Diagram - Користувачи та операції

```mermaid
usecase UC1 as "Створити файл"
usecase UC2 as "Створити каталог"
usecase UC3 as "Копіювати файл/каталог"
usecase UC4 as "Переміститити файл/каталог"
usecase UC5 as "Видалити файл/каталог"
usecase UC6 as "Пошук файлів"
usecase UC7 as "Скасувати операцію (Undo)"
usecase UC8 as "Надати права доступу"
usecase UC9 as "Зберегти в JSON"
usecase UC10 as "Завантажити з JSON"
usecase UC11 as "Перевірити систему"

actor Admin as "Адміністратор\n(Всі права)"
actor User as "Звичайний користувач\n(Читання/Запис)"
actor Guest as "Гість\n(Тільки читання)"

Admin --> UC1
Admin --> UC2
Admin --> UC3
Admin --> UC4
Admin --> UC5
Admin --> UC6
Admin --> UC7
Admin --> UC8
Admin --> UC9
Admin --> UC10
Admin --> UC11

User --> UC1
User --> UC2
User --> UC3
User --> UC4
User --> UC6
User --> UC7

Guest --> UC6
Guest --> UC7
Guest --> UC11
```

## State Machine - Команда (Command Lifecycle)

```mermaid
stateDiagram-v2
    [*] --> Created: new CopyCommand()
    Created --> Executed: Execute()
    Executed --> Undone: Undo()
    Undone --> Executed: Execute() again
    Executed --> [*]: Cleanup
    Undone --> [*]: Cleanup
```

## Deployment Diagram - Розташування компонентів

```mermaid
graph LR
    A["💻 Developer Machine<br/>.NET 9 SDK<br/>Visual Studio Code"]
    
    B["📦 Build Artifacts<br/>Domain.dll<br/>App.dll<br/>Tests.dll"]
    
    C["☁️ GitHub Repository<br/>NikitaRom666/practicaFileSystem<br/>Main branch"]
    
    D["📄 Documentation<br/>README.md<br/>Class Diagrams<br/>Sequence Diagrams<br/>Test Reports"]
    
    A -->|dotnet build| B
    B -->|dotnet run| A
    A -->|git push| C
    C -->|contains| D
    C -->|contains| B
```

## Interaction Overview - Послідовність роботи системи

```mermaid
sequenceDiagram
    actor User
    participant App as Application
    participant Domain as Domain Layer
    participant Proxy as Proxy/Security
    participant History as Command History
    participant Storage as JSON Storage
    
    Note over App: 1. Ініціалізація
    User->>App: Start application
    App->>Domain: Create FileSystem
    
    Note over App: 2. Основні операції
    User->>App: Copy readme.txt
    App->>Proxy: Check permissions
    Proxy->>Proxy: Verify access rights
    App->>History: Execute(CopyCommand)
    History->>Domain: Clone and add file
    
    Note over App: 3. Undo операція
    User->>App: Undo
    App->>History: Undo()
    History->>Domain: Remove cloned file
    
    Note over App: 4. Пошук
    User->>App: Search files
    App->>Domain: Search with LINQ
    Domain->>App: Results
    
    Note over App: 5. Збереження
    User->>App: Save to JSON
    App->>Storage: Serialize(DiskVolume)
    Storage->>Storage: Create disk_backup.json
    
    Note over App: 6. Завантаження
    User->>App: Load from JSON
    App->>Storage: Deserialize(disk_backup.json)
    Storage->>Domain: Restore FileSystem
    
    Note over App: 7. Тестування
    App->>App: Run xUnit tests
    App->>App: 27/27 passed ✓
```

## Information Model - Дані та відносини

```mermaid
erDiagram
    DISK_VOLUME {
        guid id PK
        string label
        long totalSpace
        long usedSpace
    }
    
    FILE_SYSTEM_ITEM {
        guid id PK
        string name
        datetime createdAt
        datetime modifiedAt
        string type
        guid parentId FK
    }
    
    FILE_ITEM {
        guid id PK
        byte[] content
        string extension
        long size
    }
    
    DIRECTORY_ITEM {
        guid id PK
        int childCount
    }
    
    FILE_SYSTEM_USER {
        guid id PK
        string name
        string role
    }
    
    USER_PERMISSION {
        guid id PK
        guid userId FK
        guid itemId FK
        int accessRights
    }
    
    COMMAND_HISTORY {
        guid id PK
        string commandType
        datetime executedAt
        boolean canUndo
    }
    
    DISK_VOLUME ||--|| FILE_SYSTEM_ITEM : "contains"
    FILE_SYSTEM_ITEM ||--|| FILE_ITEM : "is"
    FILE_SYSTEM_ITEM ||--|| DIRECTORY_ITEM : "is"
    FILE_SYSTEM_ITEM ||--|| FILE_SYSTEM_ITEM : "parent_child"
    FILE_SYSTEM_USER ||--|| USER_PERMISSION : "has"
    FILE_SYSTEM_ITEM ||--|| USER_PERMISSION : "assigned_to"
    COMMAND_HISTORY ||--|| FILE_SYSTEM_ITEM : "operates_on"
```

## Class Hierarchy - Повна ієрархія класів

```mermaid
graph TD
    A["System.Object"]
    A --> B["FileSystemItem<br/>&lt;&lt;abstract&gt;&gt;"]
    B --> C["FileItem"]
    B --> D["DirectoryItem"]
    
    A --> E["DiskVolume"]
    
    A --> F["FileSystemUser"]
    A --> G["UserPermission"]
    A --> H["AccessRight<br/>&lt;&lt;enum&gt;&gt;"]
    A --> I["UserRole<br/>&lt;&lt;enum&gt;&gt;"]
    
    A --> J["ICommand<br/>&lt;&lt;interface&gt;&gt;"]
    J --> K["CopyCommand"]
    J --> L["MoveCommand"]
    J --> M["DeleteCommand"]
    
    A --> N["CommandHistory"]
    
    A --> O["IFileSystemProxy<br/>&lt;&lt;interface&gt;&gt;"]
    O --> P["FileSystemProxy"]
    
    A --> Q["FileSystemRepository&lt;T&gt;"]
    
    A --> R["Exception"]
    R --> S["AccessDeniedException"]
    R --> T["FileSystemException"]
```

## Package Diagram - Модулі та залежності

```mermaid
graph LR
    subgraph App["📦 FileSystemEmulator.App"]
        A1["Program.cs"]
    end
    
    subgraph Domain["📦 FileSystemEmulator.Domain"]
        D1["Entities"]
        D2["Patterns"]
        D3["Services"]
        D4["Repository"]
        D5["Exceptions"]
        D6["Interfaces"]
    end
    
    subgraph Tests["📦 FileSystemEmulator.Tests"]
        T1["CommandHistoryTests"]
        T2["CommandTests"]
        T3["DirectoryItemTests"]
        T4["FileItemTests"]
        T5["FileSystemProxyTests"]
    end
    
    App -->|references| Domain
    Tests -->|references| Domain
    Tests -->|uses| A1
    
    D2 -->|uses| D1
    D3 -->|uses| D1
    D4 -->|uses| D1
    D5 -->|uses| D6
```
