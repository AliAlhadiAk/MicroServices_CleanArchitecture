using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace PlatformService.Tests
{
	public class PlatformIntegrationTests : IClassFixture<WebApplicationFactory<Program>> // or Startup
	{
		private readonly HttpClient _client;

		public PlatformIntegrationTests(WebApplicationFactory<Program> factory)
		{
			_client = factory.CreateClient();
		}

		[Fact]
		public async Task Test_GetAllPlatforms_ReturnsSuccess()
		{
			var response = await _client.GetAsync("/api/platforms");

			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			Assert.Contains("Test Platform", responseString);
		}

		[Fact]
		public async Task Test_CreatePlatform_ReturnsSuccess()
		{
			var newPlatform = new { Name = "New Test Platform" };
			var content = new StringContent(JsonSerializer.Serialize(newPlatform), Encoding.UTF8, "application/json");

			var response = await _client.PostAsync("/api/platforms", content);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			Assert.Contains("New Test Platform", responseString);
		}
	}
}
