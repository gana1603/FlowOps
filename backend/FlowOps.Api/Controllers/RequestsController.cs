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
        // [ApiController] has already rejected malformed input (missing title,
        // bad enum, etc.) with a 400 before we get here. Everything below
        // needs the database or the clock, so it can't live in the DTO.
        var now = DateTime.UtcNow;

        // 1. DueDate: normalize to UTC (Npgsql's timestamptz expects Kind=Utc),
        //    then reject past dates. Unspecified kind is treated as UTC.
        DateTime? dueDateUtc = null;
        if (dto.DueDate is { } dueDate)
        {
            dueDateUtc = dueDate.Kind switch
            {
                DateTimeKind.Utc => dueDate,
                DateTimeKind.Local => dueDate.ToUniversalTime(),
                _ => DateTime.SpecifyKind(dueDate, DateTimeKind.Utc)
            };

            if (dueDateUtc <= now)
            {
                ModelState.AddModelError(
                    nameof(dto.DueDate),
                    "Due date must be in the future.");
            }
        }

        // 2. Workflow must exist and be active; then find its initial stage
        //    (lowest Position). The server owns the starting state.
        var workflow = await _dbContext.Workflows
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == dto.WorkflowId);

        WorkflowStage? initialStage = null;

        if (workflow is null)
        {
            ModelState.AddModelError(
                nameof(dto.WorkflowId),
                $"Workflow {dto.WorkflowId} does not exist.");
        }
        else if (!workflow.IsActive)
        {
            ModelState.AddModelError(
                nameof(dto.WorkflowId),
                $"Workflow {dto.WorkflowId} is not active.");
        }
        else
        {
            initialStage = await _dbContext.WorkflowStages
                .AsNoTracking()
                .Where(s => s.WorkflowId == workflow.Id)
                .OrderBy(s => s.Position)
                .FirstOrDefaultAsync();

            if (initialStage is null)
            {
                ModelState.AddModelError(
                    nameof(dto.WorkflowId),
                    $"Workflow {dto.WorkflowId} has no stages, so a request cannot be created in it.");
            }
        }

        // 3. Optional assignee must exist and be active. Project only IsActive
        //    so we never load PasswordHash into memory.
        if (dto.AssignedToId is int assigneeId)
        {
            var assigneeIsActive = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == assigneeId)
                .Select(u => (bool?)u.IsActive)
                .FirstOrDefaultAsync();

            if (assigneeIsActive is null)
            {
                ModelState.AddModelError(
                    nameof(dto.AssignedToId),
                    $"User {assigneeId} does not exist.");
            }
            else if (assigneeIsActive == false)
            {
                ModelState.AddModelError(
                    nameof(dto.AssignedToId),
                    $"User {assigneeId} is not active.");
            }
        }

        // Report every problem found above in a single 400 response.
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        // If ModelState is valid, workflow and initialStage are non-null.
        var request = new Request
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Priority = dto.Priority,
            Status = RequestStatus.New,
            WorkflowId = workflow!.Id,
            WorkflowStageId = initialStage!.Id,
            AssignedToId = dto.AssignedToId,
            DueDate = dueDateUtc,
            CreatedAt = now,
            UpdatedAt = now
        };

        _dbContext.Requests.Add(request);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRequestById),
            new { id = request.Id },
            request);
    }
}