using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.MySql.Models.DTOs;
public class TokenRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;
    [Required]
    public string RequestToken { get; set; } = string.Empty;   
}