using FlowOps.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowOps.Api.Data.Configurations;

public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.HasKey(workflow => workflow.Id);

        builder.Property(workflow => workflow.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(workflow => workflow.Description)
            .HasMaxLength(500);

        builder.HasMany(workflow => workflow.Stages)
            .WithOne(stage => stage.Workflow)
            .HasForeignKey(stage => stage.WorkflowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}