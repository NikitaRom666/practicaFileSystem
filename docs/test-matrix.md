# Матриця тестування

| Тест | Компонент | Результат |
|------|-----------|-----------|
| CopyCommand_Execute_AddsItemToDestination | CopyCommand | Passed |
| CopyCommand_Undo_RemovesItemFromDestination | CopyCommand | Passed |
| CopyCommand_Execute_ClonesItem | CopyCommand | Passed |
| MoveCommand_Execute_MovesItemFromSourceToDestination | MoveCommand | Passed |
| MoveCommand_Undo_RestoresItemToSourceParent | MoveCommand | Passed |
| MoveCommand_Undo_RestoresIndexInSource | MoveCommand | Passed |
| DeleteCommand_Execute_RemovesItem | DeleteCommand | Passed |
| DeleteCommand_Undo_RestoresItemAndIndex | DeleteCommand | Passed |
| CommandHistory_Execute_AddsCommandToStack | CommandHistory | Passed |
| CommandHistory_Undo_RemovesLastCommand | CommandHistory | Passed |
| CommandHistory_CanUndo_ReturnsFalseWhenEmpty | CommandHistory | Passed |
| CommandHistory_Execute_EnforcesMaxSize20 | CommandHistory | Passed |
| DirectoryItem_Add_AddsChildItem | DirectoryItem | Passed |
| DirectoryItem_Remove_RemovesChildItem | DirectoryItem | Passed |
| DirectoryItem_Search_FindsItemByName | DirectoryItem | Passed |
| DirectoryItem_GetSize_CalculatesRecursiveSize | DirectoryItem | Passed |
| DirectoryItem_GetSize_ReturnsZeroForEmptyDirectory | DirectoryItem | Passed |
| DirectoryItem_Validate_ChecksStructure | DirectoryItem | Passed |
| FileItem_Create_WithContent | FileItem | Passed |
| FileItem_GetExtension_ReturnsFileExtension | FileItem | Passed |
| FileItem_Clone_CreatesNewIdAndCopy | FileItem | Passed |
| FileItem_Touch_UpdatesModifiedTime | FileItem | Passed |
| FileItem_GetSize_EqualsContentLength | FileItem | Passed |
| FileSystemProxy_GrantPermission_AssignsRights | FileSystemProxy | Passed |
| FileSystemProxy_Admin_HasFullAccess | FileSystemProxy | Passed |
| FileSystemProxy_User_DeniedWrite | FileSystemProxy | Passed |
| FileSystemProxy_Guest_DeniedWrite | FileSystemProxy | Passed |
