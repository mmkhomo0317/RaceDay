using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;

namespace RaceDay.Tests;

public class AuthorizationTests : IClassFixture<ApiFactory<Program>>
{
    private readonly HttpClient _client;
    public AuthorizationTests(ApiFactory<Program> factory) => _client = factory.CreateClient();

    private Task<HttpResponseMessage> Send(HttpMethod method, string url, string? role = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (role != null) request.Headers.Add(TestAuthHandler.RoleHeader, role);
        return _client.SendAsync(request);
    }

    [Fact]
    public async Task Unauthenticated_request_is_rejected_with_401()
    {
        var response = await Send(HttpMethod.Get, "/api/events");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("Organiser")]
    [InlineData("Participant")]
    public async Task Authenticated_roles_can_list_events(string role)
    {
        var response = await Send(HttpMethod.Get, "/api/events", role);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}