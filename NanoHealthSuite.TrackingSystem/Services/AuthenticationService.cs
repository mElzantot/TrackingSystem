using NanoHealthSuite.Data.Enums;
using NanoHealthSuite.Data.Models;
using NanoHealthSuite.TrackingSystem.Shared;
using NanoHealthSuite.TrackingSystem.Processors;

namespace NanoHealthSuite.TrackingSystem.Services;

public class AuthenticationService
{
    private readonly IHashingService _hashingService;
    private readonly ITokenServiceProvider _tokenServiceProvider;
    private readonly IUserRepository _userRepository;

    public AuthenticationService(
        IHashingService hashingService,
        ITokenServiceProvider tokenServiceProvider,
        IUserRepository userRepository)
    {
        _hashingService = hashingService;
        _tokenServiceProvider = tokenServiceProvider;
        _userRepository = userRepository;
    }

    public async Task<Result<AuthResponseDto>> Register(AuthRequest newUser)
    {
        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = newUser.UserName,
                Email = newUser.Email,
                PasswordHash = _hashingService.Hash(newUser.Password),
                RoleId = newUser.RoleId
            };
            await _userRepository.AddAsync(user);
            return Result.Ok(_tokenServiceProvider.GenerateAccessToken(user));
        }
        catch (Exception e)
        {
            return Result.Fail<AuthResponseDto>(e.Message);
        }
    }

    public async Task<AuthResponseDto?> Login(LoginRequest request)
    {
        var user = await _userRepository.GetSingleAsync(
            u => u, 
            u => u.Email == request.Email,
            includes: u => u.Role);
        
        if (user == null) return null;
        var isAuthorized = _hashingService.HashCheck(user.PasswordHash, request.Password);
        return isAuthorized ? _tokenServiceProvider.GenerateAccessToken(user) : null;
    }

    public async Task<bool> CheckIfUserNameExist(string userName)
    {
        return await _userRepository.GetFirstAsync(x => x, x => x.Name == userName) != null;
    }
}