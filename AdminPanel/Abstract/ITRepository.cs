using System.Data.Entity;
using System.Linq;

namespace AdminPanel.Abstract
{
    public interface ITRepository<C, TObject> where TObject : class where C : DbContext
    {
        int Count();

        void Add(TObject t);
        void Delete(TObject t);
        void Save();
        void Update(TObject updated);

        TObject Get(int id);
        IQueryable<TObject> GetAll { get; }
    }
}
