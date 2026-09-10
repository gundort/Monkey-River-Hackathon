using MongoDB.Driver;

namespace BackEnd.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task AddAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);

        Task<T> FindOneAsync(FilterDefinition<T> filter);
        Task UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update);
    }
}
