using Models;
using Repository;
using Utils;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> userRepository;
        //inject IRepository
        public UserService(IRepository<User> repository)
        => userRepository = repository;
        public User? Create(User user)
        {
            if (user.Email != null && (FindByEmail(user.Email) != null)) throw new Exception("User already exists");
            return userRepository.Add(user);
        }

        public IEnumerable<User> FindAll() => userRepository.FindAll();

        public User? FindByID(long id) => userRepository.FindByID(id);

        public void RemoveUser(long id) => userRepository.Remove(id);

        public bool SignedInValidated(long id, string code)
        {
            User? user = this.FindByID(id);
            if (user == null)
            {
                return false;
            }
            if(TOTPUtils.VerifyCode(user.ActivationKey, code)){
                user.SignedInValidated = 1;
                this.userRepository.Update(user);
                return true;
            }else{
                user.SignedInValidated = 0;
                this.userRepository.Update(user);
                return false;
            }

        }

        public bool Signin(string email, string password)
        {
            User? user = this.userRepository.FindByEmail(email);
            if (user?.Email != null)
            {
                if (user.Email == email && user.Password == password)
                {
                    this.userRepository.UpdateSignedInStatus(user.Id, 1);
                    return true;
                }
                else
                {
                    this.userRepository.UpdateSignedInStatus(user.Id, 0);
                    return false;
                }
            }
            else
            {
                throw new InvalidOperationException("The user doesn't exist");
            }
        }

        public User UpdateUser(User user) => userRepository.Update(user);

        private User? FindByEmail(string email) => userRepository.FindByEmail(email);
    }
}