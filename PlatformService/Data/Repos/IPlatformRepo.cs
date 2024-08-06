using PlatformService.Models;

namespace PlatformService.Data.Repos
{
	public interface IPlatformRepo
	{
		Task<bool> SaveChanges();

		Task<IEnumerable<Platfrom>> GetAllPlatforms();
		Task<Platfrom> GetPlafromById(int id);
		Task CreatePlatform(Platfrom platfrom);
	}
}
