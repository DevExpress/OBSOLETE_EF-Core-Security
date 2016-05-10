using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFCoreSecurityODataService {
    public class BasicAuthModule : IHttpModule {
        public void Init(HttpApplication app) {
            app.AuthenticateRequest += new EventHandler(app_AuthenticateRequest);
        }
        private void app_AuthenticateRequest(object sender, EventArgs args) {
            WebApiApplication app = (WebApiApplication)sender;
            //if(!app.Request.IsSecureConnection) {
            //    CreateNotAuthorizedResponse(app, 403, 4,
            //        "SSL is required. Please ensure you use HTTPS in the address.");
            //    app.CompleteRequest();
            //}
            //else 
            if(!app.Request.Headers.AllKeys.Contains("Authorization")) {
                CreateNotAuthorizedResponse(app, 401, 1,
                    "Please provide Authorization headers with your request.");
                app.CompleteRequest();
            }
            else if(!BasicAuthProvider.Authenticate(app)) {
                CreateNotAuthorizedResponse(app, 401, 3, "Logon failed.");
                app.CompleteRequest();
            }
        }
        private static void CreateNotAuthorizedResponse(HttpApplication app, int code, int subCode, string description) {
            HttpResponse response = app.Context.Response;
            response.StatusCode = code;
            response.SubStatusCode = subCode;
            response.StatusDescription = description;
            response.AppendHeader("WWW-Authenticate", "Basic");
        }
        public void Dispose() { }
    }
}