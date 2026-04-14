using System.ComponentModel.DataAnnotations;

namespace OpsBoard.API.DTOs;

public class CreateResourceDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Type { get; set; }

    [Range(1, int.MaxValue)]
    public int OrganizationId { get; set; }
}