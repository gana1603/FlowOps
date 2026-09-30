using FlowOps.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowOps.Api.Data.Configurations;

public class WorkflowStageConfiguration : IEntityTypeConfiguration<WorkflowStage>
{
    public void Configure(EntityTypeBuilder<WorkflowStage> builder)
    {
        builder.HasKey(stage => stage.Id);

        builder.Property(stage => stage.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(stage => new
        {
            stage.WorkflowId,
            stage.Position
        })
        .IsUnique();

        builder.HasIndex(stage => new
        {
            stage.WorkflowId,
            stage.Name
        })
        .IsUnique();

        builder.HasOne(stage => stage.Workflow)
            .WithMany(workflow => workflow.Stages)
            .HasForeignKey(stage => stage.WorkflowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}