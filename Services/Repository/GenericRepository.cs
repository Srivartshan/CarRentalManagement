using CarRentalManagement.Data;
using Microsoft.EntityFrameworkCore;
using IRepository;

namespace Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly CarRentalContext _carRentalContext;

        public GenericRepository(CarRentalContext carRentalContext)
        {
            _carRentalContext = carRentalContext;
        }

        public async Task Delete(T Entity)
        {
            if (Entity == null)
                throw new ArgumentNullException(nameof(Entity), "Entity cannot be null.");

            _carRentalContext.Set<T>().Remove(Entity);
            await Save();
        }

        public Task<List<T>> GetAll()
        {
            return _carRentalContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByData(int Id)
        {
            if (Id <= 0)
                throw new ArgumentException("Id must be greater than 0.", nameof(Id));

            var entity = await _carRentalContext.Set<T>().FindAsync(Id);
            if (entity == null)
                throw new KeyNotFoundException($"Entity with Id {Id} was not found.");

            return entity;
        }

        public async Task Register(T Entity)
        {
            if (Entity == null)
                throw new ArgumentNullException(nameof(Entity), "Entity cannot be null.");

            await _carRentalContext.Set<T>().AddAsync(Entity);
            await Save();
        }

        public async Task Save()
        {
                await _carRentalContext.SaveChangesAsync();
        }

        public async Task Update(T Entity)
        {
            if (Entity == null)
                throw new ArgumentNullException(nameof(Entity), "Entity cannot be null.");

            _carRentalContext.Set<T>().Update(Entity);
            await Save();
        }
    }
}
