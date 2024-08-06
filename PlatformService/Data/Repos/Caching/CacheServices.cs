using Microsoft.EntityFrameworkCore.Storage;
using PlatformService.Data.Repos.Caching;
using StackExchange.Redis;
using System.Text.Json;
using IDatabase = StackExchange.Redis.IDatabase;

namespace BlogApp.Net.Services
{
	public class CacheService : ICacheService
	{
		private IDatabase _cachedb;

		public CacheService()
		{
			var redis = ConnectionMultiplexer.Connect("localhost:6379");
			_cachedb = redis.GetDatabase();
		}

		public T GetData<T>(string key)
		{
			var value = _cachedb.StringGet(key);
			if (!string.IsNullOrEmpty(value))
			{
				return JsonSerializer.Deserialize<T>(value);
			}
			return default;
		}

		public object RemoveData(string key)
		{
			var exists = _cachedb.KeyExists(key);

			if (exists)
			{
				return _cachedb.KeyDelete(key);
			}
			return false;
		}

		public bool SetData<T>(string key, T value, TimeSpan expirationTime)
		{
			var adjustedExpiration = expirationTime.Add(TimeSpan.FromMinutes(2));
			var isSet = _cachedb.StringSet(key, JsonSerializer.Serialize(value), adjustedExpiration);
			return isSet;
		}

		

		public bool UpdateCacheIfExists<T>(string key, T value, TimeSpan expirationTime)
		{
			var exists = _cachedb.KeyExists(key);

			if (exists)
			{
				var serializedValue = JsonSerializer.Serialize(value);
				var isUpdated = _cachedb.StringSet(key, serializedValue, expirationTime);
				return isUpdated;
			}
			return false;
		}
	}
}