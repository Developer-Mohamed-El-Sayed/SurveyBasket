namespace SurveyBasket.API.Repository.Implementation;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private const int _expirationIn = 60;
    private readonly JwtOptions _options = options.Value;

    public (string token, int expiresIn) GenerateToken(ApplicationUser user)
    {
        Claim[] claims = [
            new(JwtRegisteredClaimNames.Email,user.Email!),
            new(JwtRegisteredClaimNames.GivenName,user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName,user.LastName),
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub,user.Id)
        ];
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer:_options.Issuer,
            audience:_options.Audience,
            claims:claims,
            signingCredentials:signingCredentials,
            expires: DateTime.UtcNow.AddMinutes(_expirationIn)
        );
        return(token: new JwtSecurityTokenHandler().WriteToken(token),_expirationIn);
    }
}
