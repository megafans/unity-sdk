using UnityEngine;
using System.Collections.Generic;

namespace MegafansSDK.Utils {

	public class MegafansPrefs {

		private const string k_prefix = "Megafans_";
        private const string k_userId = k_prefix + "UserId";
        private const string k_accessToken = k_prefix + "AccessToken";
        private const string k_refreshToken = k_prefix + "RefreshToken";
        private const string k_emailAddress = k_prefix + "Email";
        private const string k_facebookID = k_prefix + "FacebookID";
        private const string k_username = k_prefix + "Username";
		private const string k_phoneNumber = k_prefix + "PhoneNumber";
		private const string k_profilePicUrl = k_prefix + "ProfilePicUrl";
		private const string k_profilePic = k_prefix + "ProfilePic";
        private const string k_registrationComplete = k_prefix + "RegistrationComplete";
        private const string k_userStatus = k_prefix + "UserStatus";
        private const string k_SMSAvailable = k_prefix + "SMSAvailable";
        private const string k_currentTokenBalance = k_prefix + "CurrentTokenBalance";

        private const string k_deviceTokens = k_prefix + "UsedDeviceTokens";

        public static int UserId
        {
            get
            {
                return PlayerPrefs.GetInt (k_userId, 0);
            }

            set
            {
                PlayerPrefs.SetInt (k_userId, value);
            }
        }

        public static bool IsRegisteredMegaFansUser
        {
            get
            {
                return (MegafansPrefs.UserStatus == 1 || MegafansPrefs.UserStatus == 5 || MegafansPrefs.UserStatus == 11 || MegafansPrefs.UserStatus == 13);
            }
        }

        public static string AccessToken
        {
            get
            {
                return PlayerPrefs.GetString (k_accessToken, "");
            }

            set
            {
                PlayerPrefs.SetString (k_accessToken, value);
            }
        }

        public static string RefreshToken
        {
            get
            {
                return PlayerPrefs.GetString (k_refreshToken, "");
            }

            set
            {
                PlayerPrefs.SetString (k_refreshToken, value);
            }
        }

        public static string Email {
            get {
                return PlayerPrefs.GetString (k_emailAddress, "");
            }

            set {
                PlayerPrefs.SetString (k_emailAddress, value);
            }
        }

        public static string FacebookID
        {
            get
            {
                return PlayerPrefs.GetString(k_facebookID, "");
            }

            set
            {
                PlayerPrefs.SetString(k_facebookID, value);
            }
        }

        public static string Username {
			get {
				return PlayerPrefs.GetString (k_username, "");
			}

			set {
				PlayerPrefs.SetString (k_username, value);
			}
		}

		public static string PhoneNumber {
			get {
				return PlayerPrefs.GetString (k_phoneNumber, "");
			}

			set {
				PlayerPrefs.SetString (k_phoneNumber, value);
			}
		}

		public static string ProfilePicUrl {
			get {
				return PlayerPrefs.GetString (k_profilePicUrl, "");
			}

			set {
				PlayerPrefs.SetString (k_profilePicUrl, value);
			}
		}

		public static string ProfilePic {
			get {
				return PlayerPrefs.GetString (k_profilePic, "");
			}

			set {
				PlayerPrefs.SetString (k_profilePic, value);
			}
		}

        public static int UserStatus
        {
            get
            {
                return PlayerPrefs.GetInt(k_userStatus, 0);
            }

            set
            {
                PlayerPrefs.SetInt(k_userStatus, value);
            }
        }

        public static float CurrentTokenBalance
        {
            get
            {
                return PlayerPrefs.GetFloat(k_currentTokenBalance, 0.0f);
            }

            set
            {
                PlayerPrefs.SetFloat(k_currentTokenBalance, value);
            }
        }

        public static bool SMSAvailable
        {
            get
            {
                int value = PlayerPrefs.GetInt(k_SMSAvailable, 0);
                if (value == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                if (value) {
                    PlayerPrefs.SetInt(k_SMSAvailable, 1);
                } else {
                    PlayerPrefs.SetInt(k_SMSAvailable, 0);
                }
            }
        }

        public static string DeviceTokens
        {
            get
            {
                return PlayerPrefs.GetString(k_deviceTokens);
            }

            set
            {
                string previousDeviceTokens = PlayerPrefs.GetString(k_deviceTokens);
                if (!string.IsNullOrEmpty(previousDeviceTokens))
                {
                    string[] splitTokens = previousDeviceTokens.Split(',');
                    previousDeviceTokens += ("," + value + "-" + splitTokens.Length);
                    PlayerPrefs.SetString(k_deviceTokens, previousDeviceTokens);
                }
                else
                {
                    PlayerPrefs.SetString(k_deviceTokens, value);
                }

                Debug.Log(PlayerPrefs.GetString(k_deviceTokens));
            }
        }

        public static void ClearPrefs() {
            PlayerPrefs.DeleteKey (k_accessToken);
            PlayerPrefs.DeleteKey (k_refreshToken);
            PlayerPrefs.DeleteKey (k_username);
			PlayerPrefs.DeleteKey (k_phoneNumber);
			PlayerPrefs.DeleteKey (k_profilePicUrl);
			PlayerPrefs.DeleteKey (k_profilePic);
            PlayerPrefs.DeleteKey(k_emailAddress);
            PlayerPrefs.DeleteKey(k_userStatus);
            PlayerPrefs.DeleteKey(k_currentTokenBalance);
            PlayerPrefs.DeleteKey(k_facebookID);
        }
    }
}