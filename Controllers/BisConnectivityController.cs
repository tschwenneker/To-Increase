using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

using System.Web.Mvc;

namespace BisConnectivityServices.Controllers
{
    [Authorize]
    public class BisConnectivityController : Controller
    {
        String BisMessageHttpActionServiceName = "BisMessageHttpAction";

        [HttpPost]
        public ActionResult executeMessagePost(string project, string message)
        {
            var client = this.GetClient();
            var channel = client.InnerChannel;
            var clientConfig = ClientConfiguration.getClientConfiguration();

            var context = new BisMessageHttpActionServiceReference.CallContext()
            {
                Language = clientConfig.LanguageId,
                Company = clientConfig.CompanyId
            };

            var oauthHeader = OAuthHelper.GetAuthenticationHeader(clientConfig);

            var result = "<xml>Empty</xml>";
            var content = "";

            try
            {
                Response.ContentType = @"text/plain";

                content = this.GetInputContent();

                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                    requestMessage.Headers[OAuthHelper.OAuthHeader] = oauthHeader;
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;

                    result = ((BisMessageHttpActionServiceReference.BisMessageStartFromHttp)channel).executeMessage(new BisMessageHttpActionServiceReference.executeMessage(context, content, message, project)).result;
                }

                return Content(result);
            }
            catch (Exception ex)
            {
                result = String.Format("<xml>Error: {0}</xml>", ex.Message);

                return Content(result);
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        [HttpGet]
        public ActionResult executeMessageGet(string project, string message)
        {
            var client = this.GetClient();
            var channel = client.InnerChannel;
            var clientConfig = ClientConfiguration.getClientConfiguration();

            var context = new BisMessageHttpActionServiceReference.CallContext()
            {
                Language = clientConfig.LanguageId,
                Company = clientConfig.CompanyId
            };

            var oauthHeader = OAuthHelper.GetAuthenticationHeader(clientConfig);

            var result = "<xml>Empty</xml>";

            try
            {
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                    requestMessage.Headers[OAuthHelper.OAuthHeader] = oauthHeader;
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;

                    result = ((BisMessageHttpActionServiceReference.BisMessageStartFromHttp)channel).executeMessageGet(new BisMessageHttpActionServiceReference.executeMessageGet(context, message, project)).result;
                }

                return Content(result);
            }
            catch (Exception ex)
            {
                result = String.Format("<xml>Error: {0}</xml>", ex.Message);

                return Content(result);
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        #region Private Methods
        private BisMessageHttpActionServiceReference.BisMessageStartFromHttpClient GetClient()
        {
            var aosUriString = ClientConfiguration.getClientConfiguration().UriString;

            var serviceUriString = SoapHelper.GetSoapServiceUriString(BisMessageHttpActionServiceName, aosUriString);
            var endpointAddress = new System.ServiceModel.EndpointAddress(serviceUriString);

            var binding = SoapHelper.GetBinding();
            var client = new BisMessageHttpActionServiceReference.BisMessageStartFromHttpClient(binding, endpointAddress);

            return client;
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
        #endregion
    }
}
