using System.Collections.Generic;
using System.Threading.Tasks;
using Authenticator.Models;

namespace Authenticator.Services
{
    public interface IUserConnectionService
    {
        Task<UserConnection> Get(string id);
        Task<IEnumerable<UserConnection>> GetAll();
        Task Add(UserConnection user);
        Task Update(UserConnection user);
        Task Delete(string id);
    }
}