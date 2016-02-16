using AdminPanel.Entities;
using System.Linq;

namespace AdminPanel.Abstract
{
    public interface ISizesRepository
    {
        IQueryable<Size> GetSizes();

        void Add(Size t);
        void Delete(Size t);

        Size Get(int id);
    }
}
