using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SecurityKey _securityKey;

        public TokenService(string issuer, string audience, SecurityKey securityKey)
    {
        _issuer = issuer;
        _audience = audience;
        _securityKey = securityKey;
    }

    public string CreateToken(string userName, bool isLoggedIn)
    {
        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, userName),
            new Claim("isLoggedIn", isLoggedIn.ToString())};
        
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(jwt);    
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = _securityKey
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch (Exception)
        {
            return null;
        }
    }
}