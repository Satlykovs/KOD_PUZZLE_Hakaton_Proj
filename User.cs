using Microsoft.AspNetCore.Identity;

public class UserClass : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AvatarPath { get; set; }
    public string DateOfBirth { get; set; }
}