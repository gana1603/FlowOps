namespace FlowOps.Api.Domain;

public class Request
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public RequestPriority Priority { get; set; }

    public RequestStatus Status { get; set; }

    public int WorkflowId { get; set; }

    public Workflow Workflow { get; set; } = null!;

    public int WorkflowStageId { get; set; }

    public WorkflowStage WorkflowStage { get; set; } = null!;

    public int? AssignedToId { get; set; }

    public User? AssignedTo { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}