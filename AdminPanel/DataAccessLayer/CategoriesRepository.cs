using AdminPanel.Abstract;
using AdminPanel.Entities;
using System.Linq;

namespace AdminPanel.DataAccessLayer
{
    public class CategoriesRepository : ICategoriesRepository
    {
        AdminPanelContext db = new AdminPanelContext();
        private ITRepository<Category> categoryRepository;

        public CategoriesRepository(ITRepository<Category> categoryRepository)
        { this.categoryRepository = categoryRepository; }

        public IQueryable<Category> GetCategories()
        { return categoryRepository.GetAll; }

        public Category Get(int id)
        { return categoryRepository.Get(id); }

        public void Add(Category category)
        { categoryRepository.Add(category); }

        public void Delete(Category category)
        { categoryRepository.Delete(category); }
    }
}