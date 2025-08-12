using WebSIMS.Models.Entities;

namespace WebSIMS.Services.Interfaces;

public interface IUserService
{
    public Task<Users?> LoginUserAsync(string username, string password);
}