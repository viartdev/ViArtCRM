using Microsoft.AspNetCore.Http;
using System;
using ViArtCRM.Models;

namespace ViArtCRM.HelperTools {
    public static class BrowserDataHelper {
        private const string userSessionStringVar = "userSessionString";
        private const string userIDVar = "userIDVar";
        private const string userLoginVar = "userLoginVar";

        public static UserSession GetBrowserUserSession(IRequestCookieCollection cookies) {
            if (cookies[userSessionStringVar] != null && cookies[userIDVar] != null && cookies[userLoginVar] != null) {
                UserSession userSession = new UserSession();
                userSession.UserID = Convert.ToInt32(cookies[userIDVar]);
                userSession.USession = cookies[userSessionStringVar].ToString();
                userSession.Login = cookies[userLoginVar].ToString();
                return userSession;
            }
            else
                return null;
        }
        public static void WriteUserDataToCookies(UserSession userSession, IResponseCookies cookies) {
            cookies.Append(userSessionStringVar, userSession.USession);
            cookies.Append(userIDVar, userSession.UserID.ToString());
            cookies.Append(userLoginVar, userSession.Login);            
        }
        public static void ClearUserDataCookies(IRequestCookieCollection requestCookieCollection,IResponseCookies cookies) {
            var myCookies = requestCookieCollection.Keys;
            foreach (string cookie in myCookies) {
                cookies.Delete(cookie);
            }
            //UserSession userSession = new UserSession() {
            //    Login = String.Empty,
            //    UserID = -1,
            //    USession = String.Empty
            //};
            //WriteUserDataToCookies(userSession, cookies);
        }
    }   
}
