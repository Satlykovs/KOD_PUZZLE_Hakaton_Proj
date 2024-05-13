

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
public interface ITokenService
{
    string CreateToken(string email, List<string> roleNames);
    ClaimsPrincipal ValidateToken(string token);
}
