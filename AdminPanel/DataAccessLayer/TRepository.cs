using AdminPanel.Abstract;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace AdminPanel.DataAccessLayer
{
    public class TRepository<TObject> : ITRepository<TObject> where TObject : class
    {
        protected AdminPanelContext _context;
        private IDbSet<TObject> _entities;

        public TRepository(AdminPanelContext context)
        {
            _context = context;
        }

        private IDbSet<TObject> Entities
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

        public virtual IQueryable<TObject> GetAll
        {
            get { return Entities; }
        }

        public void Add(TObject t)
        {
            Entities.Add(t);
            _context.SaveChanges();
        }

        public void Update(TObject entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}",
                        validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public void Delete(TObject t)
        {
            Entities.Remove(t);
            _context.SaveChanges();
        }

        public int Count()
        {
            return Entities.Count();
        }
    }
}