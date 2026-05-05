namespace FileSystemEmulator.Domain.Interfaces;

/// <summary>
/// Інтерфейс для виводу в консоль
/// </summary>
public interface IPrintable
{
    void Print(int indent = 0);
    string GetTreeRepresentation();
}
