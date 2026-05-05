namespace FileSystemEmulator.Tests;

using Xunit;
using FileSystemEmulator.Domain.Entities;

public class FileItemTests
{
    [Fact]
    public void Constructor_SetsNameCorrectly()
    {
        // Arrange
        var name = "test.txt";

        // Act
        var file = new FileItem(name);

        // Assert
        Assert.Equal(name, file.Name);
    }

    [Fact]
    public void GetSize_ReturnsContentLength()
    {
        // Arrange
        var content = System.Text.Encoding.UTF8.GetBytes("Hello World");
        var file = new FileItem("test.txt", content);

        // Act
        var size = file.GetSize();

        // Assert
        Assert.Equal(content.Length, size);
    }

    [Fact]
    public void Extension_ParsedFromName()
    {
        // Arrange
        var file = new FileItem("document.pdf");

        // Act
        var ext = file.Extension;

        // Assert
        Assert.Equal("pdf", ext);
    }

    [Fact]
    public void SetContent_UpdatesModifiedAt()
    {
        // Arrange
        var file = new FileItem("test.txt");
        var originalModifiedAt = file.ModifiedAt;
        System.Threading.Thread.Sleep(10); // щоб точно відрізнилось

        // Act
        file.Content = System.Text.Encoding.UTF8.GetBytes("new content");

        // Assert
        Assert.True(file.ModifiedAt > originalModifiedAt);
    }

    [Fact]
    public void Equals_SameNameAndSize_ReturnsTrue()
    {
        // Arrange
        var content = System.Text.Encoding.UTF8.GetBytes("test");
        var file1 = new FileItem("file.txt", content);
        var file2 = new FileItem("file.txt", content);

        // Act
        var result = file1 == file2;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Validate_WithValidData_ReturnsTrue()
    {
        // Arrange
        var file = new FileItem("test.txt", System.Text.Encoding.UTF8.GetBytes("content"));

        // Act
        var isValid = file.Validate();

        // Assert
        Assert.True(isValid);
    }
}
