using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpsBoard.API.Data;
using OpsBoard.API.Models;
using OpsBoard.API.DTOs;

[ApiController]
[Route("api/organization")]
public class OrganizationController : ControllerBase 
{
  private readonly AppDbContext _context;

  public OrganizationController(AppDbContext context)
  {
    _context = context;
  }

  [HttpGet]
  public async Task<IActionResult> Get() 
  {
    var data = await _context.Resources
  .Include(r => r.Organization)
  .Select(r => new ResourceDto
    {
      Id = r.Id,
      Name = r.Name,
      Type = r.Type,
      OrganizationName = r.Organization.Name
    })
  .ToListAsync();
    return Ok(data);
  }

  [HttpPost]
public async Task<IActionResult> Create([FromBody] CreateOrganizationDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var organization = new Organization
    {
        Name = dto.Name
    };

    _context.Organizations.Add(organization);
    await _context.SaveChangesAsync();

    return Ok(new
    {
        message = "Organization created successfully",
        data = organization
    });
}
}