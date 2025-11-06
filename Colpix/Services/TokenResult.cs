using Colpix.Data.Models;

namespace Colpix.Services
{
    public record TokenResult(string Token, DateTime Expires);

    public interface ITokenService
    {
        TokenResult CreateToken(User user);
    }
}