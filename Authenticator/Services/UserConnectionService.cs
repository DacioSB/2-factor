using System.Collections.Generic;
using System.Threading.Tasks;
using Authenticator.Models;
using Authenticator.Repositories;
using Util;

namespace Authenticator.Services
{
    public class UserConnectionService : IUserConnectionService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserConnectionService> _logger;

        public UserConnectionService(IUserRepository userRepository, ILogger<UserConnectionService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserConnection> Get(string id)
        {
            UserConnection userConnection = await _userRepository.Get(id);
            _logger.LogInformation(userConnection.Secret);
            KeyValuePair<string,string>[] result = TOTPGenerator.GenerateTOTP(userConnection.Secret).ToArray();
            Dictionary<string, string> dict = result.ToDictionary(item => item.Key, item => item.Value);
            userConnection.totp = dict["totp"];
            userConnection.futureTotp = dict["future_totp"];
            userConnection.remainingSeconds = int.Parse(dict["remainingSeconds"]);
            return userConnection;
        }

        public async Task<IEnumerable<UserConnection>> GetAll()
        {
            return await _userRepository.GetAll();
        }

        public async Task Add(UserConnection user)
        {
            await _userRepository.Add(user);
        }

        public async Task Update(UserConnection user)
        {
            await _userRepository.Update(user);
        }

        public async Task Delete(string id)
        {
            await _userRepository.Delete(id);
        }
    }
}