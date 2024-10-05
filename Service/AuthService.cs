using Infrastructure.Model;
using Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.AuthenticateAsync(username, password);
            if (user != null)
            {
                await _userRepository.UpdateLastLoginAsync(user.Id);
            }
            return user;
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            // In a real-world scenario, use a proper password hashing library
            user.PasswordHash = password; // Replace with proper password hashing
            user.CreatedAt = DateTime.UtcNow;
            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _userRepository.UsernameExistsAsync(username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }
    }
}
