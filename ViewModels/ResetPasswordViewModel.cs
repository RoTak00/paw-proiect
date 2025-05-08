using System.ComponentModel.DataAnnotations;

public class ResetPasswordViewModel
{
    [Required]
    public string Email { get; set; } = default!;

    [Required]
    public string Token { get; set; } = default!;

    [Required, DataType(DataType.Password)]
    public string NewPassword { get; set; } = default!;

    [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = default!;
}