using Domain.Interface.Service;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class GenericDataService<T> : IDataService<T> where T : class
    {
        private readonly ApplicationContextFactory _contextFactory;
        private readonly NonqueryDataService<T> _nonQueryDataService;

        public GenericDataService(ApplicationContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonqueryDataService<T>(contextFactory);

        }


        public async Task<T> Create(T entity)
        {
            return await _nonQueryDataService.Create(entity);
        }

        public async Task<bool> CreateRange(List<T> entity)
        {
            return await _nonQueryDataService.CreateRange(entity);
        }

        public async Task<bool> Delete(T entity)
        {
            return await _nonQueryDataService.Delete(entity);
        }

        public async Task<T> Get(int id)
        {
            using ApplicationDBContext context = _contextFactory.CreateDbContext();
            return await context.Set<T>().FindAsync(id);
        }


        public async Task<IEnumerable<T>> GetAll()
        {
            using ApplicationDBContext context = _contextFactory.CreateDbContext();
            return await context.Set<T>().ToListAsync();

        }

        public async Task<List<T>> GetAllAsync(int pageSize)
        {
            using ApplicationDBContext context = _contextFactory.CreateDbContext();
            return await context.Set<T>().Take(pageSize).ToListAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            using ApplicationDBContext context = _contextFactory.CreateDbContext();
            return await context.Set<T>().ToListAsync();
        }

        public async Task<T> Update(T entity)
        {
            return await _nonQueryDataService.Update(entity);
        }

        public async Task<List<T>> Filter(string query)
        {
            using ApplicationDBContext context = _contextFactory.CreateDbContext();
            return await context.Set<T>().FromSqlRaw(query).ToListAsync();
        }

        public async Task<Domain.Entities.Meter> GetByMeterNoAsync(string MeterNo)
        {
            using ApplicationDBContext context = _contextFactory.CreateDbContext();
            return await context.Set<Domain.Entities.Meter>().FirstOrDefaultAsync(x=> x.MeterNo == MeterNo);
        }
    }
}
