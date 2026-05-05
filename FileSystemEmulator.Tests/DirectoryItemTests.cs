namespace FileSystemEmulator.Tests;

using Xunit;
using FileSystemEmulator.Domain.Entities;

public class DirectoryItemTests
{
    [Fact]
    public void Add_IncreasesChildCount()
    {
        // Arrange
        var dir = new DirectoryItem("folder");
        var file = new FileItem("test.txt");

        // Act
        dir.Add(file);

        // Assert
        Assert.Single(dir.Children);
    }

    [Fact]
    public void GetSize_ReturnsSumOfChildren()
    {
        // Arrange
        var dir = new DirectoryItem("folder");
        var file1 = new FileItem("file1.txt", System.Text.Encoding.UTF8.GetBytes("hello"));
        var file2 = new FileItem("file2.txt", System.Text.Encoding.UTF8.GetBytes("world"));
        dir.Add(file1);
        dir.Add(file2);

        // Act
        var size = dir.GetSize();

        // Assert
        Assert.Equal(10, size); // "hello" + "world" = 10 байт
    }

    [Fact]
    public void Search_FindsByName()
    {
        // Arrange
        var dir = new DirectoryItem("root");
        var file = new FileItem("important.txt");
        dir.Add(file);

        // Act
        var results = dir.Search("import").ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("important.txt", results[0].Name);
    }

    [Fact]
    public void Remove_DecreaseChildCount()
    {
        // Arrange
        var dir = new DirectoryItem("folder");
        var file = new FileItem("test.txt");
        dir.Add(file);

        // Act
        dir.Remove(file);

        // Assert
        Assert.Empty(dir.Children);
    }

    [Fact]
    public void Indexer_ReturnsCorrectChild()
    {
        // Arrange
        var dir = new DirectoryItem("folder");
        var file = new FileItem("test.txt");
        dir.Add(file);

        // Act
        var found = dir["test.txt"];

        // Assert
        Assert.NotNull(found);
        Assert.Equal("test.txt", found.Name);
    }

    [Fact]
    public void Add_SameName_ThrowsException()
    {
        // Arrange
        var dir = new DirectoryItem("folder");
        var file1 = new FileItem("test.txt");
        var file2 = new FileItem("test.txt");
        dir.Add(file1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => dir.Add(file2));
    }

    [Fact]
    public void GetSize_Recursive_IncludesSubdirectories()
    {
        // Arrange
        var root = new DirectoryItem("root");
        var subdir = new DirectoryItem("sub");
        var file = new FileItem("file.txt", System.Text.Encoding.UTF8.GetBytes("hello"));
        root.Add(subdir);
        subdir.Add(file);

        // Act
        var size = root.GetSize();

        // Assert
        Assert.Equal(5, size);
    }
}
