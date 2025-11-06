
using System.Threading.Tasks;
using Colpix.Data.Models;

namespace Colpix.Services
{
    public interface IAuthService
    {
        Task<TokenResult?> LoginAsync(string username, string password);
        Task<(bool Created, string? Error, User? User)> RegisterAsync(string username, string email, string password);
    }
}