using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.MySql.Models.DTOs;

public class UserRegistrationRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Mail { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}