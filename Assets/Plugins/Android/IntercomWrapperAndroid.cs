using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
//using net.sf.jni4net;

namespace MegaFans.Unity.Android
{
    ///// <summary>
    ///// Simple C# wrapper to externally link Unity SDK calls to Native Android SDKs.
    ///// Any new Android methods that must have a Unity implementation should be defined, and called here.
    ///// </summary>
    public class IntercomWrapperAndroid
    {
        /// <summary>
        /// Register User with user Id action
        /// </summary>
        /// /// <param name="userId">Current user id</param>
        /// <param name="email">Current user email address</param>
        /// <param name="gameId">Current game id</param>
        /// <param name="gameName">Current game name</param>
        public static void RegisterUserWithUserId(string userID, string gameId, string gameName)
        {
            var IntercomWrapper = new AndroidJavaClass("com.megafans.unitysdk.IntercomWrapper");
            IntercomWrapper.CallStatic("RegisterIntercomUserWithId", new object[] { userID, gameId, gameName });
        }

        /// <summary>
        /// Update username to intercom
        /// </summary>
        /// <param name="username">Current user username</param>
        public static void UpdateUsernameToIntercom(string username)
        {
            var IntercomWrapper = new AndroidJavaClass("com.megafans.unitysdk.IntercomWrapper");
            IntercomWrapper.CallStatic("UpdateUsernameToIntercom", username);
        }

        /// <summary>
        /// Show Intercom Button
        /// </summary>
        public static void ShowIntercom()
        {
            var IntercomWrapper = new AndroidJavaClass("com.megafans.unitysdk.IntercomWrapper");
            IntercomWrapper.CallStatic("ShowIntercom");
        }

        /// <summary>
        /// Hide Intercom Button
        /// </summary>
        public static void HideIntercom()
        {
            var IntercomWrapper = new AndroidJavaClass("com.megafans.unitysdk.IntercomWrapper");
            IntercomWrapper.CallStatic("HideIntercom");
        }

        /// <summary>
        /// Log Tournament Event to Intercom
        /// </summary>
        /// <param name="tournamentId">Tournament Id</param>
        /// <param name="levelId">level Id</param>
        /// <param name="score">Score for tournament</param>
        /// <param name="gameId">Current game Id</param>
        /// <param name="gameName">Current game name</param>
        public static void LogTournamentToIntercom(int tournamentId, float score, string gameId, string gameName)
        {
            Debug.Log("Log Tournament" + " - tournamneId = " + tournamentId + ", score = " + score + ", gameId = " + gameId + ", gameName = " + gameName);
            var IntercomWrapper = new AndroidJavaClass("com.megafans.unitysdk.IntercomWrapper");
            IntercomWrapper.CallStatic("LogTournamentToIntercom", tournamentId, score, gameId, gameName);
        }

        /// <summary>
        /// Logout User with from Intercom
        /// </summary>
        public static void LogoutFromIntercom()
        {         
            var IntercomWrapper = new AndroidJavaClass("com.megafans.unitysdk.IntercomWrapper");
            IntercomWrapper.CallStatic("LogOut");
        }

        public static void ShowIntercomIfUnreadMessages()
        {
            var IntercomWrapper = new AndroidJavaClass("com.megafans.unitysdk.IntercomWrapper");
            IntercomWrapper.CallStatic("ShowIntercomIfUnreadMessages");
        }
    }
}
