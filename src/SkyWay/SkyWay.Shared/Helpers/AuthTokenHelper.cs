using System;
using System.Collections.Generic;
using System.Text;

namespace SkyWay.Helpers
{
    public class AuthTokenHelper
    {
        #region Properties

        public static AuthToken AuthToken { get; set; }

        #endregion

        #region Methods

        public static bool WillAuthTokenExpireSoon()
        {
            if (DateTime.UtcNow.AddSeconds(20) > AuthToken.ExpiresOn)
                return true;

            return false;
        }

        public static void SetAuthToken(AuthToken authToken)
        {
            AuthToken = authToken;
        }

        #endregion
    }
}
