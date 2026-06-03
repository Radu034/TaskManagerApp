using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManagerApp.API.Data;
using TaskManagerApp.API.Services;

namespace TaskManagerApp.Tests;

public class AuthServiceTests
{
    private static AuthService CreateService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var ctx = new AppDbContext(options);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test-secret-key-for-unit-tests-minimum-32-chars",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            })
            .Build();

        return new AuthService(ctx, config);
    }

    [Fact]
    public void HashPassword_ReturnsNonEmptyHash()
    {
        var service = CreateService();
        var hash = service.HashPassword("password123");
        hash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void VerifyPassword_ReturnsTrue_WhenPasswordMatches()
    {
        var service = CreateService();
        var hash = service.HashPassword("mypassword");
        var result = service.VerifyPassword(hash, "mypassword");
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ReturnsFalse_WhenPasswordWrong()
    {
        var service = CreateService();
        var hash = service.HashPassword("correctpassword");
        var result = service.VerifyPassword(hash, "wrongpassword");
        result.Should().BeFalse();
    }
}
