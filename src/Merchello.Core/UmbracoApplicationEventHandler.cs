namespace Merchello.Core
{
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Services;
    using Umbraco.Web;

    /// <summary>
    /// Handles the Umbraco Application "Starting" and "Started" event and initiates the Merchello startup
    /// </summary>
    public class UmbracoApplicationEventHandler : ApplicationEventHandler
    {
        /// <summary>
        /// The Umbraco Application Starting event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            StoreSettingService.Saved += StoreSettingServiceSaved;

            DomainService.Saved += DomainService_Saved;
            DomainService.Deleted += DomainService_Deleted;

            StoreService.Created += StoreService_Created;
            StoreService.Saved += StoreService_Saved;
            StoreService.Deleted += StoreService_Deleted;
        }

        private void StoreService_Created(IStoreService sender, Events.NewEventArgs<Models.IStore> e)
        {
            UmbracoContext.Current.HttpContext.Cache[Constants.Cache.StoresList] =
                sender.GetAll();
        }

        private void StoreService_Saved(IStoreService sender, Umbraco.Core.Events.SaveEventArgs<Models.IStore> e)
        {
            UmbracoContext.Current.HttpContext.Cache[Constants.Cache.StoresList] =
                sender.GetAll();
        }

        private void StoreService_Deleted(IStoreService sender, Umbraco.Core.Events.DeleteEventArgs<Models.IStore> e)
        {
            UmbracoContext.Current.HttpContext.Cache[Constants.Cache.StoresList] =
                sender.GetAll();
        }

        private void DomainService_Saved(IDomainService sender, Umbraco.Core.Events.SaveEventArgs<Umbraco.Core.Models.IDomain> e)
        {
            UmbracoContext.Current.HttpContext.Cache[Constants.Cache.DomainsList] =
                sender.GetAll(true);
        }

        private void DomainService_Deleted(IDomainService sender, Umbraco.Core.Events.DeleteEventArgs<Umbraco.Core.Models.IDomain> e)
        {
            UmbracoContext.Current.HttpContext.Cache[Constants.Cache.DomainsList] =
                sender.GetAll(true);
        }

        /// <summary>
        /// Resets the store currency.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The save event args.
        /// </param>
        private void StoreSettingServiceSaved(
            IStoreSettingService sender,
            Umbraco.Core.Events.SaveEventArgs<Models.IStoreSetting> e)
        {
            CurrencyContext.Current.ResetCurrency();
        }
    }
}