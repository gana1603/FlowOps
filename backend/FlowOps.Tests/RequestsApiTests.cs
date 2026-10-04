using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using FlowOps.Tests.Infrastructure;

namespace FlowOps.Tests;

public class RequestsApiTests : IClassFixture<FlowOpsWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RequestsApiTests(FlowOpsWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Fact]
    public async Task CreateRequest_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new
        {
            title = "Test laptop access request",
            description = "Integration test request for laptop access.",
            priority = "High",
            workflowId = 1,
            dueDate = "2026-12-31T00:00:00Z"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/requests",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdRequest = await response.Content.ReadFromJsonAsync<CreatedRequestResponse>();

        Assert.NotNull(createdRequest);
        Assert.True(createdRequest.Id > 0);
        Assert.Equal("Test laptop access request", createdRequest.Title);
        Assert.Equal("High", createdRequest.Priority);
        Assert.Equal("New", createdRequest.Status);
        Assert.Equal(1, createdRequest.WorkflowId);
        Assert.Equal(1, createdRequest.WorkflowStageId);
    }

    private sealed class CreatedRequestResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int WorkflowId { get; set; }
        public int WorkflowStageId { get; set; }
    }

    [Fact]
    public async Task CreateRequest_WithBlankTitle_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            title = "   ",
            description = "Integration test request with an invalid title.",
            priority = "High",
            workflowId = 1,
            dueDate = "2026-12-31T00:00:00Z"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/requests",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Contains("Title", responseBody);
    }

    [Fact]
    public async Task CreateRequest_WithNonExistentWorkflow_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            title = "Test request with invalid workflow",
            description = "Integration test request with a non-existent workflow.",
            priority = "High",
            workflowId = 999,
            dueDate = "2026-12-31T00:00:00Z"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/requests",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Contains("Workflow 999 does not exist", responseBody);
    }

    [Fact]
    public async Task CreateRequest_WithPastDueDate_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            title = "Test request with past due date",
            description = "Integration test request with an invalid due date.",
            priority = "High",
            workflowId = 1,
            dueDate = "2020-01-01T00:00:00Z"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/requests",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Contains("Due date must be in the future", responseBody);
    }

[Fact]
public async Task CreateRequest_WithInvalidPriority_ReturnsBadRequest()
{
    // Arrange
    var request = new
    {
        title = "Test request with invalid priority",
        description = "Integration test request with an invalid priority.",
        priority = "Urgent",
        workflowId = 1,
        dueDate = "2026-12-31T00:00:00Z"
    };

    // Act
    var response = await _client.PostAsJsonAsync(
        "/api/requests",
        request);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    var responseBody = await response.Content.ReadAsStringAsync();

    Assert.Contains("priority", responseBody, StringComparison.OrdinalIgnoreCase);
}
}
