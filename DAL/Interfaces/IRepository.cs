namespace DAL.Interfaces;

public interface IRepository<T>
{
    Task Add(T entity);
    Task Update(T entity);
    Task Delete(int id);
    Task<List<T>> GetAll();
    Task<T> GetById(int id);
}