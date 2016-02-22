using AdminPanel.Abstract;
using System.Data.Entity;
using System.Linq;

namespace AdminPanel.DataAccessLayer
{
    public class TRepository<C, TObject> : ITRepository<C, TObject> where TObject : class where C : DbContext
    {
        protected C _context;
        private IDbSet<TObject> _entities;

        public TRepository(C context)
        {
            _context = context;
        }

        protected IDbSet<TObject> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<TObject>();
                }
                return _entities;
            }
        }

        public TObject Get(int id)
        {
            return Entities.Find(id);
        }

        public IQueryable<TObject> GetAll
        {
            get { return Entities; }
        }

        public virtual void Add(TObject t)
        {
            Entities.Add(t);
            Save();
        }

        public void Update(TObject entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            Save();
        }

        public virtual void Delete(TObject t)
        {
            Entities.Remove(t);
            Save();
        }

        public int Count()
        {
            return Entities.Count();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}