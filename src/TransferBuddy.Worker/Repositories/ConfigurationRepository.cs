using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;   
using TransferBuddy.Models;

namespace TransferBuddy.Repositories
{
    public class ConfigurationRepository : MongoRepository
    {
        private IMongoCollection<TransferConfig> collection;

        public ConfigurationRepository()
        {
            this.collection = this.Db.GetCollection<TransferConfig>("config");
        }

        public async Task<List<TransferConfig>> Get()
        {
            var cursor = await this.collection.FindAsync(Builders<TransferConfig>.Filter.Empty);
            
            var configs = new List<TransferConfig>();
            while (await cursor.MoveNextAsync())
            {
                var batch = cursor.Current;
                foreach (var p in batch)
                {
                    configs.Add(p);
                }
            }

            return configs;
        }

        public async Task Create(TransferConfig config)
        {
            await this.collection.InsertOneAsync(config);
        }
    }
}