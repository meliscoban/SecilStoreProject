using MongoDB.Driver;
using MongoDB.Bson;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace DataAccess.Repositories
{
	public class MongoConfigurationRepository : IConfigurationRepository
	{
		private readonly IMongoCollection<ConfigurationRecord> _collection;

		public MongoConfigurationRepository(string connectionString, string databaseName, string collectionName)
		{
			var client = new MongoClient(connectionString);
			var database = client.GetDatabase(databaseName);
			_collection = database.GetCollection<ConfigurationRecord>(collectionName);
		}

		public async Task<IEnumerable<ConfigurationRecord>> GetActiveConfigurationsAsync(string applicationName)
		{
			var filter = Builders<ConfigurationRecord>.Filter.And(
				Builders<ConfigurationRecord>.Filter.Eq(r => r.ApplicationName, applicationName),
				Builders<ConfigurationRecord>.Filter.Eq(r => r.IsActive, true)
			);

			var asd = _collection.Find(filter).ToListAsync();

			return await _collection.Find(filter).ToListAsync();
		}

		public async Task<ConfigurationRecord> GetConfigurationAsync(ObjectId id)
		{
			var filter = Builders<ConfigurationRecord>.Filter.Eq(r => r.Id, id);

			return await _collection.Find(filter).FirstOrDefaultAsync();
		}

		public async Task AddOrUpdateConfigurationAsync(ConfigurationRecord record)
		{
			var filter = Builders<ConfigurationRecord>.Filter.And(
				Builders<ConfigurationRecord>.Filter.Eq(r => r.ApplicationName, record.ApplicationName),
				Builders<ConfigurationRecord>.Filter.Eq(r => r.Name, record.Name)
			);

			await _collection.ReplaceOneAsync(filter, record, new ReplaceOptions { IsUpsert = true });
		}

		public async Task DeleteConfigurationAsync(ObjectId id)
		{
			var filter = Builders<ConfigurationRecord>.Filter.Eq(r => r.Id, id);

			await _collection.DeleteOneAsync(filter);
		}
	}
}
