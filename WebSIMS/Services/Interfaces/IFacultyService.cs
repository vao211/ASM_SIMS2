using WebSIMS.Models.Entities;

namespace WebSIMS.Services.Interfaces;

public interface IFacultyService
{
    Task<IEnumerable<Faculty>> GetAllAsync();
    Task<Faculty?> GetByIdAsync(int id);
    Task AddAsync(Faculty faculty);
    Task UpdateAsync(Faculty faculty);
    Task DeleteAsync(int id);
}