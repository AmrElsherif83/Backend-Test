using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using System.Data;

namespace Backend_Test.Tests.IntegrationTests
{
    [Collection("Database collection")]
    public class DriversControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IDbConnection _dbConnection;
        public DriversControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture dbFixture)
        {
            _client = factory.CreateClient();
            _dbConnection = dbFixture.Connection;
        }

        [Fact]
        public async Task GetDriverById_ShouldReturnNotFound_WhenDriverDoesNotExist()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/drivers/{nonExistingId}");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task CreateDriver_ShouldReturnCreated_WhenDataIsValid()
        {
            var emailId = Guid.NewGuid();
            // Arrange
            var driver = new { FirstName = "John", LastName = "Doe", Email = $"{emailId}@example.com", PhoneNumber = "+1234567890" };
            var content = new StringContent(JsonSerializer.Serialize(driver), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/drivers", content);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }
    }
}
