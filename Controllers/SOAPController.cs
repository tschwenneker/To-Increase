using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace BisConnectivityServices.Controllers
{
    public class SOAPController : Controller
    {
        //static private string soapResponse = @"<?xml version='1.0' encoding='utf-8'?><s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/' xmlns:toin='{0}'  xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'><s:Header/><s:Body>{1}</s:Body></s:Envelope>";
       // static private string soapFaultResponse = @"<?xml version='1.0' encoding='utf-8'?><s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/' xmlns:toin='{0}'  xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>"
       //                                             + "<s:Header/><s:Body>"
       //                                             + "<s:Fault><faultcode>s:Server</faultcode>"
       //                                             + "<faultstring>{1}</faultstring><detail>{2}</detail></s:Fault></s:Body></s:Envelope>";

        public ActionResult executeMessage(string project, string webservice, string httpaction)
        {
            String result;
            Response.ContentType = @"text/xml";
            ClientConfiguration clientConfig = ClientConfiguration.getClientConfiguration();

            using (AxAccess axAccess = new AxAccess(this.User.Identity.Name, this.Request.Url.AbsolutePath, httpaction, clientConfig))
            {
                try
                {
                    Response.ContentType = @"text/xml";
                    string content = this.GetInputContent();
                    result = axAccess.executeWebservice(project, webservice, content);
                }
                catch (Exception ex)
                {
                    result = String.Format("<xml>Error: {0}</xml>", ex.Message);
                }
            }

            
            return Content(result);
        }

     /*   public ActionResult getWsdl(string project, string webservice , string httpaction)
        {
            String result = "";
            Response.ContentType = @"text/xml";


            string url = this.Request.Url.AbsoluteUri;
            url = url.Remove(url.Length -5);
           
            using (AxAccess axAccess = new AxAccess(this.User.Identity.Name, url, httpaction))

            {
                result = axAccess.executeWSDL(project, webservice);
            }

            return Content(result);
        }
        */


        public ActionResult checkConnection(string project, string webservice)
        {
            String result = "";
            Response.ContentType = @"text/plain";
            ClientConfiguration clientConfig = ClientConfiguration.getClientConfiguration();

            using (AxAccess axAccess = new AxAccess(this.User.Identity.Name, this.Request.Url.AbsolutePath, "get", clientConfig))

            {
                result = axAccess.executePing();
                
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
