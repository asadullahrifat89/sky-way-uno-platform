using System;
using System.Collections.Generic;
using System.Text;

namespace SkyWay
{
    public class CookieHelper
    {
        #region Methods

        public static bool IsCookieAccepted()
        {
            return CacheHelper.GetCachedValue(Constants.COOKIE_KEY) is string cookie && cookie == Constants.COOKIE_ACCEPTED_KEY;
        }

        public static void SetCookieAccepted()
        {
            CacheHelper.SetCachedValue(Constants.COOKIE_KEY, Constants.COOKIE_ACCEPTED_KEY);
        }

        #endregion
    }
}
