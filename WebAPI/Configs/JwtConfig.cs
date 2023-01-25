
namespace EFCoreSample.MySql.Config;

public class JwtConfig
{
    public const string SectionName = "JwtConfig";
    public string Secret { get; set; } = string.Empty;
    public TimeSpan ExpiryTime { get; set; } 
}