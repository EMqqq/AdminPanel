using AdminPanel.Abstract;
using AdminPanel.Entities;
using System.Linq;

namespace AdminPanel.DataAccessLayer
{
    public class SizesRepository : ISizesRepository
    {
        private ITRepository<Size> sizeRepository;

        public SizesRepository(ITRepository<Size> sizeRepository)
        { this.sizeRepository = sizeRepository; }

        public IQueryable<Size> GetSizes()
        { return sizeRepository.GetAll; }

        public Size Get(int id)
        { return sizeRepository.Get(id); }

        public void Add(Size size)
        { sizeRepository.Add(size); }

        public void Delete(Size size)
        { sizeRepository.Delete(size); }
    }
}