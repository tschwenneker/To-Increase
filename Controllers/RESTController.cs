using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BisConnectivityServices.Models; 

namespace BisConnectivityServices.Controllers
{
    public class RESTController : Controller
    {
        //
        // GET: /REST/

        // GET: REST
        public ActionResult executeMessage(string project, string webservice)
        {
            //try
            //{
                string httpaction = this.Request.HttpMethod.ToString();

                String result;
                Response.ContentType = @"text/plain";
                ClientConfiguration clientConfig = ClientConfiguration.getClientConfiguration();

                //this.User.Identity.Name

                using (AxAccess axAccess = new AxAccess(this.User.Identity.Name, this.Request.Url.AbsolutePath, httpaction, clientConfig))
                {
                    try
                    {
                        Response.ContentType = @"text/plain";

                        string content = this.GetInputContent();
                        // add the  content back to the correct soap response
                        result = axAccess.executeWebservice(project, webservice, content);
                    }
                    catch (Exception ex)
                    {
                        result = String.Format("<xml>Error: {0}</xml>", ex.Message);

                        return Content(result);
                    }
                    //result = "SOAP" + "" + axAccess.executewebservice(this, project, webservice, httpaction);
                }
                return Content(result);
            //}
            //catch
            //{
            //    return View();
            //}
        }

        /*  public ActionResult getWsdl(string project, string webservice)
          {
              String result = "";
              Response.ContentType = @"text/plain";
              using (AxAccess axAccess = new AxAccess(this.User.Identity.Name, this.Request.Url.AbsolutePath, "Get"))
              {
                  result = axAccess.executeWSDL(project, webservice);
              }

              return Content(result);
          }*/



        public ActionResult checkConnection(string project, string webservice)
        {
            String result = "";
            Response.ContentType = @"text/plain";
            ClientConfiguration clientConfig = ClientConfiguration.getClientConfiguration();

            using (AxAccess axAccess = new AxAccess(this.User.Identity.Name, this.Request.Url.AbsolutePath, "Get", clientConfig))
            {
                result = axAccess.executePing();
                //result = "SOAP " + axAccess.checkConnection(this);
            }

            AxAccessModel access = new AxAccessModel();
            
            if (result.ToLower().Equals("true"))
                access.Connected = "Application is correct connected to :";
            else
                access.Connected = "Application is not correct connected to :";
            return Content(result);
        }


        private String GetInputContent()
        {
            var content = "";
            var arguments = this.Request.InputStream;

            using (StreamReader reader = new StreamReader(arguments, System.Text.Encoding.Default))
            {
                content = reader.ReadToEnd();
            }

            return content;
        }

    }
}
