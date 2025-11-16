namespace Core.Entities;

public class University
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<User> Users { get; set; }
}
