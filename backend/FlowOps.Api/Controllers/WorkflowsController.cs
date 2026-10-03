using System.Runtime.Versioning;
using FlowOps.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public WorkflowsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkflows()
    {
        var workflows = await _dbContext.Workflows
        .AsNoTracking()
        .ToListAsync();

        return Ok(workflows);
    }
}