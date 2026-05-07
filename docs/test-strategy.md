# Test Strategy

## Рівні тестування

### Unit Tests (xUnit + Moq)
- Тестування окремих методів
- Використання Moq для мокування залежностей
- Покриття ~ 90%

### Сценарії тестування

1. **File Operations**
   - Створення файлу
   - Читання вмісту
   - Видалення файлу

2. **Directory Operations**
   - Створення каталогу
   - Додавання файлів та підкаталогів
   - Видалення з вложеними елементами

3. **Command Pattern**
   - Виконання Copy, Move, Delete
   - Undo останньої операції
   - Undo кількох операцій послідовно

4. **Proxy Pattern**
   - Admin має всі права
   - User має обмежені права
   - Guest має тільки Read
   - AccessDeniedException на заборонених операціях

5. **Search & Filter**
   - Пошук по імені
   - Пошук по розширенню
   - Рекурсивний пошук в підкаталогах

6. **Serialization**
   - Збереження структури у JSON
   - Відновлення з JSON
   - Збереження команд

## Tool & Framework

- **Test Framework:** xUnit
- **Mocking:** Moq
- **Assertions:** Fluent Assertions (опційно)
