namespace DAL.Interfaces
{
    public interface IRepository<T>
    {
        void Add(T entity);

        T? Get(int id);

        void Update(T entity);

        void Delete(T entity);

        IEnumerable<T> GetAll();
    }
}
