using FlowOps.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowOps.Api.Data.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(r => r.Priority)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        builder.HasOne(r => r.Workflow)
            .WithMany()
            .HasForeignKey(r => r.WorkflowId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.WorkflowStage)
            .WithMany()
            .HasForeignKey(r => r.WorkflowStageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.AssignedTo)
            .WithMany()
            .HasForeignKey(r => r.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.Priority);
        builder.HasIndex(r => r.AssignedToId);
        builder.HasIndex(r => r.DueDate);
    }
}