using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.MySql.Models.DTOs;

public class UserRegistrationRequest
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Mail { get; set; }
    [Required]
    public string? Password { get; set; }
}