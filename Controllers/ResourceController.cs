using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpsBoard.API.Data;
using OpsBoard.API.Models;
  
[ApiController]
[Route("api/[controller]")]
public class ResourceController : ControllerBase
{
  private readonly AppDbContext _context;

  public ResourceController (AppDbContext context)
  {
    _context = context;
  }

  [HttpGet]
  public async Task<IActionResult> Get() 
  {
    var data = await _context.Resources.ToListAsync();
    return Ok(data);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] Resource resource)
  {
    _context.Resources.Add(resource);
    await _context.SaveChangesAsync();
    return Ok(resource);
  }
}
