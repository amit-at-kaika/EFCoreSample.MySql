using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.MySql.Models.DTOs;
public class UserLoginRequest
{
    [Required]
    public string Mail { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}