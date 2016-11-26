using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;   
using TransferBuddy.Models;

namespace TransferBuddy.Repositories
{
    public class RateRepository : MongoRepository
    {
        private IMongoCollection<Rate> collection;

        public RateRepository(string func, string source, string target)
        {
            this.collection = this.Db.GetCollection<Rate>($"{func}_{source}_{target}");
        }

        public async Task Add(Rate rate)
        {
            await this.collection.InsertOneAsync(rate);
        }

        public async Task<IEnumerable<Rate>> Get()
        {
            var sort = Builders<Rate>.Sort.Ascending("Date");
            var cursor = await this.collection.FindAsync(Builders<Rate>.Filter.Empty);
            
            var rates = new List<Rate>();
            while (await cursor.MoveNextAsync())
            {
                var batch = cursor.Current;
                foreach (var p in batch)
                {
                    rates.Add(p);
                }
            }

            return rates;
        }
    }
}