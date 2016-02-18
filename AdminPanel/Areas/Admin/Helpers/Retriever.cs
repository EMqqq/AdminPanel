using AdminPanel.DataAccessLayer;
using AdminPanel.Entities;
using System.Linq;

namespace AdminPanel.Areas.Admin.Helpers
{
    public static class Retriever
    {
        public static IQueryable<Color> GetColors()
        {
            AdminPanelContext context = new AdminPanelContext();
            IQueryable<Color> colorsQuery = from p in context.Colors
                                            orderby p.ColorName
                                            select p;
            return colorsQuery;
        }

        public static IQueryable<DeliveryMethod> GetDeliveryMethods()
        {
            AdminPanelContext context = new AdminPanelContext();
            IQueryable<DeliveryMethod> deliveryMethodQuery = from c in context.DeliveryMethods
                                                             orderby c.DeliveryMethodId
                                                             select c;
            return deliveryMethodQuery;
        }

        public static IQueryable<Category> GetCategories()
        {
            AdminPanelContext context = new AdminPanelContext();
            IQueryable<Category> categoriesQuery = from c in context.Categories
                                                          orderby c.CategoryName
                                                          select c;
            return categoriesQuery;
        }
    }
}