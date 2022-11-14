using UnityEngine;

namespace MegafansSDK.Utils {

	public class DeviceInfo {

		public static string DeviceToken {
			get {
                if (!string.IsNullOrEmpty(MegafansPrefs.DeviceTokens)) {
                    return SystemInfo.deviceUniqueIdentifier + "-" + MegafansPrefs.DeviceTokens.Length;
                } else {
                    return SystemInfo.deviceUniqueIdentifier;
                }				
            }
		}

        public static string DeviceType {
            get
            {
#if UNITY_EDITOR
                return "iOS";
#elif UNITY_IOS
                return "iOS";
#elif UNITY_ANDROID
                return "Android";
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
                return "";/*

                var status = OneSignal.GetPermissionSubscriptionState();

                if (!status.permissionStatus.hasPrompted)
                    return "";

                if (status == null || status.subscriptionStatus == null || status.subscriptionStatus.userId == null)

                    return "";
                else
                    return status.subscriptionStatus.userId;*/
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