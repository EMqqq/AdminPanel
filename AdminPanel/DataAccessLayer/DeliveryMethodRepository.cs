using AdminPanel.Entities;
using AdminPanel.Abstract;
using System.Linq;

namespace AdminPanel.DataAccessLayer
{
    public class DeliveryMethodRepository : IDeliveryMethodRepository
    {
        private ITRepository<DeliveryMethod> deliveryRepository;

        public DeliveryMethodRepository(ITRepository<DeliveryMethod> deliveryRepository)
        { this.deliveryRepository = deliveryRepository; }

        public IQueryable<DeliveryMethod> GetDeliveryMethods()
        { return deliveryRepository.GetAll; }

        public DeliveryMethod Get(int id)
        { return deliveryRepository.Get(id); }

        public void Add(DeliveryMethod delivery)
        { deliveryRepository.Add(delivery); }

        public void Delete(DeliveryMethod delivery)
        { deliveryRepository.Delete(delivery); }
    }
}