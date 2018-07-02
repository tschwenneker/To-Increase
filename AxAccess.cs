using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using BisConnectivityServices.BisWebserviceReferences;


namespace BisConnectivityServices
{
    public sealed class AxAccess :IDisposable
    {
        private String BisActionServiceName = "BisWsWebServiceOperation";
        public IClientChannel channel { get; set; }
        public string oauthHeader { get; set; }

        private string userName;
        private string url;
        private string httpaction;

        private ClientConfiguration clientConfig;

        //todo change this to the correct service references
        private BisWsWebserviceCallClient client;

        public AxAccess(string _userName, string _url, string _httpaction, ClientConfiguration clientConfig)
        {
            // clientConfig needs to be set before
            this.clientConfig = clientConfig;
            this.userName = _userName;
            this.url = _url;
            this.httpaction = _httpaction;

            this.client = this.GetClient();
            this.channel = client.InnerChannel;

            

            // TODO: this seems to do nothing? check
            //var context = new BisWebserviceReferences.CallContext()
            //{
            //    Language = clientConfig.LanguageId,
            //    Company = clientConfig.CompanyId
            //};

            oauthHeader = OAuthHelper.GetAuthenticationHeader(clientConfig);
        }


        private BisWsWebserviceCallClient GetClient()
        {
            var aosUriString = clientConfig.UriString;

            var serviceUriString = SoapHelper.GetSoapServiceUriString(BisActionServiceName, aosUriString);
            var endpointAddress = new System.ServiceModel.EndpointAddress(serviceUriString);

            var binding = SoapHelper.GetBinding();
            var client = new BisWsWebserviceCallClient(binding, endpointAddress);

            return client;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
            client.Close();
        }

        public string executeWebservice(string project, string webservice, string content)
        {
            string result = "" ;
            try
            {

                
                var context = new BisWebserviceReferences.CallContext()
                {//todo  
                    Language = clientConfig.LanguageId,
                    Company = clientConfig.CompanyId
                };


                //handle the soap content to retrieve the data from the content
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                    requestMessage.Headers[OAuthHelper.OAuthHeader] = oauthHeader;
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
                    // add the New reference and remove the old service references to httpmessage
                    //result = ((BisMessageHttpActionServiceReference.BisMessageStartFromHttp)channel).executeMessage(new BisMessageHttpActionServiceReference.executeMessage(context, project, webservice,  content)).result;
                    
                    result = ((BisWebserviceReferences.BisWsWebserviceCall)channel).executeOperation(
                        new BisWebserviceReferences.executeOperation(
                            context, this.convertHttpActionToEnum(httpaction), project, url, userName, webservice, content)
                        ).result;
                }

                // add the  content back to the correct soap response
                return result;
            }
            catch (Exception ex)
            {
                result = String.Format("<xml>Error: {0}</xml>", ex.Message);

                return result;
            }

           // return result;
        }


        public string executePing()
        {
           string message = "";

           var context = new BisWebserviceReferences.CallContext()
           {//todo  
                Language = clientConfig.LanguageId,
               Company = clientConfig.CompanyId
           };


            //handle the soap content to retrieve the data from the content
            using (OperationContextScope operationContextScope = new OperationContextScope(channel))
            {
                HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                requestMessage.Headers[OAuthHelper.OAuthHeader] = oauthHeader;
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
                // add the New reference and remove the old service references to httpmessage
                //result = ((BisMessageHttpActionServiceReference.BisMessageStartFromHttp)channel).executeMessage(new BisMessageHttpActionServiceReference.executeMessage(context, project, webservice,  content)).result;
                
                message = ((BisWebserviceReferences.BisWsWebserviceCall)channel).ping(
                    new BisWebserviceReferences.ping(context, BisConUrlHttpAction.Post, "dummy", "dummy")).result;
            }

            return message;
        }


     /*   public string executeWSDL(string project, string webservice)
        {
            string result = "";

            var context = new BisWebserviceReferences.CallContext()
            {//todo  
                Language = clientConfig.LanguageId,
                Company = clientConfig.CompanyId
            };


            //handle the soap content to retrieve the data from the content
            using (OperationContextScope operationContextScope = new OperationContextScope(channel))
            {
                HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                requestMessage.Headers[OAuthHelper.OAuthHeader] = oauthHeader;
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
                // add the New reference and remove the old service references to httpmessage
                //result = ((BisMessageHttpActionServiceReference.BisMessageStartFromHttp)channel).executeMessage(new BisMessageHttpActionServiceReference.executeMessage(context, project, webservice,  content)).result;

                result = ((BisWebserviceReferences.BisWsWebserviceCall)channel).getWsdl(new BisWebserviceReferences.getWsdl(context, this.convertHttpActionToEnum(httpaction), project, webservice)).result;
            }

            return result;
        }
        */
        private BisWebserviceReferences.BisConUrlHttpAction convertHttpActionToEnum(string httpaction)
        {
            BisWebserviceReferences.BisConUrlHttpAction action = BisConUrlHttpAction.Get;
            switch (httpaction.ToLower())
            {
                case "get":
                    action = BisConUrlHttpAction.Get;
                    break;
                case "delete":
                    action = BisConUrlHttpAction.Delete;
                    break;
                case "post":
                    action = BisConUrlHttpAction.Post;
                    break;
                case "patch":
                    action = BisConUrlHttpAction.Patch;
                    break;
                case "Put":
                    action = BisConUrlHttpAction.Put;
                    break;
            }

            return action;
        }
    }
    

}