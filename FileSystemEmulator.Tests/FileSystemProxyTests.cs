namespace FileSystemEmulator.Tests;

using System.Text;
using Xunit;
using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Exceptions;
using FileSystemEmulator.Domain.Patterns.Command;
using FileSystemEmulator.Domain.Patterns.Proxy;
using Moq;

public class FileSystemProxyTests
{
    [Fact]
    public void GetItem_AdminUser_ReturnsNullWithoutException()
    {
        var proxy = new FileSystemProxy();
        var admin = new FileSystemUser("admin", UserRole.Admin);

        var result = proxy.GetItem("path/to/secret.txt", admin);

        Assert.Null(result);
    }

    [Fact]
    public void GetItem_UserWithoutRight_ThrowsAccessDeniedException()
    {
        var proxy = new FileSystemProxy();
        var user = new FileSystemUser("bob", UserRole.User);

        Assert.Throws<AccessDeniedException>(() =>
            proxy.GetItem("path/to/secret.txt", user));
    }

    [Fact]
    public void Delete_AdminUser_AlwaysSucceeds()
    {
        var history = new CommandHistory();
        var proxy = new FileSystemProxy(history);
        var admin = new FileSystemUser("admin", UserRole.Admin);
        var dir = new DirectoryItem("folder");
        var file = new FileItem("file.txt");
        dir.Add(file);

        proxy.Delete(file, admin);

        Assert.Empty(dir.Children);
    }

    [Fact]
    public void Undo_AdminUser_ReplaysMockCommand()
    {
        var history = new CommandHistory();
        var proxy = new FileSystemProxy(history);
        var admin = new FileSystemUser("admin", UserRole.Admin);
        var mockCommand = new Mock<ICommand>();
        mockCommand.SetupGet(c => c.Description).Returns("Mock command");

        history.Execute(mockCommand.Object);

        proxy.Undo(admin);

        mockCommand.Verify(c => c.Execute(), Times.Once);
        mockCommand.Verify(c => c.Undo(), Times.Once);
    }

    [Fact]
    public void Undo_NonAdminUser_ThrowsAccessDeniedException()
    {
        var history = new CommandHistory();
        var proxy = new FileSystemProxy(history);
        var user = new FileSystemUser("bob", UserRole.User);
        var mockCommand = new Mock<ICommand>();
        mockCommand.SetupGet(c => c.Description).Returns("Mock command");

        history.Execute(mockCommand.Object);

        Assert.Throws<AccessDeniedException>(() => proxy.Undo(user));
        mockCommand.Verify(c => c.Undo(), Times.Never);
    }

    [Fact]
    public void GrantPermission_UserCanWriteContent()
    {
        var proxy = new FileSystemProxy();
        var user = new FileSystemUser("alice", UserRole.User);
        var file = new FileItem("document.txt");
        var content = Encoding.UTF8.GetBytes("updated");

        proxy.GrantPermission(user, file, AccessRight.Write);
        proxy.WriteContent(file, content, user);

        Assert.Equal(content, file.Content);
    }

    [Fact]
    public void Copy_SameSourceAndDestination_ThrowsInvalidFileSystemOperationException()
    {
        var proxy = new FileSystemProxy();
        var admin = new FileSystemUser("admin", UserRole.Admin);
        var folder = new DirectoryItem("folder");

        Assert.Throws<InvalidFileSystemOperationException>(() => proxy.Copy(folder, folder, admin));
    }

    [Fact]
    public void Copy_WhenDiskIsFull_ThrowsDiskFullException()
    {
        var proxy = new FileSystemProxy();
        var admin = new FileSystemUser("admin", UserRole.Admin);
        var disk = new DiskVolume("C:\\", 4);
        var destination = disk.Root;
        var file = new FileItem("big.txt", Encoding.UTF8.GetBytes("12345"));

        Assert.Throws<DiskFullException>(() => proxy.Copy(file, destination, admin));
    }
}
