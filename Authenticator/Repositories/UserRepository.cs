using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Authenticator.Models;

namespace Authenticator.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserConnection> _users;

        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<UserConnection>("userConnection");
        }

        public async Task<UserConnection> Get(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserConnection>> GetAll()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        public async Task Add(UserConnection user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task Update(UserConnection user)
        {
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        public async Task Delete(string id)
        {
            await _users.DeleteOneAsync(user => user.Id == id);
        }
    }
}