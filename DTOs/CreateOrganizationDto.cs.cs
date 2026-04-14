using System.ComponentModel.DataAnnotations;

namespace OpsBoard.API.DTOs;

public class CreateOrganizationDto
{
    [Required]
    public string Name { get; set; }
}