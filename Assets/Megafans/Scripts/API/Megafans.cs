using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.UI;
using MegafansSDK.Utils;

using MegaFans.Unity.Android;
using MegaFans.Unity.iOS;

namespace MegafansSDK {
	
	public class Megafans : MonoBehaviour
    {

        [SerializeField] private string gameUID = "";
        [SerializeField] private string tutorialUrl = "";
        [SerializeField] private string tokenPurchase200ProductID = "";
        [SerializeField] private string tokenPurchase1000ProductID = "";
        [SerializeField] private string tokenPurchase3000ProductID = "";
        [SerializeField] private string tokenPurchase10000ProductID = "";
        [SerializeField] private Sprite gameIcon;

        private Megafans() {
            Debug.Log("REPORT - INIT MEGAFANS");
        }

		private static Megafans instance = null;
		public static Megafans Instance {
			get {
				return instance;
			}
		}

		private ILandingOptionsListener landingOptionsListener;
		private IJoinGameCallback joinGameCallback;

		/// <summary>
		/// Gets or sets the Game Unique Identifier. Available at MegaFans developer portal.
		/// Set its value once at start.
		/// </summary>
		/// <value>The Game Unique Identifier.</value>
		public string GameUID {
			get {
				return gameUID;
			}

			set {
				gameUID = value;
			}
		}

        private string gameName = "";
        /// <summary>
        /// Gets or sets the Game Name. Provided by you the developer.
        /// Set its value once at start.
        /// </summary>
        /// <value>The Game Name.</value>
        public string GameName
        {
            get
            {
                return gameName;
            }

            set
            {
                gameName = value;
            }
        }

		/// <summary>
		/// Gets or sets the URL of a Tutorial video, which a user can watch from the MegaFans landing screen.
		/// Set its value once at start.
		/// </summary>
		/// <value>The URL of tutorial video.</value>
		public string TutorialUrl {
			get {
				return tutorialUrl;
			}

			set {
				tutorialUrl = value;
			}
		}

        /// <summary>
        /// Gets or sets the Game Icon, which is needed throughout the MegaFans SDK for your games branding
        /// Set its value once at start.
        /// </summary>
        /// <value>The Sprite for the current games icon</value>
        public Sprite GameIcon
        {
            get
            {
                return gameIcon;
            }

            set
            {
                GameIcon = value;
            }
        }

        /// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 200 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
        /// Set its value once at start.
        /// </summary>
        /// <value>The ProductID for 200 MegaFans Token Purchase.</value>
        public string ProductID200Tokens
        {
            get
            {
                return tokenPurchase200ProductID;
            }

            set
            {
                tokenPurchase200ProductID = value;
            }
        }

        /// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 1000 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
        /// Set its value once at start.
        /// </summary>
        /// <value>The ProductID for 1000 MegaFans Token Purchase.</value>
        public string ProductID1000Tokens
        {
            get
            {
                return tokenPurchase1000ProductID;
            }

            set
            {
                tokenPurchase1000ProductID = value;
            }
        }

        /// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 3000 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
        /// Set its value once at start.
        /// </summary>
        /// <value>The ProductID for 3000 MegaFans Token Purchase.</value>
        public string ProductID3000Tokens
        {
            get
            {
                return tokenPurchase3000ProductID;
            }

            set
            {
                tokenPurchase3000ProductID = value;
            }
        }

        /// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 10000 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
        /// Set its value once at start.
        /// </summary>
        /// <value>The ProductID for 10000 MegaFans Token Purchase.</value>
        public string ProductID10000Tokens
        {
            get
            {
                return tokenPurchase10000ProductID;
            }

            set
            {
                tokenPurchase10000ProductID = value;
            }
        }

        private string currentTournamentToken = "";
        private int currentTournamentId = 0;
		public int CurrentTournamentId {
			get {
				return currentTournamentId;
			}

			set {
				currentTournamentId = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the user is currently logged in to MegaFans.
		/// </summary>
		/// <value><c>true</c> if the user is logged in; otherwise, <c>false</c>.</value>
		public bool IsUserLoggedIn {
			get {
                string accessToken = MegafansPrefs.AccessToken;
                return accessToken != "";

            }
		}

		void Awake() {
			if (instance == null) {
				instance = this;
				DontDestroyOnLoad (this.gameObject);
			}
			else if (instance != this) {
				Destroy(gameObject);
			}
		}

		void Start() {
            Debug.Log("REPORT - START MEGAFANS");
        }

        public void ShowMegafans(IJoinGameCallback joinGameCallback,
            ILandingOptionsListener landingOptionsListener)
        {
            if (!Megafans.Instance.IsUserLoggedIn)
            {
                Megafans.Instance.ShowLandingScreen(joinGameCallback, landingOptionsListener);
            } else {

                Megafans.Instance.ShowTournamentLobby(joinGameCallback, landingOptionsListener);
            }
        }

        /// <summary>
        /// Shows the landing screen of MegaFans to the user.
        /// </summary>
        /// <param name="landingOptionsListener">An interface to detect callbacks from the landing screen.
        /// For example, when user clicks Play Game or when user logs in.</param>
        public void ShowLandingScreen(IJoinGameCallback joinGameCallback, ILandingOptionsListener landingOptionsListener) {
			this.landingOptionsListener = landingOptionsListener;
            this.joinGameCallback = joinGameCallback;

            MegafansUI.Instance.EnableUI (true);
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
			ILandingOptionsListener landingOptionsListener) {

			this.landingOptionsListener = landingOptionsListener;
			this.joinGameCallback = joinGameCallback;

			MegafansUI.Instance.EnableUI (true);
			MegafansUI.Instance.ShowTournamentLobby ();
		}

		/// <summary>
		/// Saves the user's score to server.
		/// </summary>
		/// <param name="score">Score of user.</param>
		/// <param name="token">Match token obtained from MegaFans API.</param>
		/// <param name="gameType">Game type obtained from IJoinGameCallback's StartGame function.</param>
		/// <param name="OnScoreSaved">Optional callback to take an action when score has been saved successfully.</param>
		/// <param name="OnFailed">Optional callback to take an action when score could not be saved.</param>
        public void SaveScore(float score, string metaString, GameType gameType, Action OnScoreSaved = null, Action<string> OnFailed = null) {
            MegafansUI.Instance.EnableUI(true);
            if (string.IsNullOrEmpty(this.currentTournamentToken)) {
                Debug.Log("No Tournament token found");
                return;
            }
            MegafansWebService.Instance.SaveScore (this.currentTournamentToken, score,
				(SaveScoreResponse response) => {
                    this.currentTournamentToken = "";
                    if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
                        MegafansUI.Instance.ShowLeaderboardWithScore(gameType, RankingType.RANKING, score, metaString);
//#if UNITY_EDITOR
//                        Debug.Log("Unity Editor");
//#elif UNITY_IOS
//                        Debug.Log("Log tournament iOS");
//                        IntercomWrapperiOS.LogTournamentToIntercom(this.currentTournamentId, score, GameUID, Application.productName);
//#elif UNITY_ANDROID
//                        Debug.Log("Log tournament Android");
//                        IntercomWrapperAndroid.LogTournamentToIntercom(this.currentTournamentId, score, GameUID, Application.productName);
//#endif
                        if (OnScoreSaved != null) {
                            Debug.Log("REPORT - SAVE SCORE");
                            OnScoreSaved ();
						}
					}
					else {
						if (OnFailed != null) {
							OnFailed (response.message);
						}
					}
				},
				(string error) => {
					if (OnFailed != null) {
						OnFailed (error);
					}
				});
		}

		/// <summary>
		/// Saves the tokens purchased by user to server. Call it after the user purchases tokens via IAP.
		/// </summary>
		/// <param name="transactionNumber">Transaction number returned by IAP.</param>
		/// <param name="numberOfTokens">Number of tokens purchased by user.</param>
		/// <param name="OnSuccess">Optional callback to detect this operation's success.</param>
		/// <param name="OnFailure">Optional callback to detect this operation's failure.</param>
		public void SaveTokens(string transactionNumber, string receipt, string productId, int numberOfTokens, Action OnSuccess = null,
			Action<string> OnFailure = null) {

			MegafansWebService.Instance.BuyTokens (GameUID, numberOfTokens, transactionNumber, productId, receipt,
				(BuyTokensResponse response) => {
					if(response.success.Equals(MegafansConstants.SUCCESS_CODE)) {
                        //MegafansUI.Instance.ShowStoreWindow(numberOfTokens, true);
                        MegafansPrefs.CurrentTokenBalance += numberOfTokens;
                        MegafansUI.Instance.ShowStoreWindow();

                        if (OnSuccess != null) {
							OnSuccess();
						}
					}
					else {
						if(OnFailure != null) {
							OnFailure(response.message);
						}
					}
				},
				(string error) => {
					if(OnFailure != null) {
						OnFailure(error);
					}
				});
		}


		internal void ReportPlayGameClicked() {
			if (landingOptionsListener != null) {
				landingOptionsListener.OnPlayGameClicked ();
			}
		}

		internal void ReportUserLoggedIn(string userId) {
			if (landingOptionsListener != null) {
                Debug.Log("REPORT - USER LOGGED IN");
#if UNITY_EDITOR
                Debug.Log("Unity Editor");
#elif UNITY_IOS
                Debug.Log("IOS");
                IntercomWrapperiOS.RegisterIntercomUserWithID(userId, GameUID, Application.productName);
#elif UNITY_ANDROID
                Debug.Log("ANDROID");
                IntercomWrapperAndroid.RegisterUserWithUserId(userId, GameUID, Application.productName);
#endif

                landingOptionsListener.OnUserLoggedIn (userId);
			}
		}

		internal void ReportUserRegistered(string userId) {
			if (landingOptionsListener != null) {
                Debug.Log("REPORT - USER REGISTERED");
                landingOptionsListener.OnUserRegistered ();
            }
        }

        internal void ReportStartGame(string tournamentToken, int tournamentID, GameType gameType, Dictionary<string, string> metaData) {
            if (joinGameCallback != null)
            {
                Debug.Log("REPORT - START GAME");
                this.currentTournamentToken = tournamentToken;
                joinGameCallback.StartGame(gameType, metaData);
            }
        }


        internal void ReportPurchaseTokens(int numberOfTokens) {
			if (joinGameCallback != null) {
				joinGameCallback.PurchaseTokens (numberOfTokens);
			}
		}
    }
}