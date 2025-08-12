using WebSIMS.Repositories.Interfaces;
using WebSIMS.Models.Entities;
using WebSIMS.Services.Interfaces;


namespace WebSIMS.Services
{
    public class UserService :  IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository repository)
        {
            _userRepository = repository;
        }
        public async Task<Users?> LoginUserAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsername(username);
            if (user == null) return null;

            return user.PasswordHash.Equals(password) ? user : null;
        }
    }
}
