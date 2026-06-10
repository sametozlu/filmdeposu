using System.ComponentModel.DataAnnotations;

namespace FilmSerileri.ViewModels;

public class LoginViewModel
{
  [Required, EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required, DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;

  public string? ReturnUrl { get; set; }
}

public class ForgotPasswordViewModel
{
  [Required, EmailAddress]
  public string Email { get; set; } = string.Empty;
}

public class ResetPasswordViewModel
{
  [Required, EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string Token { get; set; } = string.Empty;

  [Required, MinLength(6), DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;

  [Required, Compare(nameof(Password)), DataType(DataType.Password)]
  public string ConfirmPassword { get; set; } = string.Empty;
}

public class RegisterViewModel
{
  [Required, MaxLength(60)]
  public string DisplayName { get; set; } = string.Empty;

  [Required, EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required, MinLength(6), DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;

  [Required, Compare(nameof(Password)), DataType(DataType.Password)]
  public string ConfirmPassword { get; set; } = string.Empty;
}
