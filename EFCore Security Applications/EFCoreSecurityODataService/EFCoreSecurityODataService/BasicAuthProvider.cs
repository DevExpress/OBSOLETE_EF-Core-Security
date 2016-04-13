using EFCoreSecurityODataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace EFCoreSecurityODataService {
    public class BasicAuthProvider {
        public static bool Authenticate(HttpContext context) {
            string authHeader = context.Request.Headers["Authorization"];
            IPrincipal principal;
            if(TryGetPrincipal(authHeader, out principal)) {
                HttpContext.Current.User = principal;
                return true;
            }
            return false;
        }
        private static bool TryGetPrincipal(string authHeader, out IPrincipal principal) {
            string user;
            string password;
            if(TryParseAuthorizationHeader(authHeader, out user, out password)) {
                return TryAuthenticate(user, password, out principal);
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
        private static bool TryAuthenticate(string user, string password, out IPrincipal principal) {
            EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext();
            if(dbContext.Users.Any(p => (p.Name == user) && (p.Password == password))) {
                principal = new GenericPrincipal(new GenericIdentity(user), new string[] { "Users" });
                return true;
            }
            else {
                principal = null;
                return false;
            }
        }
    }
}