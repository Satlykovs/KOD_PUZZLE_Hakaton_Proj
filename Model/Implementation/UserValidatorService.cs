using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

public class UserValidatorService : IUserValidatorService
{
    public bool ValidatePassword(string password)
    {
        if (password == null)
            return false;

        if (password.Length < 6)
            return false;

        if (!Regex.IsMatch(password, @"\d"))
            return false;

        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?\"":{}|<>]"))
            return false;

        if (!Regex.IsMatch(password, @"[A-Z]"))
            return false;

        return true;
    }

    public bool ValidateEmail(string email)
    {
        if (email == null)
        return false;

        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }
}