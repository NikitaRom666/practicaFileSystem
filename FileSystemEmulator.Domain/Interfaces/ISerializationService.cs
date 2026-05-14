namespace FileSystemEmulator.Domain.Interfaces;

using FileSystemEmulator.Domain.Entities;

public interface ISerializationService
{
    void SaveToJson(DiskVolume disk, string path);
    DiskVolume LoadFromJson(string path);
}