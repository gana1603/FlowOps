using FlowOps.Api.DTOs;
using FlowOps.Api.Domain;
using FlowOps.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public RequestsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetRequests()
    {
        var requests = await _dbContext.Requests
            .AsNoTracking()
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet("{id:int}")]
public async Task<IActionResult> GetRequestById(int id)
{
    var request = await _dbContext.Requests
        .Include(r => r.Workflow)
        .Include(r => r.WorkflowStage)
        .AsNoTracking()
        .FirstOrDefaultAsync(r => r.Id == id);

    if (request is null)
    {
        return NotFound();
    }

    return Ok(request);
}

    [HttpPost]
public async Task<IActionResult> CreateRequest(CreateRequestDto dto)
{
    var request = new Request
    {
        Title = dto.Title,
        Description = dto.Description,
        Priority = dto.Priority,
        Status = RequestStatus.New,
        WorkflowId = dto.WorkflowId,
        WorkflowStageId = dto.WorkflowStageId,
        AssignedToId = dto.AssignedToId,
        DueDate = dto.DueDate,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    _dbContext.Requests.Add(request);
    await _dbContext.SaveChangesAsync();

    return CreatedAtAction(
        nameof(GetRequests),
        new { id = request.Id },
        request);
}
}