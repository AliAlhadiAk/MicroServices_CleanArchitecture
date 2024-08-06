using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using StackExchange.Redis;
using Xunit;

namespace MyApp.IntegrationTests
{
	public class RedisIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string _redisConnectionString = "localhost:6379";

		public RedisIntegrationTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task Redis_Connection_IsSuccessful()
		{

			var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);
			var database = connectionMultiplexer.GetDatabase();


			var key = "test-key";
			var value = "test-value";
			await database.StringSetAsync(key, value);
			var storedValue = await database.StringGetAsync(key);


			Assert.Equal(value, storedValue);
		}
	}
}