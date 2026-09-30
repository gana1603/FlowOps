namespace FlowOps.Api.Domain;

public class Role
{
    public int id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = new List<User>();
}