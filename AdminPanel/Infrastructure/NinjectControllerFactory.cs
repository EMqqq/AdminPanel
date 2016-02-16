using System;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using AdminPanel.Abstract;
using AdminPanel.DataAccessLayer;

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
            ninjectKernel.Bind(typeof(ITRepository<>)).To(typeof(TRepository<>));
            ninjectKernel.Bind<ICategoriesRepository>().To<CategoriesRepository>();
            ninjectKernel.Bind<ISizesRepository>().To<SizesRepository>();
            ninjectKernel.Bind<IDeliveryMethodRepository>().To<DeliveryMethodRepository>();
        }
    }
}