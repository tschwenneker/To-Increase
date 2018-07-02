using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BisConnectivityServices.Models
{
    public class AxAccessModel
    {
       // public string logonUser { get; set; }
        public string tentant { get; set; }
        public string company { get; set; }
        public string uri { get; set;}
        public String language { get; set; }
        public string Connected { get; set; }

        String mtentant = System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryTenant"];
        String mCompany = System.Configuration.ConfigurationManager.AppSettings["CompanyId"];
        String mLanguage = System.Configuration.ConfigurationManager.AppSettings["LanguageId"];
        String mUri = System.Configuration.ConfigurationManager.AppSettings["UriString"];



        public AxAccessModel()
        {
            uri = mUri;
            tentant = mtentant;
            company = mCompany;
            language = mLanguage;

            
        }
    }
}