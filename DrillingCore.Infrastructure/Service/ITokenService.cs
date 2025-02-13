namespace DrillingCore.Infrastructure.Services
{
    public interface ITokenService
    {
        string GenerateToken(DrillingCore.Core.Entities.User user);
    }
}
