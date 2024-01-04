using SecurityAPI.Repositories;
using SecurityAPI.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DigiFiler_API.Repositories
{
    public class BaseRepository : IRepository
    {
        protected readonly AppDbContext _appDbContext;

        public BaseRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public void Add<T>(T entity) where T : class
        {
            _appDbContext.Set<T>().Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _appDbContext.Set<T>().Remove(entity);
        }


        public void Update<T>(T entity) where T : class
        {
            _appDbContext.Set<T>().Update(entity);
        }


        public async Task<bool> SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync() > 0;
        }

        public async Task<T[]> GetAllAsync<T>() where T : class
        {
            return await _appDbContext.Set<T>().ToArrayAsync();
        }


        public async Task<T> GetByIdAsync<T>(int id) where T : class
        {
            return await _appDbContext.Set<T>().FindAsync(id);
        }

        public async void Attach<T>(T entity) where T : class
        {
            _appDbContext.Attach(entity);
        }
    }
}
