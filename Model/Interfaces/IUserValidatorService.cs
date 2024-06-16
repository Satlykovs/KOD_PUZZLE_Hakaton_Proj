public interface IUserValidatorService
{
    public bool ValidatePassword(string password);
    public bool ValidateEmail(string email);
}