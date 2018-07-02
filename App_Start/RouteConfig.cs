using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BisConnectivityServices
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //default the Project and the webservice define the Id 
            //SOAP
            //The Type is defined in the URL SOAP the action is defined in the HttpAction
            //REST
            //For Rest the http type defines which action should be executed
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            /*  routes.MapRoute(
                name: "SOAPPing",
                url: "BisWebservice/SOAP/{httpaction}",
                defaults: new { controller = "SOAP", action = "CheckConnection" },
                constraints: new { httpaction = "ping" }
                );
                */

                routes.MapRoute(
              name: "RESTPing",
              url: "BisWebservice/Ping/",
              defaults: new { controller = "REST", action = "checkConnection" },
              constraints: new { httpMethod = new HttpMethodConstraint("Get") }
              );

            routes.MapRoute(
             name: "SOAPWsdl",
             url: "BisWebservice/SOAP/{httpaction}/{project}/{webservice}/WSDL",
             defaults: new { controller = "SOAP", action = "getWsdl" }
             );
            routes.MapRoute(
               name: "SOAP",
               url: "BisWebservice/SOAP/{httpaction}/{project}/{webservice}/",
               defaults: new { controller = "SOAP", action = "executeMessage" },
               constraints: new {httpMethod = new HttpMethodConstraint("GET", "POST")}
               );

            routes.MapRoute(
              name: "BASIC",
              url: "BisWebservice/BASIC/{httpaction}/{project}/{webservice}/",
              defaults: new { controller = "Basic", action = "executeMessage" },
              constraints: new { httpMethod = new HttpMethodConstraint("GET", "POST", "PUT", "PATCH", "DELETE") }
              );

            //REST URL
           /* routes.MapRoute(
              name: "RESTPing",
              url: "BisWebservice/REST/Ping/",
              defaults: new { controller = "REST", action = "checkConnection" },
              constraints: new { httpMethod = new HttpMethodConstraint("Get") }
              );
              */
            routes.MapRoute(
              name: "RESTOperation",
              url: "BisWebservice/REST/{project}/{webservice}/{*arguments}",
              defaults: new { controller = "REST", action = "executeMessage" },
              constraints: new { httpMethod = new HttpMethodConstraint("GET", "POST", "PUT", "PATCH", "DELETE") }
              );

            //Start the default page
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            //Start the default page
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Setup", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}