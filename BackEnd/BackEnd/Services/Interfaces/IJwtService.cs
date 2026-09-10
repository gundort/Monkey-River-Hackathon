using BackEnd.Models;

namespace BackEnd.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
