namespace Merchello.Web.WebApi
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using Merchello.Core;
    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.Membership;
    using Umbraco.Core.Security;
    using Umbraco.Web;
    using Umbraco.Web.Editors;
    using Umbraco.Web.WebApi;
    using UConstants = Umbraco.Core.Constants;


    /// <summary>
    /// The base Merchello back office API controller.
    /// </summary>
    [JsonCamelCaseFormatter]
    public abstract class MerchelloApiController : UmbracoAuthorizedJsonController
    {
        private IUser _currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloApiController"/> class.
        /// </summary>
        protected MerchelloApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        protected MerchelloApiController(IMerchelloContext merchelloContext) : this(merchelloContext, UmbracoContext.Current)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            
            MerchelloContext = merchelloContext;
            InstanceId = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        protected MerchelloApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext) : base(umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");

            MerchelloContext = merchelloContext;
            InstanceId = Guid.NewGuid();
        }
   
        /// <summary>
        /// Gets the current <see cref="IMerchelloContext"/>
        /// </summary>
        public IMerchelloContext MerchelloContext { get; private set; }

        /// <summary>
        /// Gets the instance id
        /// </summary>
        /// <remarks>
        /// Useful for debugging
        /// </remarks>
        internal Guid InstanceId { get; private set; }

        /// <summary>
        /// Gets the current backend user
        /// </summary>
        public IUser CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    // Get the user who 
                    var userTicket = new System.Web.HttpContextWrapper(System.Web.HttpContext.Current).GetUmbracoAuthTicket();
                    if (userTicket != null)
                    {
                        _currentUser = ApplicationContext.Services.UserService.GetByUsername(userTicket.Name);
                    }
                }
                return _currentUser;
            }
        }

        public void ValidateStoreAccess(int storeId)
        {
            if (CurrentUser == null)
            {
                //not logged in
                throw new HttpResponseException(Request.CreateUserNoAccessResponse());
            }

            var startNodeIds = UmbracoContext.Current.Security.CurrentUser.CalculateContentStartNodeIds(ApplicationContext.Current.Services.EntityService);
            var hasAccessToRoot = startNodeIds.Contains(UConstants.System.Root);

            if (!startNodeIds.Contains(storeId) && !hasAccessToRoot)
            {
                throw new HttpResponseException(Request.CreateUserNoAccessResponse());
            }
        }
    }
}
