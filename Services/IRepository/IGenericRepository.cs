namespace IRepository
{
    public interface IGenericRepository<T> where T :class
    {
        Task<T> GetByData(int Id);
        Task<List<T>> GetAll();
        Task Register(T Entity);
        Task Update(T Entity);
        Task Delete(T Entity);
        Task Save();
    }
}