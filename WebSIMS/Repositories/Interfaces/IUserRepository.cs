using WebSIMS.Models.Entities;

namespace WebSIMS.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<Users?> GetUserByUsername(string username);
        Task<Users?> GetUserById(int id);
        Task<List<Users>> GetAllAsync();
        Task<List<Users>> GetUsersByRoleAsync(string role);
        Task AddAsync(Users user);
        void Update(Users user);
        void Delete(Users user);
        Task SaveChangeAsync();
        
    }
}
