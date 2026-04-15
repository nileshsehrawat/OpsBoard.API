using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpsBoard.API.Data;
using OpsBoard.API.Models;
using OpsBoard.API.DTOs;
using Microsoft.AspNetCore.Authorization;
[ApiController]
[Route("api/resource")]
[Authorize]

public class ResourceController : ControllerBase
{
  private readonly AppDbContext _context;

  public ResourceController(AppDbContext context)
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
  public async Task<IActionResult> Create([FromBody] CreateResourceDto dto)
  {
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var orgExists = await _context.Organizations
        .AnyAsync(o => o.Id == dto.OrganizationId);

    if (!orgExists)
        return BadRequest("Invalid OrganizationId");

    var resource = new Resource
    {
        Name = dto.Name,
        Type = dto.Type,
        OrganizationId = dto.OrganizationId
    };

    _context.Resources.Add(resource);
    await _context.SaveChangesAsync();

    return Ok(new
    {
        message = "Resource created successfully",
        data = resource
    });
  }

  [HttpGet("by-type")]
  public async Task<IActionResult> GetByType(string type)
  {
    var data = await _context.Resources
        .Where(r => r.Type == type)
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

}
