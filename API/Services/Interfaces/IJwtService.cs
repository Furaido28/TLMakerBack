using Domain.Models;

namespace API.Services.Interfaces;

public interface IJwtService {
    public string GenerateToken(User user);
}