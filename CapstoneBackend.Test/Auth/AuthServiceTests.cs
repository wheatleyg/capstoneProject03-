using CapstoneBackend.Auth;
using CapstoneBackend.Auth.Models;
using CapstoneBackend.Utilities.Exceptions;
using Moq;

using CapstoneBackend.Core.Repositories;
using CapstoneBackend.Core.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace CapstoneBackend.Test.Auth;

public class AuthServiceTests
{
    [Fact]
    public async Task Login_WhenCalledWithoutUsername_ThrowsBadRequest()
    {
        //arrange
        var repo = new Mock<IAuthRepository>();
        var wrapper = new Mock<IAuthServiceWrapper>();
        var service = new AuthService(repo.Object, wrapper.Object);

        var badLogin = new Login
        {
            Username = "",
            Password = "it's a secret"
        };
        //act
        var test = async () => await service.Login(badLogin);
        //assert
        await Assert.ThrowsAsync<BadRequestException>(test);
    }
    
    [Fact]
    public async Task Login_WhenCalledWithoutPassword_ThrowsBadRequest()
    {
        //arrange
        var repo = new Mock<IAuthRepository>();
        var wrapper = new Mock<IAuthServiceWrapper>();
        var service = new AuthService(repo.Object, wrapper.Object);

        var badLogin = new Login
        {
            Username = "myUsername",
            Password = ""
        };
        //act
        var test = async () => await service.Login(badLogin);
        //assert
        await Assert.ThrowsAsync<BadRequestException>(test);
    }
    
    [Fact]
    public async Task Login_UserDoesntExist_ThrowsUnauthenticated()
    {
        //arrange
        var repo = new Mock<IAuthRepository>();
        repo.Setup(r => r.GetUserByUsername(It.IsAny<string>()))
            .ReturnsAsync((DatabaseUser) null!);
        
        var wrapper = new Mock<IAuthServiceWrapper>();
        var service = new AuthService(repo.Object, wrapper.Object);

        var badLogin = new Login
        {
            Username = "myUsername",
            Password = "it's a secret"
        };
        //act
        var test = async () => await service.Login(badLogin);
        //assert
        await Assert.ThrowsAsync<UnauthenticatedException>(test);
    }
    
    [Fact]
    public async Task Login_UserIsDeleted_ThrowsUnauthenticated()
    {
        //arrange
        var repo = new Mock<IAuthRepository>();
        repo.Setup(r => r.GetUserByUsername(It.IsAny<string>()))
            .ReturnsAsync(new DatabaseUser
            {
                IsDeleted = true
            });
        
        var wrapper = new Mock<IAuthServiceWrapper>();
        var service = new AuthService(repo.Object, wrapper.Object);

        var badLogin = new Login
        {
            Username = "myUsername",
            Password = "it's a secret"
        };
        //act
        var test = async () => await service.Login(badLogin);
        //assert
        await Assert.ThrowsAsync<UnauthenticatedException>(test);
    }
    
    [Fact]
    public async Task Login_UserPasswordIsWrong_ThrowsUnauthenticated()
    {
        //arrange
        var repo = new Mock<IAuthRepository>();
        var wrapper = new Mock<IAuthServiceWrapper>();
        repo.Setup(r => r.GetUserByUsername(It.IsAny<string>()))
            .ReturnsAsync(new DatabaseUser
            {
                IsDeleted = false,
                PasswordHash = [1],
                PasswordSalt = [1]
            });
        wrapper.Setup(w => w.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(false);
        
        var service = new AuthService(repo.Object, wrapper.Object);

        var badLogin = new Login
        {
            Username = "myUsername",
            Password = "it's a secret"
        };
        //act
        var test = async () => await service.Login(badLogin);
        //assert
        await Assert.ThrowsAsync<UnauthenticatedException>(test);
    }
    
    [Fact]
    public async Task Login_IsSuccessful_ReturnsAuthToken()
    {
        //arrange
        var hash = new byte[] { 1 };
        var salt = new byte[] { 2 };
            
        var repo = new Mock<IAuthRepository>();
        repo.Setup(r => r.GetUserByUsername(It.IsAny<string>()))
            .ReturnsAsync(new DatabaseUser
            {
                IsDeleted = false,
                PasswordHash = hash,
                PasswordSalt = salt
            });
        
        var wrapper = new Mock<IAuthServiceWrapper>();
        wrapper.Setup(w => w.VerifyPasswordHash(It.IsAny<string>(), hash, salt)).Returns(true);
        wrapper.Setup(w => w.CreateToken(It.IsAny<DatabaseUser>())).Returns("token");
        
        var service = new AuthService(repo.Object, wrapper.Object);

        var badLogin = new Login
        {
            Username = "myUsername",
            Password = "it's a secret"
        };
        //act
        var test = await service.Login(badLogin);
        //assert
        Assert.IsType<AuthToken>(test);
    }
}

// Moved outside of AuthServiceTests class
public class CatDbRepositoryTests
{
    private readonly ICatDbRepository _repo;
    private readonly ITestOutputHelper _output;

    public CatDbRepositoryTests()
    {
        _output = _output;
        // WARNING: Replace YOUR_PASSWORD with your actual MySQL password
        var myConnectionString = "Server=localhost;Database=fun_facts_db;Uid=root;Pwd=PoofBall#1;"; 
    
        var inMemorySettings = new Dictionary<string, string> {
            {"ConnectionStrings:DefaultConnection", myConnectionString}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        // Create the Repository manually
        _repo = new CatDbRepository(configuration);
    }

    [Fact]
    public async Task GetById_ShouldReturnCatFact()
    {
        // Act
        // Assuming ID 1 exists because of your filler data script
        var result = await _repo.GetById(1); 
        if (result != null)
        {
            _output.WriteLine($"Found fact: {result.FactText}");
            _output.WriteLine($"Created at: {result.CreatedAt}");
        }
        else
        {
            _output.WriteLine("Result was null.");
        }

        // Assert
        Assert.NotNull(result);
        Assert.True(result.FactText.Contains("Cats sleep"), "The fact text did not contain the expected phrase.");
    }

    [Fact]
    public async Task GetAll_ShouldReturnFacts()
    {
        // Act
        var result = await _repo.GetAllAsync();

        // Assert
        Assert.NotEmpty(result);
    }
}