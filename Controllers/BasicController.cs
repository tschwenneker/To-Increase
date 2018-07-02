using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace BisConnectivityServices.Controllers
{
    public class BasicController : Controller
    {
        // GET: Basic
        public ActionResult executeMessage(string project, string webservice, string httpaction)
        {
            String result;
            Response.ContentType = @"text/plain";
            ClientConfiguration clientConfig = ClientConfiguration.getClientConfiguration();

            using (AxAccess axAccess = new AxAccess(this.User.Identity.Name, this.Request.Url.AbsolutePath, httpaction, clientConfig))
            {
                try
                {
                    Response.ContentType = @"text/plain";
                    //remove the soap envelope
                    string content = this.GetInputContent();
                    // add the  content back to the correct soap response
                    result = axAccess.executeWebservice(project, webservice, content);
                }
                catch (Exception ex)
                {
                    result = String.Format("<xml>Error: {0}</xml>", ex.Message);
                }
            }


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