using Microsoft.EntityFrameworkCore;
using WebSIMS.Data;
using WebSIMS.Models.Entities;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Services;

public class FacultyService :  IFacultyService
{
    private readonly SIMSdbContext _context;

    public FacultyService(SIMSdbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Faculty>> GetAllAsync()
    {
        return await _context.FacultyDb.Include(f => f.User).ToListAsync();
    }

    public async Task<Faculty?> GetByIdAsync(int id)
    {
        return await _context.FacultyDb.Include(f => f.User).FirstOrDefaultAsync(f => f.FacultyID == id);
    }

    public async Task AddAsync(Faculty faculty)
    {
        _context.FacultyDb.Add(faculty);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Faculty faculty)
    {
        _context.FacultyDb.Update(faculty);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var faculty = await _context.FacultyDb.FindAsync(id);
        if (faculty != null)
        {
            _context.FacultyDb.Remove(faculty);
            await _context.SaveChangesAsync();
        }
    }
}