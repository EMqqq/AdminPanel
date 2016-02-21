using System;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using AdminPanel.Abstract;
using AdminPanel.DataAccessLayer;
using System.Data.Entity;
using AdminPanel.Entities;

namespace AdminPanel.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            ninjectKernel.Bind(typeof(ITRepository<AdminPanelContext, Color>)).To(typeof(ColorRepository));
            ninjectKernel.Bind(typeof(ITRepository<AdminPanelContext, FilePath>)).To(typeof(FilePathRepository));
            ninjectKernel.Bind(typeof(ITRepository<AdminPanelContext, Supplier>)).To(typeof(SupplierRepository));

            ninjectKernel.Bind(typeof(ITRepository<AdminPanelContext, Category>)).To(typeof(TRepository<AdminPanelContext, Category>));
            ninjectKernel.Bind(typeof(ITRepository<AdminPanelContext, DeliveryMethod>)).To(typeof(TRepository<AdminPanelContext, DeliveryMethod>));
            ninjectKernel.Bind(typeof(ITRepository<AdminPanelContext, Size>)).To(typeof(TRepository<AdminPanelContext, Size>));
            //ninjectKernel.Bind<ICategoriesRepository>().To<CategoriesRepository>();
            //ninjectKernel.Bind<ISizesRepository>().To<SizesRepository>();
            //ninjectKernel.Bind<IDeliveryMethodRepository>().To<DeliveryMethodRepository>();
        }
    }
}