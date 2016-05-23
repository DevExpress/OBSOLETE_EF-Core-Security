using DevExpress.EntityFramework.SecurityDataStore;
using EFCoreSecurityODataService.DataModel;
using System.Web.Http;
using System;
using System.Linq;
using DevExpress.EntityFramework.SecurityDataStore.Authorization;

namespace EFCoreSecurityODataService {
    public class WebApiApplication : System.Web.HttpApplication, ISecurityApplication
    {
        public ISecurityUser CurrentUser { get; set; }
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Updater.UpdateDatabase();
        }
    }
}
