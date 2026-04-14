namespace OpsBoard.API.Models;

public class Organization
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<Resource> Resources { get; set; } = new();
}