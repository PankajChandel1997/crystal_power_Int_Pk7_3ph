using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Service
{
    public interface IDataService<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<List<T>> GetAllAsync(int pageSize);
        Task<List<T>> GetAllAsync();
        Task<T> Get(int id);
        Task<T> Create(T entity);
        Task<bool> CreateRange(List<T> entity);
        Task<bool> Delete(T entity);
        Task<T> Update(T entity);
        Task<List<T>> Filter(string query);
        Task<Domain.Entities.Meter> GetByMeterNoAsync(string meterNo);
    }
}
