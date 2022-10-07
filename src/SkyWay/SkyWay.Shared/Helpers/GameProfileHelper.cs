﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SkyWay
{
    public static class GameProfileHelper
    {
        #region Fields

        public static GameProfile GameProfile { get; set; }

        #endregion

        #region Methods

        public static bool HasuserLoggedIn()
        {
            return GameProfile is not null && GameProfile.User is not null && !GameProfile.User.UserId.IsNullOrBlank() && !GameProfile.User.UserName.IsNullOrBlank();
        }

        #endregion
    }
}
