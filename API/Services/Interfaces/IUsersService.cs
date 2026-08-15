using Domain.Models;

namespace API.Services.Interfaces;

public interface IUsersService {
    Task<User?> GetById(int id);
    Task<ICollection<User>> GetAllAsync();
}