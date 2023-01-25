namespace EFCoreSample.MySql.Models;

public class AuthResults
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public bool Result { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}