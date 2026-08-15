namespace Domain.Models;

public class TierList {
    public int Id { get; set; }
    public string Nom { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
}