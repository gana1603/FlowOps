namespace FlowOps.Api.Domain;

public class WorkflowStage
{
    public int Id { get; set; }

    public int WorkflowId { get; set; }

    public Workflow Workflow { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public int Position { get; set; }

    public bool IsTerminal { get; set; }
}