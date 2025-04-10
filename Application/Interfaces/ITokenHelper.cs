namespace Application.Interfaces;

public interface ITokenHelper
{
    string GenerateToken(Guid userId);
}
