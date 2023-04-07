using UnityEngine;
using OneSignalSDK;


namespace MegafansSDK.Utils
{

    public class DeviceInfo
    {

        public static string DeviceToken
        {
            get
            {
                if (!string.IsNullOrEmpty(MegafansPrefs.DeviceTokens))
                {
                    return SystemInfo.deviceUniqueIdentifier + "-" + MegafansPrefs.DeviceTokens.Length;
                }
                else
                {
                    return SystemInfo.deviceUniqueIdentifier;
                }
            }
        }

        public static string DeviceType
        {
            get
            {
#if UNITY_EDITOR
                return "iOS";
#elif UNITY_IOS
                return "iOS";
#elif UNITY_ANDROID
                return "Android";
#else
                return "Web";
#endif
            }
        }

        public static string Platform
        {
            get
            {

#if UNITY_IOS
                return "iOS";
#else
                return "Android";
#endif
            }
        }

        public static string NotificationID

        {

            get
            {


                var status = OneSignal.Default.NotificationPermission;

                if (status == NotificationPermission.NotDetermined)
                    return "";
                else
                    return "";

            }

        }

        public static string AdvertisingID

        {

            get
            {
                return Megafans.Instance.AdvertisingID;
            }

        }

    }

}