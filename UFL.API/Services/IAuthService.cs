using System.Threading.Tasks;
using UFL.API.Models;

namespace UFL.API.Services
{
    public interface IAuthService
    {
    Task<User>  Register(User user, string password);
    Task<User> Login(string username, string password);
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt);
    Task<bool> UserExists(string username);
    }
}
