using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

public class AdminController : ControllerBase
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    public AdminController(RoleManager<ApplicationRole> roleManager,
     UserManager<ApplicationUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailService = emailService;
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("addrole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (roleExists)
        {
            return BadRequest("Роль существует");

        }

        var role = new ApplicationRole{Name = roleName};
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            return Ok(_roleManager.Roles.ToList());

        }
        else
        {   
            return BadRequest();

        }
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("giverole")]
    public async Task<IActionResult> GiveRole(string userName, string roleName)
    {
        ApplicationUser user = await _userManager.FindByNameAsync(userName);
        if (user != null)
        {
            if (_roleManager.FindByNameAsync(roleName) != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains("Admin"))
                {
                    userRoles.Remove("Admin");
                }
                await _userManager.RemoveFromRolesAsync(user, userRoles);
                await _userManager.AddToRoleAsync(user, roleName);
                var newRole = await _userManager.GetRolesAsync(user);
                var stringUserRoles = string.Join(", ", userRoles.ToArray());
                await _emailService.SendEmail(user.Email, "Изменена роль", $"Ваша роль изменена с {stringUserRoles} на {roleName}", false);
                return Ok(newRole);
            }    
        }
        return NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public IActionResult All()
    {
        if (User.Identity.IsAuthenticated)
        {
            foreach (var role in _roleManager.Roles)
            {
                Console.WriteLine(role);
            }
            var users = _userManager.Users.ToList();
            var responseData = users.Select(u => new 
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserRoles = _userManager.GetRolesAsync(u).Result
            });

            return Ok(responseData);
        }
        return NotFound("Не авторизован");
    }   


}   
