namespace Application.Interfaces;

public interface ITokenHelper
{
    string GenerateToken(Guid userId);
    bool ValidateToken(string token, out Guid userId);
}
