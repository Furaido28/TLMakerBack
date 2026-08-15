namespace Domain.Models;

public class User {
    public int Id { get; set; }
    
    public string Nom { get; set; }
    
    public string Prenom { get; set; }
    
    public ICollection<TierList> TierLists { get; set; } = new List<TierList>();
}