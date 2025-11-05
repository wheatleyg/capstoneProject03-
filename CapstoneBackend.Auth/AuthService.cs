using System.Globalization;
using System.Text.RegularExpressions;
using CapstoneBackend.Auth.Models;
using CapstoneBackend.Utilities.Exceptions;

namespace CapstoneBackend.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IAuthServiceWrapper _authServiceWrapper;

    private const string GenericEmailMessage =
        "There was an error processing the provided email address. Please examine the provided value and try again.";
    
    
    public AuthService(IAuthRepository authRepository, IAuthServiceWrapper authServiceWrapper)
    {
        _authRepository = authRepository;
        _authServiceWrapper = authServiceWrapper;
    }

    public Task<ApiUser> Register(ApiUser user)
    {
        ValidateUsername(user.Username);
        ValidateEmail(user.EmailAddress);
        ValidatePassword(user.Password);
        ValidateNoCollisions(user);
        
        //set default values to prevent user overrides
        user.Id = null;
        user.CreateDatetime = DateTime.UtcNow;
        
        return _authRepository.Register(user);
    }

    public async Task<AuthToken> Login(Login credentials)
    {
        ValidateUsername(credentials.Username);
        ValidatePassword(credentials.Password);
        
        var user = await _authRepository.GetUserByUsername(credentials.Username);
        if (user == null)
            throw new UnauthenticatedException($"No user found for ${credentials.Username}");
        if (user.IsDeleted)
            throw new UnauthenticatedException($"A login attempt was made for user {user.Id}, but user is inactive.");
        if (!_authServiceWrapper.VerifyPasswordHash(credentials.Password, user.PasswordHash, user.PasswordSalt))
            throw new UnauthenticatedException($"A login attempt was made for user {user.Id}, but password was incorrect.");
        var token = _authServiceWrapper.CreateToken(user);

        return new AuthToken
        {
            Id = user.Id,
            Token = token
        };
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new BadRequestException("User provided an empty username")
            {
                ClientMessage = "Please provide a valid username and try again."
            };
    }

    private static void ValidateEmail(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new BadRequestException("User provided an empty email address")
            {
                ClientMessage = "Please provide a valid email address and try again."
            };
        try
        {
            //normalize the domain
            static string DomainMapper(Match match)
            {
                //use this to convert unicode domain names
                var internetDomainName = new IdnMapping();
                //extract and process domain name or throw ArgumentException if invalid
                var domainName = internetDomainName.GetAscii(match.Groups[2].Value);
                return match.Groups[1].Value + domainName;
            }

            emailAddress = Regex.Replace(emailAddress, @"(@)(.+)$", DomainMapper, RegexOptions.None,
                TimeSpan.FromMilliseconds(200));

            _ = Regex.IsMatch(emailAddress,
                @"^(?("")("".+?(?<!\\)""@)" + //if the username starts with a quote, it should end with a quote
                @"|(([0-9a-z]" + //quotes aside, look for alphanumeric characters
                @"((\.(?!\.))" + //periods are ok, but not two consecutive periods
                @"|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)" + //these characters are also fine
                @"(?<=[0-9a-z])@))" + //any additional alphanumerics immediately before the '@'
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])" + //if there are brackets in the domain name, the value between them should be an ip address
                @"|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+" + //in the domain name, look for alphanumerics and hyphens followed by a period 1 or more times
                @"[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", //allow up to 22 characters for the top-level domain
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException ex)
        {
            throw new BadRequestException("Email regex timed out", ex)
            {
                ClientMessage = GenericEmailMessage
            };
        }
        catch (ArgumentException ex)
        {
            throw new BadRequestException("Invalid email address", ex)
            {
                ClientMessage = "The provided email address was invalid. Please provide a valid value and try again."
            };
        }
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new BadRequestException("User provided an empty password")
            {
                ClientMessage = "Please provide a password and try again."
            };
        //TODO add more password validation?
    }

    private void ValidateNoCollisions(ApiUser user)
    {
        var emailUser = _authRepository.GetUserByEmail(user.EmailAddress);
        // just give a general error. do not reveal that the email is already in the db
        if (emailUser != null) throw new BadRequestException("Email address already exists")
        {
            ClientMessage = GenericEmailMessage
        };
        
        var usernameUser = _authRepository.GetUserByUsername(user.Username);
        // see comment above
        if (usernameUser != null) throw new BadRequestException("Username address already exists")
        {
            ClientMessage = "Username is not available." //consider whether to use a more generic message
        };
    }
}