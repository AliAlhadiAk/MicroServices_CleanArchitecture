using AutoMapper;
using PlatformService.Dto_s;
using PlatformService.Models;

namespace PlatformService.Profiles
{
	public class PlatformsProfile : Profile
	{
        public PlatformsProfile()
        {
            CreateMap<Platfrom, PlatformReadDto>();
			CreateMap<PlatformCreateDto, Platfrom>();
		}
    }
}
