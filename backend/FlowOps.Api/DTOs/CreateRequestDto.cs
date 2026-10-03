using FlowOps.Api.Domain;

namespace FlowOps.Api.DTOs;

public class CreateRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public RequestPriority Priority { get; set; }

    public int WorkflowId { get; set; }

    public int WorkflowStageId { get; set; }

    public int? AssignedToId { get; set; }

    public DateTime? DueDate { get; set; }
}