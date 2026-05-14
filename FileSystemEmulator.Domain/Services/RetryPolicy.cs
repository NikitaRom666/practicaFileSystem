namespace FileSystemEmulator.Domain.Services;

public static class RetryPolicy
{
    public static async Task ExecuteAsync(
        Func<Task> action,
        int maxAttempts = 3,
        int baseDelayMs = 100)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (maxAttempts < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempts));

        if (baseDelayMs < 0)
            throw new ArgumentOutOfRangeException(nameof(baseDelayMs));

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                Console.WriteLine($"[RETRY] Attempt {attempt}/{maxAttempts}");
                await action();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RETRY] Attempt {attempt}/{maxAttempts} failed: {ex.Message}");

                if (attempt == maxAttempts)
                    throw;

                var delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                await Task.Delay(delay);
            }
        }
    }
}