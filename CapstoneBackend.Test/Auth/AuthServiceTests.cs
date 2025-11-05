using CapstoneBackend.Auth;
using CapstoneBackend.Auth.Models;
using CapstoneBackend.Utilities.Exceptions;
using Moq;

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