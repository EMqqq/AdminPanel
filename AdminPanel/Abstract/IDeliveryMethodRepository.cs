using System.Linq;
using AdminPanel.Entities;

namespace AdminPanel.Abstract
{
    public interface IDeliveryMethodRepository
    {
        IQueryable<DeliveryMethod> GetDeliveryMethods();

        void Add(DeliveryMethod t);
        void Delete(DeliveryMethod t);

        DeliveryMethod Get(int id);
    }
}
