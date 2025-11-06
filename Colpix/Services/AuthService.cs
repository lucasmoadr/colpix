
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Colpix.Data.Models;
using Colpix.Data.Repositories;

namespace Colpix.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthService(AppDbContext db, ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        public async Task<TokenResult?> LoginAsync(string username, string password)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (verify == PasswordVerificationResult.Failed) return null;

            return _tokenService.CreateToken(user);
        }

        public async Task<(bool Created, string? Error, User? User)> RegisterAsync(string username, string email, string password)
        {
            if (await _db.Users.AnyAsync(u => u.Username == username))
                return (false, "Username already exists.", null);

            var user = new User
            {
                Username = username,
                LastUpdate = DateTime.UtcNow
            };

            user.PasswordHash = _hasher.HashPassword(user, password);

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return (true, null, user);
        }
    }
}