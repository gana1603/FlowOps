using System.ComponentModel.DataAnnotations;
using FlowOps.Api.Domain;

namespace FlowOps.Api.DTOs;

public class CreateRequestDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [EnumDataType(typeof(RequestPriority))]
    public RequestPriority Priority { get; set; }

    [Range(1, int.MaxValue)]
    public int WorkflowId { get; set; }

    [Range(1, int.MaxValue)]
    public int? AssignedToId { get; set; }

    public DateTime? DueDate { get; set; }
}