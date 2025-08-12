using Microsoft.EntityFrameworkCore;
using WebSIMS.Data;
using WebSIMS.Models.Entities;
using WebSIMS.Repositories.Interfaces;

namespace WebSIMS.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SIMSdbContext _dbContext;

        public UserRepository(SIMSdbContext context)
        {
            _dbContext = context;
        }

        public async Task AddAsync(Users user)
        {
            await _dbContext.UsersDb.AddAsync(user);
        }

        public void Update(Users user)
        {
            _dbContext.UsersDb.Update(user);
        }

        public void Delete(Users user)
        {
            _dbContext.UsersDb.Remove(user);
        }

        public async Task<Users?> GetUserById(int id)
        {
            return await _dbContext.UsersDb.FindAsync(id);
        }

        public async Task<Users?> GetUserByUsername(string username)
        {
            Console.WriteLine($"Querying user with username: {username}");
            var user = await _dbContext.UsersDb.FirstOrDefaultAsync(u => u.Username == username);
            Console.WriteLine(user != null ? $"Found user: {user.Username}" : "User not found.");
            return user;
        }

        public async Task<List<Users>> GetAllAsync()
        {
            return await _dbContext.UsersDb.ToListAsync();
        }

        public async Task SaveChangeAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public Task<List<Users>> GetUsersByRoleAsync(string role)
        {
            throw new NotImplementedException();
        }
    }
}