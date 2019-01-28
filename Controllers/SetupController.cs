using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace BisConnectivityServices.Controllers
{

    public class SetupController : Controller
    {
        // GET: Setup
        public ActionResult Index()
        {
            var config = ClientConfiguration.getClientConfiguration();
            return View(config);
        }


        // POST: Setup/Create
        [HttpPost]
        public ActionResult Index(ClientConfiguration clientConfig)
        {
            clientConfig.Save();

            return Json(new
            {
                isSuccess = true,
                message = "Settings are saved"
            });
        }

        public ActionResult TestConfiguration(ClientConfiguration clientConfig)
        {
            string message = "";
            string invalidFormField = "";
            //ClientConfiguration clientConfig = ClientConfiguration.getClientConfiguration();


            var validationErrors = clientConfig.getValidationErrors();
            if (validationErrors.Any())
            {
                string json = JsonConvert.SerializeObject(CreateJsonFromValidationErrors(validationErrors));
                return Content(json, "application/json");
            }
            
            try
            {
                return Json(this.makePingRequest(clientConfig));

            }

            catch (System.ServiceModel.EndpointNotFoundException e)
            {
                message = e.Message;
                invalidFormField = "UriString";
            }

            catch (System.ServiceModel.FaultException e)
            {
                switch (e.Message)
                {
                    case "Internal Server Error":
                        message = "Unkown error. Please check if the value for language is correct.";
                        break;
                    case "Forbidden":
                        message = "The system returned \"Forbidden\". Your input for Active Directory source uri might be incorrect.";
                        if (clientConfig.ActiveDirectoryResource.EndsWith("/"))
                        {
                            message += " Try to remove the ending slash of your Active Directory source uri.";
                        }
                        break;
                    default:
                        message = e.Message;
                        break;
                }
            }

            catch (AggregateException e) when (e.InnerException is Microsoft.IdentityModel.Clients.ActiveDirectory.AdalServiceException)
            {
                var innerEx = (Microsoft.IdentityModel.Clients.ActiveDirectory.AdalServiceException)e.InnerException;

                switch (innerEx.ErrorCode)
                {
                    case "invalid_resource":
                        invalidFormField = "ActiveDirectoryResource";
                        break;
                    case "failed_to_refresh_token":
                        invalidFormField = "ActiveDirectoryTenant";
                        break;
                    case "unauthorized_client":
                        invalidFormField = "ActiveDirectoryClientAppId";
                        break;
                    default:
                        break;
                }

                message = e.InnerException.Message;
            }

            catch (AggregateException e) when (e.InnerException is ArgumentNullException 
                && (e.InnerException as ArgumentNullException).ParamName == "resource")
            {
                invalidFormField = "ActiveDirectoryResource";
                message = e.Message;
            }


            catch (AggregateException e)
                when (e.InnerException is Microsoft.IdentityModel.Clients.ActiveDirectory.AdalException
                    && ((Microsoft.IdentityModel.Clients.ActiveDirectory.AdalException)e.InnerException).ErrorCode == "unknown_user_type")
            {
                message = "The username or password is incorrect.";
            }

            catch (AggregateException e)
            {
                message = e.InnerException.Message;
            }

            catch (ArgumentNullException e) when (e.ParamName == "clientId")
            {
                invalidFormField = "ActiveDirectoryClientAppId";
                message = e.Message;
            }

            catch (ArgumentNullException e) when (e.ParamName == "authority")
            {
                invalidFormField = "ActiveDirectoryTenant";
                message = e.Message;
            }


            catch (Exception e)
            {
                if (clientConfig.UserName == "" || clientConfig.Password == "")
                {
                    message = "Please provide a valid user-password combination. " + e.Message;
                } else
                {
                    message = e.Message;
                }
                
            }

            // this.Response.StatusCode = 422;
            // this.Response.StatusDescription = "Unprocessable Entity";



            return Json(new
            {
                isSuccess = false,
                message = message,
                invalidFormField = invalidFormField
            });
        }
    

        private object makePingRequest(ClientConfiguration clientConfig)
        {
            using (AxAccess axAccess = new AxAccess(this.User.Identity.Name, this.Request.Url.AbsolutePath, "Get", clientConfig))
            {
                string result = axAccess.executePing();
                //result = "SOAP " + axAccess.checkConnection(this);

               return (new { isSuccess = true });
            }
        }

        private List<ValidationResult> getValidationErrors(ClientConfiguration config)
        {
            var validationContext = new ValidationContext(config, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(config, validationContext, validationResults);

            return validationResults;
        }

        private object CreateJsonFromValidationErrors(List<ValidationResult> errors)
        {
            Dictionary<string, string> invalidFormFields = new Dictionary<string, string>();
            foreach (var error in errors)
            {
                invalidFormFields.Add(error.MemberNames.First(), error.ErrorMessage);
            }


            return (new
            {
                isSuccess = false,
                invalidFormFields = invalidFormFields,
                message = ""
            });
        }

    }
}
