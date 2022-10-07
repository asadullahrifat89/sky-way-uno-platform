using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkyWay
{
    public class SessionHelper
    {
        private static Session _session;

        #region Methods

        public static Session GetCachedSession()
        {
            var localValue = CacheHelper.GetCachedValue(Constants.CACHE_SESSION_KEY);

            if (!localValue.IsNullOrBlank())
            {
                var session = JsonConvert.DeserializeObject<Session>(localValue);
                return session;
            }

            return null;
        }

        public static void SetCachedSession(Session session)
        {
            CacheHelper.SetCachedValue(Constants.CACHE_SESSION_KEY, JsonConvert.SerializeObject(session));
        }

        public static bool WillSessionExpireSoon()
        {
            if (_session is null)
                return true;

            if (DateTime.UtcNow.AddMinutes(1) > _session.ExpiresOn)
                return true;

            return false;
        }

        public static bool HasSessionExpired()
        {
            if (_session is null)
                return true;

            if (DateTime.UtcNow > _session.ExpiresOn)
                return true;

            return false;
        }      

        #endregion
    }
}
