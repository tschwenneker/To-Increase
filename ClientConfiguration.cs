using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Web.Configuration;
using System.Configuration;
using System.Text;

namespace BisConnectivityServices
{
    public partial class ClientConfiguration
    {
        private static ClientConfiguration config;
        private const string PASSWORD_SALT = "v2zrd2q0$253((!xcbcweqoj";

        /// <summary>
        /// Factory method
        /// </summary>
        /// <returns></returns>
        public static ClientConfiguration getClientConfiguration()
        {
            if (config == null)
            {
                config = CreateFromSystemConfigurationManager();
            }
            return config;
        }

        private static ClientConfiguration CreateFromSystemConfigurationManager()
        {
            var result = new ClientConfiguration();
            string password = System.Configuration.ConfigurationManager.AppSettings["Password"];
            string decryptedPassword = password == null ? "" : Decrypt(password);

            result.UriString = System.Configuration.ConfigurationManager.AppSettings["UriString"];
            result.UserName = System.Configuration.ConfigurationManager.AppSettings["UserName"];
            result.Password = decryptedPassword;
            result.CompanyId = System.Configuration.ConfigurationManager.AppSettings["CompanyId"];
            result.LanguageId = System.Configuration.ConfigurationManager.AppSettings["LanguageId"];
            result.ActiveDirectoryResource = System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryResource"];
            result.ActiveDirectoryTenant = System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryTenant"];
            result.ActiveDirectoryClientAppId = System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryClientAppId"];
            result.ActiveDirectoryClientAppSecret = System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryClientAppSecret"];
            result.IsErrorLoggingEnabled = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["IsErrorLoggingEnabled"]);

            return result;
        }

        public void Save()
        {
            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            var section = (AppSettingsSection)configuration.GetSection("appSettings");
            section.Settings["UriString"].Value = UriString;
            section.Settings["UserName"].Value = UserName;
            section.Settings["Password"].Value = Password == null ? "" : Encrypt(Password);
            section.Settings["CompanyId"].Value = CompanyId;
            section.Settings["LanguageId"].Value = LanguageId;
            section.Settings["ActiveDirectoryResource"].Value = ActiveDirectoryResource;
            section.Settings["ActiveDirectoryTenant"].Value = ActiveDirectoryTenant;
            section.Settings["ActiveDirectoryClientAppId"].Value = ActiveDirectoryClientAppId;
            section.Settings["isErrorLoggingEnabled"].Value = IsErrorLoggingEnabled ? Boolean.TrueString : Boolean.FalseString;

            configuration.Save();
        }


        //public static ClientConfiguration CloudAX = new ClientConfiguration()
        //{
        //    UriString                   = System.Configuration.ConfigurationManager.AppSettings["UriString"],
        //    UserName                    = System.Configuration.ConfigurationManager.AppSettings["UserName"],
        //    Password                    = System.Configuration.ConfigurationManager.AppSettings["Password"],
        //    CompanyId                   = System.Configuration.ConfigurationManager.AppSettings["CompanyId"],
        //    LanguageId                  = System.Configuration.ConfigurationManager.AppSettings["LanguageId"],
        //    ActiveDirectoryResource     = System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryResource"],
        //    ActiveDirectoryTenant       = System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryTenant"],
        //    ActiveDirectoryClientAppId  = System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryClientAppId"],
        //    ActiveDirectoryClientAppSecret = System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryClientAppSecret"]
        //};

        [Required]
        [DisplayName("URI")]
        public string UriString { get; set; }

        [Required]
        [DisplayName("User name")]
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required]
        [DisplayName("Company")]
        public String CompanyId { get; set; }

        [Required]
        [DisplayName("Language")]
        public String LanguageId { get; set; }

        [Required]
        [DisplayName("Active Directory source uri")]
        public string ActiveDirectoryResource { get; set; }

        [Required]
        [DisplayName("Active Directory tenant")]
        public String ActiveDirectoryTenant { get; set; }

        [Required]
        [DisplayName("Active Directory client app")]
        public String ActiveDirectoryClientAppId { get; set; }

        // it seems it doesn't need to have any value
        [DisplayName("Active Directory client secret")]
        public string ActiveDirectoryClientAppSecret { get; set; }

        [DisplayName("Enable error logging")]
        public bool IsErrorLoggingEnabled { get; set; }



        #region Encryption and decryption methods
        private static string Decrypt(string encrypted)
        {
            if (encrypted == "")
            {
                return "";
            }

            byte[] data = Convert.FromBase64String(encrypted);
            //byte[] data = System.Text.Encoding.ASCII.GetBytes(encrypted);
            byte[] decryptedData = ProtectedData.Unprotect(data, GetSaltBytes(), DataProtectionScope.CurrentUser);
            //return System.Text.Encoding.ASCII.GetString(decryptedData);
            return Encoding.Unicode.GetString(decryptedData);
        }

        private static string Encrypt(string plain)
        {
            if (plain == "")
            {
                return "";
            }

            byte[] data = Encoding.Unicode.GetBytes(plain);
            byte[] encryptedData = ProtectedData.Protect(data, GetSaltBytes(), DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedData);
            //return System.Text.Encoding.ASCII.GetString(encryptedData);
        }

        private static byte[] GetSaltBytes()
        {
            return Encoding.Unicode.GetBytes(PASSWORD_SALT);
        }

        #endregion

        public List<ValidationResult> getValidationErrors()
        {
            // adapted from https://johan.driessen.se/posts/testing-dataannotation-based-validation-in-asp.net-mvc
            var validationContext = new ValidationContext(this, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(this, validationContext, validationResults);
            return validationResults;
        }
    }
}