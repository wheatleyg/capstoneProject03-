using CapstoneBackend.Auth;
using CapstoneBackend.Auth.Models;
using CapstoneBackend.Utilities.Exceptions;
using Moq;

using CapstoneBackend.Core.Repositories;
using CapstoneBackend.Core.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

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
    public class CatDbRepositoryTests
    {
        private readonly ICatDbRepository _repo;

        public CatDbRepositoryTests()
        {
            // 1. Build a configuration that reads your actual appsettings.json (or use hardcoded for quick test)
            // WARNING: For a quick "does it work" test, you can temporarily hardcode the string to avoid file path headaches.
            // Or, point it to your Core project's appsettings.
        
            var myConnectionString = "Server=localhost;Database=fun_facts_db;Uid=root;Pwd=YOUR_PASSWORD;"; 
        
            var inMemorySettings = new Dictionary<string, string> {
                {"ConnectionStrings:DefaultConnection", myConnectionString}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // 2. Create the Repository manually
            _repo = new CatDbRepository(configuration);
        }

        [Fact]
        public async Task GetById_ShouldReturnCatFact()
        {
            // Act
            // Assuming ID 1 exists because of your filler data script
            var result = await _repo.GetByIdAsync(1); 

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Cats sleep", result.FactText); // Check for part of the text you inserted
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
}