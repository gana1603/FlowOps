using FlowOps.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlowOps.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Workflow> Workflows => Set<Workflow>();
    public DbSet<WorkflowStage> WorkflowStages => Set<WorkflowStage>();
    public DbSet<Request> Requests => Set<Request>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly
        );
        modelBuilder.Entity<Role>().HasData(
    new Role
    {
        Id = 1,
        Name = "Admin"
    },
    new Role
    {
        Id = 2,
        Name = "Manager"
    },
    new Role
    {
        Id = 3,
        Name = "Employee"
    }
);

        modelBuilder.Entity<Workflow>().HasData(
            new Workflow
            {
                Id = 1,
                Name = "Default Operations Workflow",
                Description = "Standard workflow for operational requests.",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<WorkflowStage>().HasData(
            new WorkflowStage
            {
                Id = 1,
                WorkflowId = 1,
                Name = "New",
                Position = 1,
                IsTerminal = false
            },
            new WorkflowStage
            {
                Id = 2,
                WorkflowId = 1,
                Name = "Review",
                Position = 2,
                IsTerminal = false
            },
            new WorkflowStage
            {
                Id = 3,
                WorkflowId = 1,
                Name = "In Progress",
                Position = 3,
                IsTerminal = false
            },
            new WorkflowStage
            {
                Id = 4,
                WorkflowId = 1,
                Name = "Testing",
                Position = 4,
                IsTerminal = false
            },
            new WorkflowStage
            {
                Id = 5,
                WorkflowId = 1,
                Name = "Completed",
                Position = 5,
                IsTerminal = true
            }
        );
    }
}