using System.Threading.Tasks;
using MongoDB.Driver;   
using TransferBuddy.Worker.Models;

namespace TransferBuddy.Worker.Repositories
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

        public async Task<decimal> Get()
        {
            var sort = Builders<Rate>.Sort.Ascending("Date");
            var cursor = await this.collection.FindAsync(Builders<Rate>.Filter.Empty);
            
            var rate = default(decimal);
            while (await cursor.MoveNextAsync())
            {
                var batch = cursor.Current;
                foreach (var p in batch)
                {
                    rate = p.Value;
                    break;
                }
            }

            return rate;
        }
    }
}