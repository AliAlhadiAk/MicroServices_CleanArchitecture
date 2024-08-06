using BlogApp.Net.Services;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data.Repos.Caching;
using PlatformService.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace PlatformService.Data.Repos
{
	public class PlatformRepo : IPlatformRepo
	{
		private readonly AppDbContext _context;
		private readonly ILogger<PlatformRepo> _logger;
		private readonly ICacheService _cacheService;

        public PlatformRepo(AppDbContext context,ILogger<PlatformRepo> logger,ICacheService cacheService)
        {
            _context = context;
			_logger = logger;
			_cacheService = cacheService;
        }
        public async Task CreatePlatform(Platfrom platfrom)
		{
			if (platfrom == null)
			{
				throw new ArgumentNullException(nameof(platfrom));
			}

			await _context.Platforms.AddAsync(platfrom);
			await _context.SaveChangesAsync();
			var platforms = await _context.Platforms.ToListAsync();
			_cacheService.UpdateCacheIfExists<IEnumerable<Platfrom>>("platform", platforms, TimeSpan.FromMinutes(2));

		}

		public async Task<IEnumerable<Platfrom>> GetAllPlatforms()
		{
			const string cacheKey = "all_platforms";
			const int cacheDurationInMinutes = 10; 

			var cachedPlatforms = _cacheService.GetData<IEnumerable<Platfrom>>(cacheKey);
			if (cachedPlatforms != null)
			{
				return cachedPlatforms;
			}
			var platforms = await _context.Platforms.ToListAsync();
			_cacheService.SetData(cacheKey, platforms, TimeSpan.FromMinutes(cacheDurationInMinutes));

			return platforms;
		}

		public async  Task<Platfrom> GetPlafromById(int id)
		{
			if (id <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
			}
			var cacheKey = $"platform_{id}";
			var cachedPlatform = _cacheService.GetData<Platfrom>(cacheKey);
			if (cachedPlatform != null)
			{
				return cachedPlatform;
			}
			var platform = await _context.Platforms.FirstOrDefaultAsync(p => p.Id == id);
			if (platform != null)
			{
				_cacheService.SetData(cacheKey, platform, TimeSpan.FromMinutes(2));
			}

			return platform;
		}

		public async Task<bool> SaveChanges()
		{
			return (await _context.SaveChangesAsync() >= 0);
		}
	}
}
