#pragma warning disable 649
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

using Megafan.NativeWrapper;
using MegafansSDK.UI;
using MegafansSDK.Utils;
using UnityEngine.UIElements;
using MegafansSDK.AdsManagerAPI;
using System.Collections;

namespace MegafansSDK
{
    [System.Serializable]
    public class InAppId
    {
        public string AndroidId;
        public string iosId;
        public string value;
    }
    public class Megafans : MonoBehaviour
    {

        [Space(10), Header("Game Settings:")]
        [SerializeField] string withdrawURL;
        [SerializeField] string gameUID;
        [SerializeField] Sprite gameIcon;
        [SerializeField] Texture gameTexture;
        [SerializeField] string oneSignalAppId;
        [SerializeField] string AndroidIntercomAppId;
        [SerializeField] string AndroidIntercomAppKey;
        [SerializeField] string IOSIntercomAppId;
        [SerializeField] string IOSIntercomAppKey;
        [SerializeField] public string discordChannelURL;
        [Space(10), Header("Purchase IDs:")]
        [SerializeField] InAppId tokenPurchase200ProductID;
        [SerializeField] InAppId tokenPurchase1000ProductID;
        [SerializeField] InAppId tokenPurchase3000ProductID;
        [SerializeField] InAppId tokenPurchase10000ProductID;
        [Space(10), Header("Deployment Environment:")]
        [SerializeField] Deployment deployment;
        [SerializeField] string customDeployment;
        [SerializeField] public string AdvertisingID;


        string currentTournamentToken = "";

        static Megafans instance = null;
        public static Megafans Instance => instance;

        ILandingOptionsListener landingOptionsListener;
        IJoinGameCallback joinGameCallback;

        internal AdsManager m_AdsManager;
        internal List<LevelsResponseData> m_AllTournaments;
        internal int CurrentTournamentId;
        internal int CurrentTounamentFreeEntries;
        internal bool CurrentUsingFreeEntry;

        /// <summary>
        /// Gets or sets the Game Unique Identifier. Available at MegaFans developer portal.
        /// Set its value once at start.
        /// </summary>
        /// <value>The Game Unique Identifier.</value>
        public string GameUID => gameUID;
        /// <summary>
        /// Gets or sets the Game Icon, which is needed throughout the MegaFans SDK for your games branding
        /// Set its value once at start.
        /// </summary>
        /// <value>The Sprite for the current games icon</value>
        public Sprite GameIcon => gameIcon;

        /// <summary>
        /// Gets or sets the Game Icon, which is needed throughout the MegaFans SDK for your games branding
        /// Set its value once at start.
        /// </summary>
        /// <value>The Sprite for the current games icon</value>
        public Texture GameTexture => gameTexture;

        /// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 200 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
        /// Set its value once at start.
        /// </summary>
        /// <value>The ProductID for 200 MegaFans Token Purchase.</value>
        [HideInInspector]public string ProductID200Tokens;
        /// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 1000 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
        /// Set its value once at start.
        /// </summary>
        /// <value>The ProductID for 1000 MegaFans Token Purchase.</value>
        [HideInInspector]public string ProductID1000Tokens;

        /// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 3000 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
        /// Set its value once at start.
        /// </summary>
        /// <value>The ProductID for 3000 MegaFans Token Purchase.</value>
        [HideInInspector]public string ProductID3000Tokens;

        /// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 10000 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
        /// </summary>
        /// <value>The ProductID for 10000 MegaFans Token Purchase.</value>
        [HideInInspector]public string ProductID10000Tokens;

        /// <summary>
        /// Gets a value indicating whether the user is currently logged in to MegaFans.
        /// </summary>
        /// <value><c>true</c> if the user is logged in; otherwise, <c>false</c>.</value>
        public bool IsUserLoggedIn => !string.IsNullOrEmpty(MegafansPrefs.AccessToken);

        public string WithdrawURL => withdrawURL;

        public LocationInfo? lastLocationData;
        public bool checkingLocationServices = false;
        public bool locationServicesDenied
        {
            get { return locationServicesDenied; }
            set
            {
                if (value && !checkingLocationServices)
                {
#if !UNITY_EDITOR
                    MegafansUI.Instance.ShowLocationServicesDeniedWarning();
#endif
                }
                else
                {
                    MegafansUI.Instance.HideAlertDialog();
                }
            }
        }

        public void ShowMegafans(IJoinGameCallback joinGameCallback, ILandingOptionsListener landingOptionsListener)
        {
            if (Instance.IsUserLoggedIn)
            {
#if !UNITY_EDITOR
                CheckForLocationPermissions();
#endif
                Instance.ShowTournamentLobby(joinGameCallback, landingOptionsListener);
            }
            else
            {
                Instance.ShowLandingScreen(joinGameCallback, landingOptionsListener);
            }
        }

        /// <summary>
        /// Shows the landing screen of MegaFans to the user.
        /// </summary>
        /// <param name="landingOptionsListener">An interface to detect callbacks from the landing screen.
        /// For example, when user clicks Play Game or when user logs in.</param>
        public void ShowLandingScreen(IJoinGameCallback joinGameCallback, ILandingOptionsListener landingOptionsListener)
        {
            this.landingOptionsListener = landingOptionsListener;
            this.joinGameCallback = joinGameCallback;
            MegafansUI.Instance.EnableUI(true);
            MegafansUI.Instance.ShowOnboardingStartWindow();
        }

        /// <summary>
        /// Shows the Tournament Lobby to the user.
        /// </summary>
        /// <param name="joinGameCallback">Interface to detect callbacks from tournament lobby screen.
        /// For example, when user joins a match or when user needs to purchase tokens.</param>
        /// <param name="landingOptionsListener">Interface to detect callbacks from the landing screen.
        /// A user can logout from the tournament lobby screen. After that, he is taken to the landing screen
        /// automatically and therefore, ILandingOptionsListener is required again to detect landing screen
        /// callbacks.</param>
        public void ShowTournamentLobby(IJoinGameCallback joinGameCallback,
            ILandingOptionsListener landingOptionsListener)
        {

            this.landingOptionsListener = landingOptionsListener;
            this.joinGameCallback = joinGameCallback;
            MegafansUI.Instance.EnableUI(true);
            MegafansUI.Instance.ShowTournamentLobby();
        }

        /// <summary>
        /// Saves the user's score to server.
        /// </summary>
        /// <param name="score">Score of user.</param>
        /// <param name="token">Match token obtained from MegaFans API.</param>
        /// <param name="gameType">Game type obtained from IJoinGameCallback's StartGame function.</param>
        /// <param name="onScoreSaved">Optional callback to take an action when score has been saved successfully.</param>
        /// <param name="onFailed">Optional callback to take an action when score could not be saved.</param>
        public void SaveScore(int score, string metaString, GameType gameType, Action onScoreSaved = null,
            Action<string> onFailed = null)
        {

            MegafansUI.Instance.EnableUI(true);

            if (string.IsNullOrEmpty(this.currentTournamentToken))
            {
                Debug.Log("No Tournament token found");
                return;
            }
#if !UNITY_EDITOR
            var locationData = lastLocationData.Value.latitude + "," + lastLocationData.Value.latitude;
#else
            var locationData = "latitude,longitude";
#endif
            MegafansWebService.Instance.SaveScoreNew(this.currentTournamentToken, score, locationData,
                (SaveScoreResponse response) =>
                {
                    //this.currentTournamentToken = "";
                    if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
                    {
                        if (onScoreSaved != null)
                        {
                            Debug.Log("REPORT - SAVE SCORE");
                            Debug.Log("working with Ciel " + response.message);
                            Debug.Log(response);
                            onScoreSaved();
                            MegafansUI.Instance.ShowLeaderboardWithScore(gameType, RankingType.LEADERBOARD, score, metaString, currentTournamentToken);
                        }
                    }
                    else
                    {
                        onFailed?.Invoke(response.message);
                    }
                },
                (error) => onFailed?.Invoke(error));
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            //OneSignal.StartInit(oneSignalAppId).EndInit();
            m_AdsManager = GetComponent<AdsManager>();

#if UNITY_ANDROID && !UNITY_EDITOR
            MegafanNativeWrapper.InitIntercom(AndroidIntercomAppId, AndroidIntercomAppKey);
            MegafanNativeWrapper.HideIntercom();
#elif UNITY_IOS && !UNITY_EDITOR
            MegafanNativeWrapper.InitIntercom(IOSIntercomAppId,IOSIntercomAppKey);
            MegafanNativeWrapper.HideIntercom();
#endif


#if UNITY_ANDROID
            ProductID200Tokens = tokenPurchase200ProductID.AndroidId;
            ProductID1000Tokens = tokenPurchase1000ProductID.AndroidId;
            ProductID3000Tokens = tokenPurchase3000ProductID.AndroidId;
            ProductID10000Tokens = tokenPurchase10000ProductID.AndroidId;
#elif UNITY_IOS
            ProductID200Tokens = tokenPurchase200ProductID.iosId;
                ProductID1000Tokens = tokenPurchase1000ProductID.iosId;
                ProductID3000Tokens = tokenPurchase3000ProductID.iosId;
                ProductID10000Tokens = tokenPurchase10000ProductID.iosId;
#endif

            if (Instance.IsUserLoggedIn)
            {
                Megafans.Instance.m_AdsManager.initIronSourceWithUserId(MegafansPrefs.UserId.ToString());
            }
        }

        /// <summary>
        /// Saves the tokens purchased by user to server. Call it after the user purchases tokens via IAP.
        /// </summary>
        /// <param name="transactionNumber">Transaction number returned by IAP.</param>
        /// <param name="numberOfTokens">Number of tokens purchased by user.</param>
        /// <param name="onSuccess">Optional callback to detect this operation's success.</param>
        /// <param name="onFailure">Optional callback to detect this operation's failure.</param>
        public void SaveTokens(string transactionNumber, string receipt, string productId, int numberOfTokens,
            Action onSuccess = null, Action<string> onFailure = null)
        {

            MegafansWebService.Instance.BuyTokens(GameUID, numberOfTokens, transactionNumber, productId, receipt,
                (BuyTokensResponse response) =>
                {
                    if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
                    {
                        //MegafansUI.Instance.ShowStoreWindow(numberOfTokens, true);
                        MegafansPrefs.CurrentTokenBalance += numberOfTokens;
                        MegafansUI.Instance.ShowStoreWindow();
                        onSuccess?.Invoke();
                    }
                    else
                    {
                        onFailure?.Invoke(response.message);
                    }
                },
                (error) => onFailure?.Invoke(error));
        }


        public string ServerBaseUrl()
        {
            switch (deployment)
            {
                case Deployment.Development:
                    return "https://gameapi-dev.megafans.com"; //Dev
                case Deployment.Staging:
                    return "https://gameapi-staging.megafans.com"; //Staging
                case Deployment.Production:
                    return "https://gameapi.megafans.com"; //Prod
                case Deployment.Custom:
                    return customDeployment;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        internal void ReportPlayGameClicked()
        {
            landingOptionsListener?.OnPlayGameClicked();
        }

        internal void ReportUserLoggedIn(string userId)
        {
            Megafans.Instance.m_AdsManager.initIronSourceWithUserId(userId);
            landingOptionsListener?.OnUserLoggedIn(userId);
        }


        internal void ReportUserRegistered(string userId)
        {
            landingOptionsListener?.OnUserLoggedIn(userId);
        }

        internal void ReportStartGame(string tournamentToken, int tournamentID, GameType gameType, Dictionary<string, string> metaData)
        {
            if (joinGameCallback != null)
            {
                Debug.Log("REPORT - START GAME");
                this.currentTournamentToken = tournamentToken;
                joinGameCallback.StartGame(gameType, metaData);
            }
        }

        internal void CleanTokenWhenFinishedTournament()
        {
            currentTournamentToken = "";
        }

        internal void ReportPurchaseTokens(int numberOfTokens)
        {
            joinGameCallback?.PurchaseTokens(numberOfTokens);
        }

        internal LevelsResponseData GetCurrentTournamentData()
        {
            return m_AllTournaments.Find(w => w.id == CurrentTournamentId);
        }

        internal int GetCurrentTournamentIndex()
        {
            return m_AllTournaments.FindIndex(w => w.id == CurrentTournamentId);
        }

        public void CheckForLocationPermissions()
        {
            Debug.Log("CHECKING FOR LOCATION PERMISSIONS");
            if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
            {
                Debug.Log("GETTING LOCATION PERMISSIONS");
                Permission.RequestUserPermission(Permission.CoarseLocation);
            }
            checkingLocationServices = true;
            StartCoroutine(StartLocationService());
        }       

        internal IEnumerator StartLocationService()
        {

            while (!Input.location.isEnabledByUser)
            {
                Permission.RequestUserPermission(Permission.CoarseLocation);
                this.locationServicesDenied = true;
                yield return new WaitForSeconds(1);
            }
            Input.location.Start();
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                Debug.Log("Location Services Initializing");
                this.locationServicesDenied = false;
                yield return new WaitForSeconds(1);
                maxWait--;
            }
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("Unable to determine device location");
                this.checkingLocationServices = false;
                this.locationServicesDenied = true;               
                Input.location.Stop();
                yield break;
            }
            this.locationServicesDenied = false;
            this.checkingLocationServices = false;
            Debug.Log("Latitude : " + Input.location.lastData.latitude);
            Debug.Log("Longitude : " + Input.location.lastData.longitude);
            Debug.Log("Altitude : " + Input.location.lastData.altitude);
            this.lastLocationData = Input.location.lastData;
            Input.location.Stop();
        }
    }
}