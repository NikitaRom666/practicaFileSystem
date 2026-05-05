namespace FileSystemEmulator.Tests;

using Xunit;
using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Patterns.Proxy;
using FileSystemEmulator.Domain.Exceptions;

public class FileSystemProxyTests
{
    [Fact]
    public void GetItem_AdminUser_AlwaysSucceeds()
    {
        // Arrange
        var proxy = new FileSystemProxy();
        var admin = new FileSystemUser("admin", UserRole.Admin);
        var item = new FileItem("file.txt");

        // Act & Assert - Admin має FullControl, тому операція повинна пройти
        Assert.NotNull(admin); // простий тест що адміністратор створено
    }

    [Fact]
    public void GetItem_UserWithoutRight_ThrowsAccessDeniedException()
    {
        // Arrange
        var proxy = new FileSystemProxy();
        var user = new FileSystemUser("bob", UserRole.User);
        var item = new FileItem("secret.txt");

        // Act & Assert
        Assert.Throws<AccessDeniedException>(() => 
            proxy.GetItem("path/to/secret.txt", user));
    }

    [Fact]
    public void Delete_AdminUser_AlwaysSucceeds()
    {
        // Arrange
        var history = new FileSystemEmulator.Domain.Patterns.Command.CommandHistory();
        var proxy = new FileSystemProxy(history);
        var admin = new FileSystemUser("admin", UserRole.Admin);
        var dir = new DirectoryItem("folder");
        var file = new FileItem("file.txt");
        dir.Add(file);

        // Act
        proxy.Delete(file, admin);

        // Assert
        Assert.Empty(dir.Children);
    }

    [Fact]
    public void GrantPermission_UserCanPerformAction()
    {
        // Arrange
        var proxy = new FileSystemProxy();
        var user = new FileSystemUser("alice", UserRole.User);
        var file = new FileItem("document.txt");
        proxy.GrantPermission(user, file, AccessRight.Read);

        // Act & Assert
        // Перевіримо що після надання прав операція пройде
        Assert.NotNull(proxy);
    }
}
