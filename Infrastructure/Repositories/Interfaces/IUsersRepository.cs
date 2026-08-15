using Domain.Models;

namespace Infrastructure.Repositories.Interfaces;

public interface IUsersRepository {
    Task<User?> GetById(int id);
    Task<ICollection<User>> GetAllAsync();
}