using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using EFCoreSecurityODataService.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace EFCoreSecurityODataService {
    public class BasicAuthProvider {
        public static bool Authenticate(WebApiApplication app) {
            HttpContext context = app.Context;
            string authHeader = context.Request.Headers["Authorization"];
            IPrincipal user;
            if(TryGetPrincipal(authHeader, out user)) {
                HttpContext.Current.User = user;
                return true;
            }
            return false;
        }
        private static bool TryGetPrincipal(string authHeader, out IPrincipal principal) {
            string userName;
            string password;
            if(TryParseAuthorizationHeader(authHeader, out userName, out password)) {
                return TryAuthenticate(userName, password, out principal);
            }
            principal = null;
            return false;
        }
        private static bool TryParseAuthorizationHeader(string authHeader, out string user, out string password) {
            user = "";
            password = "";
            if(string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic")) {
                return false;
            }
            string base64EncodedCreds = authHeader.Substring(6);
            string[] creds = Encoding.ASCII.GetString(Convert.FromBase64String(base64EncodedCreds)).Split(new char[] { ':' });
            if(creds.Length != 2 || string.IsNullOrEmpty(creds[0]) || string.IsNullOrEmpty(creds[1])) {
                return false;
            }
            user = creds[0];
            password = creds[1];
            return true;
        }
        private static bool TryAuthenticate(string userName, string password, out IPrincipal principal) {
            using(PermissionsProviderContext dbContext = new PermissionsProviderContext()) {
                if(dbContext.Users.Any(p => p.Name == userName && p.Password == password)) {
                    principal = new GenericPrincipal(new GenericIdentity(userName), new string[] { "Users" });
                    return true;
                }
                else {
                    principal = null;
                    return false;
                }
            }
        }
    }
}