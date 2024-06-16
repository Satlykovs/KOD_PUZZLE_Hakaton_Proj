using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/lessons")]
public class LessonController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILessonService _lessonService;
    private readonly ITokenService _tokenService;

    public LessonController(UserManager<ApplicationUser> userManager,
    ILessonService lessonService, ITokenService tokenService)
    {
        _userManager = userManager;
        _lessonService = lessonService;
        _tokenService = tokenService;
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromForm] LessonDataForm dataForm)
    {
        string authHeader = Request.Headers["Authorization"];
        string token = authHeader.Substring("Bearer ".Length).Trim();
        var principal = _tokenService.ValidateToken(token);
        if (principal == null)
        {
            return BadRequest("Не авторизован");
        }
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        if (_userManager.FindByEmailAsync(email) != null)
        {
            await _lessonService.Add(dataForm);
            return Ok();
        }
        return BadRequest("Не авторизован");
    }

    [Authorize(Roles = "Manager")]
    [HttpDelete("delete")]
    public IActionResult Delete(int lessonId)
    {
        string authHeader = Request.Headers["Authorization"];
        string token = authHeader.Substring("Bearer ".Length).Trim();
        var principal = _tokenService.ValidateToken(token);
        if (principal == null)
        {
            return BadRequest("Не авторизован");
        }
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        if (_userManager.FindByEmailAsync(email) != null)
        {
            _lessonService.Delete(lessonId);
            return Ok();
        }
        return BadRequest("Не авторизован");
    }


    [Authorize(Roles = "Manager")]
    [HttpPut("edit")]
    public async Task<IActionResult> Edit([FromForm] LessonDataForm lessonDataForm)
    {
        string authHeader = Request.Headers["Authorization"];
        string token = authHeader.Substring("Bearer ".Length).Trim();
        var principal = _tokenService.ValidateToken(token);
        if (principal == null)
        {
            return BadRequest("Не авторизован");
        }
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        if (_userManager.FindByEmailAsync(email) != null)
        {
            await _lessonService.Edit(lessonDataForm);
            return Ok();
        }
        return BadRequest("Не авторизован");
    }

    [Authorize]
    [HttpGet("mark/{lessonId}/{mark}")]
    public async Task<IActionResult> Authorize(int lessonId, int mark)
    {
        if (mark > 10 || mark < 0)
        {
            return NotFound();
        }
        string authHeader = Request.Headers["Authorization"];
        string token = authHeader.Substring("Bearer ".Length).Trim();
        var principal = _tokenService.ValidateToken(token);
        if (principal == null)
        {
            return BadRequest("Не авторизован");
        }
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        var user = _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            await _lessonService.Mark(user.Id, lessonId, mark);
            return Ok();
        }
        return BadRequest();
    }

    [Authorize]
    [HttpGet("addfavourite")]
    public async Task<IActionResult> AddFavourite(int lessonId)
    {
        string authHeader = Request.Headers["Authorization"];
        string token = authHeader.Substring("Bearer ".Length).Trim();
        var principal = _tokenService.ValidateToken(token);
        if (principal == null)
        {
            return BadRequest("Не авторизован");
        }
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        var user = _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            await _lessonService.AddFavourite(user.Id, lessonId);
            return Ok();
        }
        return BadRequest();
    }
    [Authorize]
    [HttpDelete("deletefavourite")]
    public async Task<IActionResult> DeleteFavourite(int lessonId)
    {
        string authHeader = Request.Headers["Authorization"];
        string token = authHeader.Substring("Bearer ".Length).Trim();
        var principal = _tokenService.ValidateToken(token);
        if (principal == null)
        {
            return BadRequest("Не авторизован");
        }
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        var user = _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            await _lessonService.AddFavourite(user.Id, lessonId);
            return Ok();
        }
        return BadRequest();
    }

    [HttpPost("getlessonsnonauthuser")]
    public async Task<IActionResult> GetLessonsNonAuthUser(List<string> tags)
    {
        return Ok(await _lessonService.GetLessonsNonAuthUser(tags));
    }
    [HttpPost("getlessonsauthuser")]
    public async Task<IActionResult> GetLessonsAuthUser(List<string> tags)
    {
        string authHeader = Request.Headers["Authorization"];
        string token = authHeader.Substring("Bearer ".Length).Trim();
        var principal = _tokenService.ValidateToken(token);
        if (principal == null)
        {
            return BadRequest("Не авторизован");
        }
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        var user = _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            return Ok(await _lessonService.GetLessonsAuthUser(user.Id, tags));
        }
        return BadRequest("Не авторизован");
    }
    
    [HttpGet("getlesson/{lessonId}")]
    public async Task<IActionResult> GetLesson(int lessonId)
    {
        var lesson = await _lessonService.GetLesson(lessonId);
        if (lesson != null)
        {
            return Ok(lesson);
        }
        return NotFound();
    }

}