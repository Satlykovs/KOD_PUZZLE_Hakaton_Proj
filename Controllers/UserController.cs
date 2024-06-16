using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;
    private readonly EmailAnswerPatterns _emailAnswerPatterns = new EmailAnswerPatterns();
    private readonly IUserValidatorService _userValidatorService;

    public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
     RoleManager<ApplicationRole> roleManager, IEmailService emailService,
     ITokenService tokenService, IUserValidatorService userValidatorService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _emailService = emailService;
        _tokenService = tokenService;
        _userValidatorService = userValidatorService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] RegistrationDataForm dataForm)
    {
        if (!_userValidatorService.ValidatePassword(dataForm.Password) ||
        !_userValidatorService.ValidateEmail(dataForm.Email))
        {
            return BadRequest("Почта или пароль введены неверно");
        };
        
        var checkUser = await _userManager.FindByEmailAsync(dataForm.Email);
        if (checkUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = $"{Guid.NewGuid()}_default",
                FirstName = "default",
                LastName = "default",
                AvatarPath = "Content/Avatars/default.png",
                Email = dataForm.Email
            };

            var result = await _userManager.CreateAsync(user, dataForm.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Member");
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "User", new { userId = user.Id, token}, Request.Scheme);
                try
                {
                    await _emailService
                    .SendEmailAsync(user.Email, "Подтверждение почты. Скопируйте ссылку в браузер",
                     confirmationLink, false);
                }
                catch (Exception ex)
                {
                    _userManager.DeleteAsync(user);
                    return BadRequest("Что-то пошло не так. Повтороите попытку");
                }
                return Ok();
            }
            else
            {
                Console.WriteLine(result);
                return BadRequest();
            }
        }
        return BadRequest("Пользователь существует");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && !user.EmailConfirmed)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return Ok("Почта успешно подтверждена");
            }
        }
        return BadRequest();
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RegistrationDataForm dataForm)
    {
        
        ApplicationUser user = await _userManager.FindByEmailAsync(dataForm.Email);
        if (user != null)
        {
            
            var result = _signInManager.PasswordSignInAsync(user.UserName, dataForm.Password, false, false).Result;
            if (result.Succeeded)
            {
                
                IList<string> userRoles = await _userManager.GetRolesAsync(user);

                string subject = "Вход в аккаунт";
                Console.WriteLine(dataForm.Email);
                await _emailService.SendEmailAsync(dataForm.Email, subject,
                _emailAnswerPatterns.emailAnswerPatterns[subject] , false);

                return Ok(new {
                    token = _tokenService.CreateToken(dataForm.Email, userRoles.ToList()),
                    userRole = userRoles
                    });
            }
        }
        return BadRequest("Неправильная почта или пароль");
    }
    
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        _signInManager.SignOutAsync().Wait();
        return Ok(new {token = ""});
        
    }

    [Authorize]
    [HttpGet("userinfo")]
    public async Task<IActionResult> GetUserInfo()
    {
        string authHeader = Request.Headers["Authorization"];
        string token = authHeader.Substring("Bearer ".Length).Trim();
        var principal = _tokenService.ValidateToken(token);
        if (principal == null)
        {
            return BadRequest("Не авторизован");
        }
        string userEmail = principal.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        ApplicationUser user = await _userManager.FindByEmailAsync(userEmail);
        if (user != null)
        {
            return Ok(new {data = new {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                AvatarPath = user.AvatarPath
            },
            userRole = await _userManager.GetRolesAsync(user)});
        }
        return BadRequest();
    }

    [Authorize]
    [HttpPut("updateuserinfo")]
    public async Task<IActionResult> UpdateUserInfo([FromForm] UpdateUserDataForm userForm)
    {
        var user = await _userManager.FindByEmailAsync(userForm.Email);
        user.FirstName = userForm.FirstName;
        user.LastName = userForm.LastName;
        user.UserName = userForm.UserName;
        if (userForm.Avatar != null)
        {
            var oldAvatarPath = user.AvatarPath;
            if (oldAvatarPath != "Content/Avatars/default.png")
            System.IO.File.Delete(oldAvatarPath);
            string newAvatarPath = $"{Guid.NewGuid()}_{userForm.Avatar.FileName}";
            user.AvatarPath = $"Content/Avatars/{newAvatarPath}";
            using (var filestream = new FileStream(user.AvatarPath, FileMode.Create))
        {
            userForm.Avatar.CopyTo(filestream);
        }
        }

        await _userManager.UpdateAsync(user);
        
        string subject = "Изменены личные данные";
        await _emailService.SendEmailAsync(user.Email,subject, 
            _emailAnswerPatterns.emailAnswerPatterns[subject], false);

        return Ok();
    }






    [HttpGet("firstinitialize")]
    public async Task<IActionResult> FirstInitialize()
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
                    AvatarPath = "Content/Avatars/default.png",
                    UserName = "satlykovs@gmail.com", 
                    Email = "satlykovs@gmail.com"};
                var adminCreatingResult =_userManager.CreateAsync(adminUser, "Sanjar10102007!").Result;
                if (adminCreatingResult.Succeeded)
                {
                    var addingResult = _userManager.AddToRoleAsync(adminUser, "Admin").Result;
                    string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(adminUser);
                    await _userManager.ConfirmEmailAsync(adminUser, emailConfirmationToken);
                    if (addingResult.Succeeded)
                    {
                        var managerRole = new ApplicationRole{Name = "Manager"};
                        var memberRole = new ApplicationRole{Name = "Member"};
                        await _roleManager.CreateAsync(managerRole);
                        await _roleManager.CreateAsync(memberRole);

                        return Ok();
                    }
                }
            }
        }
        return BadRequest();
    }

}
