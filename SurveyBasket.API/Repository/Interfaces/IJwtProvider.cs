namespace SurveyBasket.API.Repository.Interfaces;

public interface IJwtProvider
{
    (string token,int expiresIn) GenerateToken(ApplicationUser user,IEnumerable<string>permissions, IEnumerable<string> roles); 
    string? ValidateToken(string  token);
}
