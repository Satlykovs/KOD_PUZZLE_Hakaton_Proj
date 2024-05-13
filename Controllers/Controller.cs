using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class Controller : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;


    public Controller(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
     RoleManager<ApplicationRole> roleManager, IEmailService emailService, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _emailService = emailService;
        _tokenService = tokenService;
    }


    [HttpPost("adduser")]
    public async Task<IActionResult> CreateUser([FromForm] UserForm userForm)
{
    var user = new ApplicationUser{
        UserName = userForm.UserName,
        FirstName = userForm.FirstName,
        LastName = userForm.LastName,
        DateOfBirth = userForm.DateOfBirth,
        AvatarPath = "default.png",
        Email = userForm.Email};
        if (userForm.avatar != null)
        {
            user.AvatarPath = "Avatars\\" + userForm.avatar.FileName;
            using (var filestream = new FileStream(user.AvatarPath, FileMode.Create))
            {
                userForm.avatar.CopyTo(filestream);
            }
        }

    var result = await _userManager.CreateAsync(user, userForm.Password);
    if (result.Succeeded)
    {
        _signInManager.SignInAsync(user, false).Wait();
        _userManager.AddToRoleAsync(user, "Member");

        await _emailService.SendEmail(userForm.Email, "Регистрация в системе KOD&PUZZLE",
        "Ваша почта была использована при создании аккаунта на платформе KOD&PUZZLE. Если это были не вы, обратитесь сюда:________________",
        false);

        return Ok(new {
            token = _tokenService.CreateToken(user.Email, ["Member"]),
            role = "Member"});
    }
    else
    {
        Console.WriteLine(result);
        return BadRequest();
    }
}


    [HttpGet("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        ApplicationUser user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var result = _signInManager.PasswordSignInAsync(user.UserName, password, false, false).Result;
            if (result.Succeeded)
            {
                
                IList<string> userRoles = await _userManager.GetRolesAsync(user);
                await _emailService.SendEmail(email, "Вход в аккаунт", "Был выполнен вход в ваш аккаунт на платформе KOD&PUZZLE. Если это были не вы, обратитесь сюда:____________", false);
                return Ok(new {
                    token = _tokenService.CreateToken(email, userRoles.ToList()),
                    role = userRoles
                    });
            }
        }
        return BadRequest("Неправильная почта или пароль");
    }
    
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        string authHeader = Request.Headers["Authorization"];
        string token = authHeader.Substring("Bearer ".Length).Trim();
        var principal = _tokenService.ValidateToken(token);
        _signInManager.SignOutAsync().Wait();
        return Ok();
        
    }

    [HttpGet("firstinitialize")]
    public IActionResult FirstInitialize()
    {
        if (!_roleManager.RoleExistsAsync("Admin").Result)
        {
            var adminRole = new ApplicationRole{Name = "Admin"};
            var roleCreatingResult = _roleManager.CreateAsync(adminRole).Result;
            if (roleCreatingResult.Succeeded)
            {
                var adminUser = new ApplicationUser{
                    FirstName = "Санджар",
                    LastName = "Сатлыков",
                    DateOfBirth = "10.10.2007",
                    AvatarPath = "default.png",
                    UserName = "satlykovs@gmail.com", 
                    Email = "satlykovs@gmail.com"};
                var adminCreatingResult = _userManager.CreateAsync(adminUser, "Sanjar10102007!").Result;
                if (adminCreatingResult.Succeeded)
                {
                    var addingResult = _userManager.AddToRoleAsync(adminUser, "Admin").Result;
                    if (addingResult.Succeeded)
                    {
                        return Ok();
                    }
                }
            }
        }
        return BadRequest();
    }

}
