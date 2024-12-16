using Microsoft.Data.Sqlite;
using Models;
using RepoDb;

namespace Repository
{
    public class UserRepository : BaseRepository<User, SqliteConnection>, IRepository<User>
    {
        private string? _connectionString;
        public new string? ConnectionString => _connectionString;
        //the db is in a folder called db in the root of the project (db/sqlite.db)
        public UserRepository(IConfiguration configuration) : base(configuration.GetValue<string>("DBInfo:ConnectionString"))
        {
            GlobalConfiguration
                .Setup()
                .UseSqlite();
            _connectionString = configuration.GetValue<string>("DBInfo:ConnectionString");
        }

        public User? Add(User user)
        {
            using (var dbConnection = new SqliteConnection(ConnectionString))
            {
                var id = dbConnection.Insert<User, int>(user);
                return FindByID(id);
            }
        }

        public IEnumerable<User> FindAll()
        {
            using (var dbConnection = new SqliteConnection(ConnectionString))
            {
                return dbConnection.QueryAll<User>();
            }
        }

        public User? FindByID(long id)
        {
            using (var dbConnection = new SqliteConnection(ConnectionString))
            {
                return dbConnection.Query<User>(u => u.Id == id).FirstOrDefault();
            }
        }

        public void Remove(long id)
        {
            using (var dbConnection = new SqliteConnection(ConnectionString))
            {
                dbConnection.Delete<User>(id);
            }
        }

        public User Update(User user)
        {
            using (var dbConnection = new SqliteConnection(ConnectionString))
            {
                dbConnection.Merge(user);
                var foundUser = FindByID(user.Id);
                if (foundUser != null)
                {
                    return foundUser;
                }
                else
                {
                    throw new Exception("User not found");
                }
            }
        }

        public void UpdateSignedInStatus(long userId, int signedInValue)
        {
            var userToUpdate = FindByID(userId);
            if (userToUpdate == null)
            {
                throw new ArgumentException($"User with id: {userId} not found");
            }

            userToUpdate.SignedIn = signedInValue;
            Update(userToUpdate);
        }


        public User? FindByEmail(string email)
        {
            using (var dbConnection = new SqliteConnection(ConnectionString))
            {
                return dbConnection.Query<User>(u => u.Email == email).FirstOrDefault();
            }
        }

    }
}
