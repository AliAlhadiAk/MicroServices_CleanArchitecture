using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlatformService.Controllers;
using PlatformService.Data.Repos;
using PlatformService.Dto_s;
using PlatformService.Services.RabbitMq_MassTransit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using PlatformService.Models;

namespace PlatformService.Tests
{
	public class PlatformControllerTests
	{
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<IPlatformRepo> _platformRepoMock;
		private readonly Mock<IDriverNotificationPublisherService> _driverNotificationMock;
		private readonly PlatformController _controller;

		public PlatformControllerTests()
		{
			_mapperMock = new Mock<IMapper>();
			_platformRepoMock = new Mock<IPlatformRepo>();
			_driverNotificationMock = new Mock<IDriverNotificationPublisherService>();
			_controller = new PlatformController(_mapperMock.Object, _platformRepoMock.Object, _driverNotificationMock.Object);
		}

		[Fact]
		public async Task GetAllPlatforms_ReturnsOkResult_WithListOfPlatforms()
		{
			// Arrange
			var platforms = new List<Platfrom>
			{
				new Platfrom { Id = 1, Name = "Platform1" },
				new Platfrom { Id = 2, Name = "Platform2" }
			};
			_platformRepoMock.Setup(repo => repo.GetAllPlatforms()).ReturnsAsync(platforms);

			var platformDtos = new List<PlatformReadDto>
			{
				new PlatformReadDto { Id = 1, Name = "Platform1" },
				new PlatformReadDto { Id = 2, Name = "Platform2" }
			};
			_mapperMock.Setup(m => m.Map<IEnumerable<PlatformReadDto>>(It.IsAny<IEnumerable<Platfrom>>())).Returns(platformDtos);

			// Act
			var result = await _controller.GetAllPlatforms() as OkObjectResult;

			// Assert
			Assert.NotNull(result);
			Assert.IsType<OkObjectResult>(result);
			var returnValue = result.Value as IEnumerable<PlatformReadDto>;
			Assert.NotNull(returnValue);
			Assert.Equal(2, returnValue.Count());
		}

		[Fact]
		public async Task GetPlatformById_ReturnsOkResult_WithPlatformDto()
		{
			// Arrange
			var platform = new Platfrom { Id = 1, Name = "Platform1" };
			_platformRepoMock.Setup(repo => repo.GetPlafromById(1)).ReturnsAsync(platform);

			var platformDto = new PlatformReadDto { Id = 1, Name = "Platform1" };
			_mapperMock.Setup(m => m.Map<PlatformReadDto>(It.IsAny<Platfrom>())).Returns(platformDto);

			// Act
			var result = await _controller.GetPlatformById(1) as OkObjectResult;

			// Assert
			Assert.NotNull(result);
			Assert.IsType<OkObjectResult>(result);
			var returnValue = result.Value as PlatformReadDto;
			Assert.NotNull(returnValue);
			Assert.Equal(1, returnValue.Id);
		}

		[Fact]
		public async Task GetPlatformById_ReturnsNotFound_WhenPlatformDoesNotExist()
		{
			_platformRepoMock.Setup(repo => repo.GetPlafromById(It.IsAny<int>())).ReturnsAsync((Platfrom)null);

			var result = await _controller.GetPlatformById(1);

			Assert.IsType<NotFoundResult>(result);
		}
	}
}
