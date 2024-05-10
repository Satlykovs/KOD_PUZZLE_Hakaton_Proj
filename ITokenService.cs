using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
public interface ITokenService
{
    string CreateToken(string userName, bool isLoggedIn);
    ClaimsPrincipal ValidateToken(string token);
}
