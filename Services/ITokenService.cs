namespace Centers.API.Services;
public interface ITokenService
{
    Task<string> CreateTokenAsync(UserEntity user);
}
