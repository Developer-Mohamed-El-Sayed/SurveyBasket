namespace SurveyBasket.API.Entities;

public  class JwtOptions
{
    public const string SectionName = "Jwt";
    [Required]
    public string Key { get; set; } = string.Empty;
    [Required]
    public string Issuer { get; set; } = string.Empty;
    [Required]
    public string Audience { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int ExpiresIn { get; set; }
}
