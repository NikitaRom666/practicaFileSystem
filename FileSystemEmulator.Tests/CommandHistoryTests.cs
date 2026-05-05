namespace FileSystemEmulator.Tests;

using Xunit;
using FileSystemEmulator.Domain.Patterns.Command;
using FileSystemEmulator.Domain.Entities;

public class CommandHistoryTests
{
    [Fact]
    public void Execute_RunsCommand()
    {
        // Arrange
        var history = new CommandHistory();
        var mockCmd = new MockCommand();

        // Act
        history.Execute(mockCmd);

        // Assert
        Assert.True(mockCmd.Executed);
    }

    [Fact]
    public void Undo_RevertsLastCommand()
    {
        // Arrange
        var history = new CommandHistory();
        var mockCmd = new MockCommand();
        history.Execute(mockCmd);
        mockCmd.Executed = false;

        // Act
        history.Undo();

        // Assert
        Assert.True(mockCmd.Undone);
    }

    [Fact]
    public void Undo_WhenEmpty_ThrowsException()
    {
        // Arrange
        var history = new CommandHistory();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => history.Undo());
    }

    [Fact]
    public void CanUndo_Empty_ReturnsFalse()
    {
        // Arrange
        var history = new CommandHistory();

        // Act & Assert
        Assert.False(history.CanUndo);
    }

    [Fact]
    public void CanUndo_WithCommands_ReturnsTrue()
    {
        // Arrange
        var history = new CommandHistory();
        var mockCmd = new MockCommand();
        history.Execute(mockCmd);

        // Act & Assert
        Assert.True(history.CanUndo);
    }

    // Допоміжний mock клас для тестування
    private class MockCommand : ICommand
    {
        public bool Executed { get; set; }
        public bool Undone { get; set; }

        public string Description => "Mock Command";

        public void Execute() => Executed = true;
        public void Undo() => Undone = true;
    }
}
