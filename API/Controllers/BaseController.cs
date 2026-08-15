using System.Security.Claims;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase {
    protected int CurrentUserId {
        get {
            // Cherche le Claim "NameIdentifier" injecté dans le token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Si claim est introuvable, l'utilisateur pas connecté ou token expiré
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Utilisateur non authentifié.");

            // Convertion de la chaîne en entier (int) pour que ce soit prêt à l'emploi
            return int.Parse(userIdClaim);
        }
    }
}