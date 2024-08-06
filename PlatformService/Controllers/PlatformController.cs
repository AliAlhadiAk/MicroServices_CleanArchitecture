using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data.Repos;
using PlatformService.Dto_s;
using PlatformService.Services.RabbitMq_MassTransit;

namespace PlatformService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlatformController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IPlatformRepo _platformRepo;
		private readonly IDriverNotificationPublisherService _driverNotification;

        public PlatformController(IMapper mapper, IPlatformRepo platformRepo, IDriverNotificationPublisherService driverNotification)
        {
            _mapper = mapper;
			_platformRepo = platformRepo;
			_driverNotification = driverNotification;
        }


		[HttpGet]
		public async Task<IActionResult> GetAllPlatforms()
		{
			Console.WriteLine("--> Getting Platforms....");

			var platformItem = await _platformRepo.GetAllPlatforms();

			return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
		}

		[HttpGet("{id}", Name = "GetPlatformById")]
		public async Task<IActionResult> GetPlatformById(int id)
		{
			var platformItem = await _platformRepo.GetPlafromById(id);
			if (platformItem != null)
			{
				return Ok(_mapper.Map<PlatformReadDto>(platformItem));
			}

			return NotFound();
		}
	}
}
