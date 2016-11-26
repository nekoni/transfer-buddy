using System;
using System.Threading.Tasks;
using TransferBuddy.Worker.Models;
using MongoDB.Driver;

namespace TransferBuddy.Worker.Repositories
{
    public class UserRepository : MongoRepository
    {
        private IMongoCollection<User> collection;

        public UserRepository ()
        {
            this.collection = this.Db.GetCollection<User>("Users");
        }

        public async Task<User> GetAsync(string userId)
        {
            var users = await this.collection.FindAsync(f => f.UserId == userId);
            return await users.FirstOrDefaultAsync();
        }

        public async Task<User> InsertAsync(User user)
        {
            user.Created = DateTime.UtcNow;
            user.LastActivity = user.Created;
            await this.collection.InsertOneAsync(user);

            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.LastActivity = DateTime.UtcNow;
            await this.collection.UpdateOneAsync(
                Builders<User>.Filter.Eq("UserId", user.UserId), 
                Builders<User>.Update.Set("LastActivity", user.LastActivity));

            return user;
        }
    }
}