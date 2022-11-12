using System;
using System.Web;
using System.Web.Routing;

#if !NET7_0
using System.Web.Services.Protocols;
#endif

namespace eShopWebForms
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(System.Web.Routing.RouteCollection routes)
        {
            routes.MapPageRoute(
                "",
                "Default",
                "~/Default.aspx"
                );
            routes.MapPageRoute(
                "ProductsByPageRoute",
                "Default/index/{index}/size/{size}",
                "~/Default.aspx"
                );
            routes.MapPageRoute(
                "CreateProductRoute",
                "Catalog/Create",
                "~/Catalog/Create.aspx"
                );
            routes.MapPageRoute(
                "EditProductRoute",
                "Catalog/Edit/{id}",
                "~/Catalog/Edit.aspx"
                );
            routes.MapPageRoute(
                "ProductDetailsRoute",
                "Catalog/Details/{id}",
                "~/Catalog/Details.aspx"
                );
            routes.MapPageRoute(
                "DeleteProductRoute",
                "Catalog/Delete/{id}",
                "~/Catalog/Delete.aspx"
                );
        }
    }

#if !NET7_0
    public class ServiceRouteHandler : IRouteHandler
    {
        private readonly string _virtualPath;
        private readonly WebServiceHandlerFactory _handlerFactory = new WebServiceHandlerFactory();

        public ServiceRouteHandler(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");
            if (!virtualPath.StartsWith("~/"))
                throw new ArgumentException("Virtual path must start with ~/", "virtualPath");
            _virtualPath = virtualPath;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            // Note: can't pass requestContext.HttpContext as the first parameter because that's
            // type HttpContextBase, while GetHandler wants HttpContext.
            return _handlerFactory.GetHandler(HttpContext.Current, requestContext.HttpContext.Request.HttpMethod, _virtualPath, requestContext.HttpContext.Server.MapPath(_virtualPath));
        }
    }
#endif
}
