using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.WebApi;
using UConstants = Umbraco.Core.Constants;

namespace Merchello.Web.WebApi.Filters
{
    public class EnsureUserPermissionForStoreAttribute : ActionFilterAttribute
    {
        private readonly int? _storeId;
        private readonly string _paramName;
        private readonly bool _isNullable;

        public EnsureUserPermissionForStoreAttribute(int storeId)
        {
            _storeId = storeId;
        }

        public EnsureUserPermissionForStoreAttribute(string paramName) : this(paramName, false) { }

        public EnsureUserPermissionForStoreAttribute(string paramName, bool isNullable)
        {
            if (string.IsNullOrWhiteSpace(paramName)) throw new ArgumentException("Value cannot be null or whitespace.", "paramName");

            _paramName = paramName;
            _isNullable = isNullable;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (UmbracoContext.Current.Security.CurrentUser == null)
            {
                //not logged in
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
            }

            int storeId;
            if (_storeId.HasValue == false)
            {
                var parts = _paramName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                if (actionContext.ActionArguments[parts[0]] == null)
                {
                    if (_isNullable) { base.OnActionExecuting(actionContext); return; }
                    throw new InvalidOperationException("No argument found for the current action with the name: " + _paramName);
                }

                if (parts.Length == 1)
                {
                    storeId = (int)actionContext.ActionArguments[parts[0]];
                }
                else
                {
                    //now we need to see if we can get the property of whatever object it is
                    var pType = actionContext.ActionArguments[parts[0]].GetType();
                    var prop = pType.GetProperty(parts[1]);
                    if (prop == null)
                    {
                        throw new InvalidOperationException("No argument found for the current action with the name: " + _paramName);
                    }

                    storeId = (int)prop.GetValue(actionContext.ActionArguments[parts[0]]);
                }
            }
            else
            {
                storeId = _storeId.Value;
            }

            var startNodeIds = UmbracoContext.Current.Security.CurrentUser.CalculateContentStartNodeIds(ApplicationContext.Current.Services.EntityService);
            var hasAccessToRoot = startNodeIds.Contains(UConstants.System.Root);

            if (!startNodeIds.Contains(storeId) && !hasAccessToRoot)
            {
                throw new HttpResponseException(actionContext.Request.CreateUserNoAccessResponse());
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
