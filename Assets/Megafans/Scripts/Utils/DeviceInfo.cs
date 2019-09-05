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

	}

}