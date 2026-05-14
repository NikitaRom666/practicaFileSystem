namespace FileSystemEmulator.Domain.Entities;

[Flags]
public enum AccessRight
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4,
    Delete = 8,
    FullControl = 15
}
