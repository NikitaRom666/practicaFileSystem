# Changelog

## [1.3.0] — 2026-05-15
### Added
- Factory Method та Singleton патерни
- Observer (IFileSystemObserver, ConsoleLogger, AuditLog)
- Strategy для пошуку (за іменем, розширенням, шаблоном)
- Decorator (CompressedFileDecorator, EncryptedFileDecorator)
- Facade (FileSystemFacade — єдина точка входу)
- RetryPolicy з експоненційною затримкою
- Параметризовані xUnit тести (Theory/InlineData)
- Mermaid UML-діаграма класів

## [1.2.0] — 2026-05-05
### Added
- Серіалізація JSON (SerializationService, DTO)
- 27 xUnit тестів з Moq
- LINQ-сервіс для пошуку та статистики
- Generic repository для файлових елементів
...