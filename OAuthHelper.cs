using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace BisConnectivityServices
{
    public class OAuthHelper
    {
        /// <summary>
        /// The header to use for OAuth.
        /// </summary>
        public const string OAuthHeader = "Authorization";

        /// <summary>
        /// Retrieves an authentication header from the service.
        /// </summary>
        /// <returns>The authentication header for the Web API call.</returns>
        public static string GetAuthenticationHeader(ClientConfiguration clientConfig, bool useWebAppAuthentication = false)
        {
            string aadTenant = clientConfig.ActiveDirectoryTenant;
            string aadClientAppId = clientConfig.ActiveDirectoryClientAppId;
            string aadResource = clientConfig.ActiveDirectoryResource;

            AuthenticationContext authenticationContext = new AuthenticationContext(aadTenant);
            AuthenticationResult authenticationResult;

            if (useWebAppAuthentication)
            {
                string aadClientAppSecret = clientConfig.ActiveDirectoryClientAppSecret;
                var creadential = new ClientCredential(aadClientAppId, aadClientAppSecret);
                authenticationResult = authenticationContext.AcquireTokenAsync(aadResource, creadential).Result;

            }
            else
            {
                // OAuth through username and password.
                string username = clientConfig.UserName;
                string password = clientConfig.Password;

                // Get token object
                var userCredential = new UserPasswordCredential(username, password);
                authenticationResult = authenticationContext.AcquireTokenAsync(aadResource, aadClientAppId, userCredential).Result;
                //authenticationResult = authenticationContext.AcquireTokenAsync(aadResource, aadClientAppId, new System.Uri(aadTenant), new PlatformParameters(PromptBehavior.RefreshSession, false)).Result;
            }

            // Create and get JWT token
            return authenticationResult.CreateAuthorizationHeader();
        }
    }
}