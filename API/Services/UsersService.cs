using API.Services.Interfaces;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;

namespace API.Services;

public class UsersService : IUsersService {
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository) {
        _usersRepository = usersRepository;
    }

    public async Task<User?> GetById(int id) {
        return await _usersRepository.GetById(id);
    }

    public async Task<ICollection<User>> GetAllAsync() {
        return await _usersRepository.GetAllAsync();
    }
}