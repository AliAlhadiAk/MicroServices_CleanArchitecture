using Moq;
using PlatformService.Data.Repos;
using PlatformService.Models;
using PlatformService.Data.Repos.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using PlatformService.Data;
using System.Runtime.InteropServices;

namespace PlatformService.Tests
{
	public class PlatformRepoTests
	{
		private readonly PlatformRepo _repo;
		private readonly Mock<DbSet<Platfrom>> _mockSet;
		private readonly Mock<AppDbContext> _mockContext;
		private readonly Mock<ICacheService> _mockCacheService;
		private readonly Mock<ILogger<PlatformRepo>> _mockLogger;

		public PlatformRepoTests()
		{
			_mockSet = new Mock<DbSet<Platfrom>>();
			_mockContext = new Mock<AppDbContext>();
			_mockCacheService = new Mock<ICacheService>();
			_mockLogger = new Mock<ILogger<PlatformRepo>>();

			// Setup the mock DbSet
			_mockContext.Setup(m => m.Platforms).Returns(_mockSet.Object);

			// Instantiate PlatformRepo with mocks
			_repo = new PlatformRepo(_mockContext.Object, _mockLogger.Object, _mockCacheService.Object);
		}

		[Fact]
		public async Task CreatePlatform_ShouldAddPlatformAndUpdateCache()
		{
			// Arrange
			var platform = new Platfrom { Id = 1, Name = "Test Platform" };
			_mockSet.Setup(m => m.AddAsync(platform, default)).ReturnsAsync((Platfrom)platform);

			
			await _repo.CreatePlatform(platform);

			_mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
			_mockCacheService.Verify(c => c.UpdateCacheIfExists<IEnumerable<Platfrom>>(
				"platform", It.IsAny<IEnumerable<Platfrom>>(), TimeSpan.FromMinutes(2)), Times.Once);
		}

		[Fact]
		public async Task GetAllPlatforms_ShouldReturnCachedPlatforms()
		{
			var platformList = new List<Platfrom>
			{
				new Platfrom { Id = 1, Name = "Test Platform" }
			};
			_mockCacheService.Setup(c => c.GetData<IEnumerable<Platfrom>>("all_platforms"))
				.Returns(platformList);

			var result = await _repo.GetAllPlatforms();

			Assert.Single(result);
			Assert.Equal("Test Platform", result.First().Name);
			_mockContext.Verify(m => m.Platforms.ToListAsync(default), Times.Never);
		}

		[Fact]
		public async Task GetPlatformById_ShouldReturnPlatform()
		{
			var platform = new Platfrom { Id = 1, Name = "Test Platform" };
			_mockCacheService.Setup(c => c.GetData<Platfrom>("platform_1"))
				.Returns(platform);

			var result = await _repo.GetPlafromById(1);

			Assert.NotNull(result);
			Assert.Equal("Test Platform", result.Name);
			_mockContext.Verify(m => m.Platforms.FirstOrDefaultAsync(It.IsAny<Expression<Func<Platfrom, bool>>>(), default), Times.Never);
		}

		[Fact]
		public async Task SaveChanges_ShouldReturnTrueIfChangesSaved()
		{
			_mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);
			var result = await _repo.SaveChanges();

			Assert.True(result);
		}
	}
}
