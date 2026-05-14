namespace FileSystemEmulator.Tests;

using System.Text;
using FileSystemEmulator.Domain.Entities;
using FileSystemEmulator.Domain.Exceptions;
using FileSystemEmulator.Domain.Patterns.Creational;
using FileSystemEmulator.Domain.Patterns.Proxy;
using Xunit;

public class ParameterizedTests
{
    private readonly FileSystemItemFactory _factory = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("bad<name")]
    public void FileNameValidation_InvalidNames_Throw(string? name)
    {
        Assert.Throws<ArgumentException>(() => _factory.CreateFile(name!, "txt"));
    }

    [Theory]
    [InlineData("txt", "readme.txt", "txt")]
    [InlineData(".txt", "readme.txt", "txt")]
    [InlineData("", "readme", "")]
    public void ExtensionValidation_NormalizesValues(string extension, string expectedName, string expectedExtension)
    {
        var item = _factory.CreateFile("readme", extension, Encoding.UTF8.GetBytes("hello"));
        var file = Assert.IsType<FileItem>(item);

        Assert.Equal(expectedName, file.Name);
        Assert.Equal(expectedExtension, file.Extension);
    }

    [Theory]
    [InlineData(UserRole.Admin, "Read", true)]
    [InlineData(UserRole.Admin, "Write", true)]
    [InlineData(UserRole.Admin, "Delete", true)]
    [InlineData(UserRole.User, "Read", true)]
    [InlineData(UserRole.User, "Write", true)]
    [InlineData(UserRole.User, "Delete", true)]
    [InlineData(UserRole.Guest, "Read", false)]
    [InlineData(UserRole.Guest, "Write", false)]
    [InlineData(UserRole.Guest, "Delete", false)]
    public void RoleBasedAccess_MatrixWorks(UserRole role, string operation, bool expectedSuccess)
    {
        var history = new FileSystemEmulator.Domain.Patterns.Command.CommandHistory();
        var proxy = new FileSystemProxy(history);
        var user = new FileSystemUser(role.ToString().ToLowerInvariant(), role);
        var file = new FileItem("demo.txt", Encoding.UTF8.GetBytes("content"));
        var destination = new DirectoryItem("dest");

        if (role == UserRole.User)
        {
            if (operation == "Read")
            {
                proxy.GrantPermission(user, file, AccessRight.Read);
                proxy.GrantPermission(user, destination, AccessRight.Write);
            }
            else if (operation == "Write")
            {
                proxy.GrantPermission(user, file, AccessRight.Write);
            }
            else if (operation == "Delete")
            {
                proxy.GrantPermission(user, file, AccessRight.Delete);
            }
        }

        if (expectedSuccess)
        {
            ExecuteOperation(proxy, file, destination, user, operation);
        }
        else
        {
            Assert.Throws<AccessDeniedException>(() => ExecuteOperation(proxy, file, destination, user, operation));
        }
    }

    private static void ExecuteOperation(
        FileSystemProxy proxy,
        FileItem file,
        DirectoryItem destination,
        FileSystemUser user,
        string operation)
    {
        switch (operation)
        {
            case "Read":
                proxy.Copy(file, destination, user);
                break;
            case "Write":
                proxy.WriteContent(file, Encoding.UTF8.GetBytes("updated"), user);
                break;
            case "Delete":
                var parent = new DirectoryItem("parent");
                parent.Add(file);
                proxy.Delete(file, user);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
        }
    }
}