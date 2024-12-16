using Models;

namespace Service
{
    public interface IUserService
    {
        public User? Create(User user);
        public User? FindByID(long id);
        //findall
        public IEnumerable<User> FindAll();
        public void RemoveUser(long id);
        public User UpdateUser(User user);

        public Boolean Signin(string email, string password);
        public Boolean SignedInValidated(long id, string code);
    }
}