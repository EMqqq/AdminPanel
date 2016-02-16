using AdminPanel.Entities;
using System.Linq;

namespace AdminPanel.Abstract
{
    public interface ICategoriesRepository
    {
        IQueryable<Category> GetCategories();

        void Add(Category t);
        void Delete(Category t);

        Category Get(int id);
    }
}
