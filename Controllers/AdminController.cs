using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenSevice;
    public AdminController(RoleManager<ApplicationRole> roleManager,
     UserManager<ApplicationUser> userManager, IEmailService emailService,
     ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailService = emailService;
        _tokenSevice = tokenService;
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("giverole")]
    public async Task<IActionResult> GiveRole(string userName, string roleName)
    {
        string authHeader = Request.Headers["Authorization"];
        string token = authHeader.Substring("Bearer ".Length).Trim();
        var principals = _tokenSevice.ValidateToken(token);

        if (principals != null)
        {
            if (roleName != "Admin")
            {
        
                ApplicationUser user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    if (_roleManager.FindByNameAsync(roleName) != null)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        if (userRoles.Contains("Admin"))
                        {
                            return BadRequest("Попытка изменить роль администратора");
                        }
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                    await _userManager.AddToRoleAsync(user, roleName);
                    var newRole = await _userManager.GetRolesAsync(user);
                    var stringUserRoles = string.Join(", ", userRoles.ToArray());

                    await _emailService.SendEmailAsync(user.Email, 
                        "Изменена роль", $"Ваша роль изменена с {stringUserRoles} на {roleName}",
                        false);
                    
                    return Ok(newRole);
                    }    
                }
            }
        }
        return NotFound();
    }
       
    //ДЛЯ ТЕСТОВ
    // [Authorize(Roles = "Admin")]
    // [HttpGet("all")]
    // public IActionResult All()
    // {
    //     string authHeader = Request.Headers["Authorization"];
    //     string token = authHeader.Substring("Bearer ".Length).Trim();
    //     var principals = _tokenSevice.ValidateToken(token);

    //     if (principals != null)
    //     {
    //         var users = _userManager.Users.ToList();
    //         var responseData = users.Select(u => new 
    //         {
    //             FirstName = u.FirstName,
    //             LastName = u.LastName,
    //             UserRoles = _userManager.GetRolesAsync(u).Result
    //         });

    //         return Ok(responseData);
    //     }
    //     return NotFound("Не авторизован");
    // }

    [HttpGet("getuser")]
    public async Task<IActionResult> GetUser(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            return Ok(new
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = await _userManager.GetRolesAsync(user)
                
            });
        }
        return BadRequest("Пользователь не найден");
    }

}   
