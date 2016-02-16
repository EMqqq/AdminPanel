using System.Linq;

namespace AdminPanel.Abstract
{
    public interface ITRepository<TObject> where TObject : class
    {
        int Count();

        void Add(TObject t);
        void Update(TObject updated);
        void Delete(TObject t);

        TObject Get(int id);
        IQueryable<TObject> GetAll { get; }
    }
}
