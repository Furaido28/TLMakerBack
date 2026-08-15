using Domain.Models;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UsersRepository : IUsersRepository {
    private readonly AppDbContext _context;
    
    public UsersRepository(AppDbContext context) {
        _context = context;
    }
    
    public async Task<User?> GetById(int id) {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<ICollection<User>> GetAllAsync() {
        return await _context.Users
            .OrderBy(u => u.Nom)
            .ThenBy(u => u.Prenom)
            .ToListAsync();
    }
}