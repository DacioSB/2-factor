using Models;

namespace Repository
{
    public interface IRepository<T>
{
    string? ConnectionString { get; }
    T? Add(T item);
    void Remove(long id);
    T Update(T item);
    T? FindByID(long id);
    IEnumerable<T> FindAll();
    T? FindByEmail(string email);
    void UpdateSignedInStatus(long userId, int signedInValue);
}
}
