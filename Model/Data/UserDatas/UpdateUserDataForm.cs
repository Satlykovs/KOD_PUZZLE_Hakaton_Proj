

using System.ComponentModel.DataAnnotations;

public class UpdateUserDataForm
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set;}
    [Required]
    public string Email { get; set; }
    [Required]
    public string UserName { get; set; }
    public IFormFile? Avatar { get; set; }
}