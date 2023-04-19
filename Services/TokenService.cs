namespace Centers.API.Services;
public sealed class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<UserEntity> _userManager;

    public TokenService(IConfiguration config, UserManager<UserEntity> userManager)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config[Constants.TokenKey]!));
        _userManager = userManager ??
            throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<string> CreateTokenAsync(UserEntity user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds,
            NotBefore = DateTime.UtcNow,
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
