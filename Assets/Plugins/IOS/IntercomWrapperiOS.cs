using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace MegaFans.Unity.iOS
{
    /// <summary>
    /// Simple C# wrapper to externally link Unity SDK calls to the iOS SDK.
    /// Any new iOS methods that must have a Unity implementation should be defined, and called here.
    /// </summary>
    public class IntercomWrapperiOS
    {
        #region EXTERN_DECLARATIONS

        /// <summary>
        /// Register User with intercom C function call
        /// </summary>
        /// <param name="withUserId">Current user id</param>
        /// <param name="email">Current user email address</param>
        /// <param name="gameId">Current game id</param>
        /// <param name="gameName">Current game name</param>
        [DllImport("__Internal")]
        private static extern void _IntercomWrapperiOS_registerUserWithUserId(string userID, string gameId, string gameName);

        /// <summary>
        /// Update username to intercom C function call
        /// </summary>
        /// <param name="username">Current user username</param>
        [DllImport("__Internal")]
        private static extern void _IntercomWrapperiOS_updateUsername(string username);

        /// <summary>
        /// Show Intercom Button C function call
        /// </summary>
        [DllImport("__Internal")]
        private static extern void _IntercomWrapperiOS_showIntercom();

        /// <summary>
        /// Hide Intercom Button C function call
        /// </summary>
        [DllImport("__Internal")]
        private static extern void _IntercomWrapperiOS_hideIntercom();

        /// <summary>
        /// Show intercom if unread messages from intercom C function call
        /// </summary>
        [DllImport("__Internal")]
        private static extern void _IntercomWrapperiOS_showIntercomIfUnreadMessages();

        /// <summary>
        /// Log Tournament Event to Intercom
        /// </summary>
        /// <param name="tournamentId">Tournament Id</param>
        /// <param name="levelId">level Id</param>
        /// <param name="score">Score for tournament</param>
        /// <param name="gameId">Current game Id</param>
        /// <param name="gameName">Current game name</param>
        [DllImport("__Internal")]
        private static extern void _IntercomWrapperiOS_didFinishTournamentWithScore(int tournamentId, float score, string gameId, string gameName);

        /// <summary>
        /// Logout User from intercom C function call
        /// </summary>
        [DllImport("__Internal")]
        private static extern void _IntercomWrapperiOS_logOut();

        #endregion



        //=========================================== DEFINITIONS =========================================

        /// <summary>
        /// Register User with user Id action
        /// </summary>
        /// <param name="userId">Current user id</param>
        /// <param name="email">Current user email address</param>
        /// <param name="gameId">Current game id</param>
        /// <param name="gameName">Current game name</param>
        public static void RegisterIntercomUserWithID(string userID, string gameId, string gameName)
        {
            Debug.Log("Regist USER");
            _IntercomWrapperiOS_registerUserWithUserId(userID, gameId, gameName);
        }

        /// <summary>
        /// Update username to intercom
        /// </summary>
        /// <param name="username">Current user username</param>
        public static void UpdateUsernameToIntercom(string username)
        {
            Debug.Log("Regist USER");
            _IntercomWrapperiOS_updateUsername(username);
        }

        /// <summary>
        /// Show Intercom Button
        /// </summary>
        public static void ShowIntercom()
        {
            Debug.Log("Show Intercom");
            _IntercomWrapperiOS_showIntercom();
        }

        /// <summary>
        /// Hide Intercom Button
        /// </summary>
        public static void HideIntercom()
        {
            Debug.Log("Show Intercom");
            _IntercomWrapperiOS_hideIntercom();
        }

        /// <summary>
        /// Show intercom if unread messages from intercom C function call
        /// </summary>
        public static void ShowIntercomIfUnreadMessages()
        {
            Debug.Log("Show Intercom");
            _IntercomWrapperiOS_showIntercomIfUnreadMessages();
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
            _IntercomWrapperiOS_didFinishTournamentWithScore(tournamentId, score, gameId, gameName);
        }

        /// <summary>
        /// Logout User with from Intercom
        /// </summary>
        public static void LogoutFromIntercom()
        {
            Debug.Log("Log Out USER");
            _IntercomWrapperiOS_logOut();
        }
    }
}