using FlowOps.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowOps.Api.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(role => role.Name)
            .IsUnique();
    }
}