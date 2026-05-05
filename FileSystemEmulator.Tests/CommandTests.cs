namespace FileSystemEmulator.Tests;

using Xunit;
using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Patterns.Command;

public class CommandTests
{
    [Fact]
    public void CopyCommand_Execute_CreatesDeepCopy()
    {
        // Arrange
        var source = new FileItem("original.txt", System.Text.Encoding.UTF8.GetBytes("content"));
        var dest = new DirectoryItem("destFolder");
        var copyCmd = new CopyCommand(source, dest);

        // Act
        copyCmd.Execute();

        // Assert
        Assert.Single(dest.Children);
        Assert.Equal("original.txt", dest.Children[0].Name);
    }

    [Fact]
    public void CopyCommand_Undo_RemovesCopiedItem()
    {
        // Arrange
        var source = new FileItem("original.txt", System.Text.Encoding.UTF8.GetBytes("content"));
        var dest = new DirectoryItem("destFolder");
        var copyCmd = new CopyCommand(source, dest);
        copyCmd.Execute();

        // Act
        copyCmd.Undo();

        // Assert
        Assert.Empty(dest.Children);
    }

    [Fact]
    public void MoveCommand_Execute_MovesItem()
    {
        // Arrange
        var source = new DirectoryItem("sourceFolder");
        var file = new FileItem("file.txt");
        var dest = new DirectoryItem("destFolder");
        source.Add(file);

        var moveCmd = new MoveCommand(file, dest);

        // Act
        moveCmd.Execute();

        // Assert
        Assert.Empty(source.Children);
        Assert.Single(dest.Children);
        Assert.Equal("file.txt", dest.Children[0].Name);
    }

    [Fact]
    public void DeleteCommand_Execute_RemovesItem()
    {
        // Arrange
        var dir = new DirectoryItem("folder");
        var file = new FileItem("file.txt");
        dir.Add(file);

        var deleteCmd = new DeleteCommand(file);

        // Act
        deleteCmd.Execute();

        // Assert
        Assert.Empty(dir.Children);
    }

    [Fact]
    public void DeleteCommand_Undo_RestoresItem()
    {
        // Arrange
        var dir = new DirectoryItem("folder");
        var file = new FileItem("file.txt");
        dir.Add(file);

        var deleteCmd = new DeleteCommand(file);
        deleteCmd.Execute();

        // Act
        deleteCmd.Undo();

        // Assert
        Assert.Single(dir.Children);
        Assert.Equal("file.txt", dir.Children[0].Name);
    }
}
