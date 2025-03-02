namespace DAL.Interfaces;

public interface IRepository<T>
{
    Task Add(T entity, CancellationToken cancellationToken);
    Task Update(T entity, CancellationToken cancellationToken);
    Task Delete(int id, CancellationToken cancellationToken);
    Task<List<T>> GetAll(CancellationToken cancellationToken);
    Task<T> GetById(int id, CancellationToken cancellationToken);
}