using System.Threading.Tasks;

namespace SecurityAPI.Repositories
{
    public interface IRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();
        Task<T[]> GetAllAsync<T>() where T : class;
        Task<T> GetByIdAsync<T>(int id) where T : class;
        void Attach<T>(T entity) where T : class;
        void UpdaterRange<T>(ICollection<T> entities) where T : class;

        void DeleteRange<T>(IEnumerable<T> entities) where T : class;

        void AddRange<T>(IEnumerable<T> entities) where T : class;
    }
}