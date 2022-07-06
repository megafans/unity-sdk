using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
namespace Megafan.NativeWrapper
{
    class MegafanNativeWrapper
    {
#if UNITY_ANDROID
        private const string m_NativeAndroidClassName = "nativeplugin.sbs.com.nativeplagin.NativeWrapper";

        public static void StaticCall(string methodName, object[] _Params = null, string androidJavaClass = "")
        {
            try
            {
                using (AndroidJavaClass androidClass = new AndroidJavaClass(string.IsNullOrEmpty(androidJavaClass) ? m_NativeAndroidClassName : androidJavaClass))
                {
                    if (null != androidClass)
                    {
                        if (_Params == null)
                            androidClass.CallStatic(methodName);
                        else
                            androidClass.CallStatic(methodName, _Params);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.Log(string.Format("{0}.{1} Exception:{2}", androidJavaClass, methodName, ex.ToString()));
            }
        }
#elif UNITY_IOS
        [DllImport("__Internal")]
        private static extern void _InitIntercom(string _AppId, string _AppKey);
        [DllImport("__Internal")]
        private static extern void _registerUserWithUserId(string userID, string gameId, string gameName);
        [DllImport("__Internal")]
        private static extern void _updateUsername(string username);
        [DllImport("__Internal")]
        private static extern void _showIntercom();
        [DllImport("__Internal")]
        private static extern void _hideIntercom();
        [DllImport("__Internal")]
        private static extern void _showIntercomIfUnreadMessages();
        [DllImport("__Internal")]
        private static extern void _didFinishTournamentWithScore(int tournamentId, float score, string gameId, string gameName);
        [DllImport("__Internal")]
        private static extern void _logOut();
        [DllImport("__Internal")]
        public static extern void _openSettings();
#endif

        public static void RegisterUserWithUserId(string userID, string gameId, string gameName)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            StaticCall("RegisterIntercomUserWithId", new object[] { userID, gameId, gameName });
#elif UNITY_IOS && !UNITY_EDITOR
            _registerUserWithUserId(userID, gameId, gameName);
#endif
        }

        public static void UpdateUsernameToIntercom(string username)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            StaticCall("UpdateUsernameToIntercom",
                new object[] { username });
#elif UNITY_IOS && !UNITY_EDITOR
            _updateUsername(username);
#endif
        }

        public static void ShowIntercom()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            StaticCall("ShowIntercom");
#elif UNITY_IOS && !UNITY_EDITOR
            _showIntercom();
#endif
        }

        public static void HideIntercom()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            StaticCall("HideIntercom");
#elif UNITY_IOS && !UNITY_EDITOR
            _hideIntercom();
#endif
        }

        public static void LogTournamentToIntercom(int tournamentId, float score, string gameId, string gameName)
        {
#if UNITY_ANDROID
            StaticCall("LogTournamentToIntercom", new object[] { tournamentId, score, gameId, gameName });
#elif UNITY_IOS
            _didFinishTournamentWithScore(tournamentId, score, gameId, gameName);
#endif
        }

        public static void LogoutFromIntercom()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            StaticCall("LogOut");
#elif UNITY_IOS && !UNITY_EDITOR
            _logOut();
#endif
        }

        public static void ShowIntercomIfUnreadMessages()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            StaticCall("ShowIntercomIfUnreadMessages");
#elif UNITY_IOS && !UNITY_EDITOR
            _showIntercomIfUnreadMessages();
#endif
        }

        public static void InitIntercom(string _AppId, string _AppKey)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            StaticCall("InitIntercom", new object[] { _AppId, _AppKey });
#elif UNITY_IOS && !UNITY_EDITOR
            _InitIntercom(_AppId, _AppKey);
#endif
        }

        public static void OpenSettings()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            string packageName = currentActivityObject.Call<string>("getPackageName");

            using (var uriClass = new AndroidJavaClass("android.net.Uri"))
            using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
            using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
            {
                intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                currentActivityObject.Call("startActivity", intentObject);
            }
        }
#elif UNITY_IOS && !UNITY_EDITOR
            _openSettings();
#endif
        }
    }
}