

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

public class UserHub : Hub
{
    private static readonly Dictionary<int, string> _userConnections = new Dictionary<int, string>();
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserHub(ITokenService tokenService, UserManager<ApplicationUser> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected : {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        RemoveUserConnection(Context.ConnectionId);
        Console.WriteLine($"Client disconnected : {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SetUserId(string token)
    {
        var principal = _tokenService.ValidateToken(token);
        var emailClaim = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(emailClaim.Value);

        _userConnections[user.Id] = Context.ConnectionId;

    }

    public async Task SendToken(string oldToken)
    {
        var principal = _tokenService.ValidateToken(oldToken);
        var emailClaim = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(emailClaim.Value);

        if (_userConnections.TryGetValue(user.Id, out string connectionId))
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);

            var newToken = _tokenService.CreateToken(emailClaim.Value, roles.ToList());

            await Clients.Client(connectionId).SendAsync("ReceiveToken", newToken);
        }
    }




    public void RemoveUserConnection(string connectionId)
    {
        int? userId = _userConnections.FirstOrDefault(x => x.Value == connectionId).Key;
        if (userId != null)
        {
            _userConnections.Remove(Convert.ToInt32(userId));
        }
    }
}