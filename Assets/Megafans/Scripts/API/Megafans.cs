#pragma warning disable 649
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.UI;
using MegafansSDK.Utils;
using UnityEngine.UIElements;


namespace MegafansSDK {
	
	public class Megafans : MonoBehaviour {

		[Space(10), Header("Game Settings:")]
		[SerializeField] string gameUID;
		[SerializeField] Sprite gameIcon;
		[SerializeField] string tutorialUrl;
		[Space(10), Header("Purchase IDs:")]
		[SerializeField] string tokenPurchase200ProductID;
        [SerializeField] string tokenPurchase1000ProductID;
        [SerializeField] string tokenPurchase3000ProductID;
		[SerializeField] string tokenPurchase10000ProductID;
		[Space(10), Header("Deployment Environment:")]
		[SerializeField] Deployment deployment;
		[SerializeField] string customDeployment;


		string currentTournamentToken = "";

		static Megafans instance = null;
		public static Megafans Instance => instance;

		ILandingOptionsListener landingOptionsListener;
		IJoinGameCallback joinGameCallback;

		public int CurrentTournamentId  { get ; set ; } = 0;

		/// <summary>
		/// Gets or sets the Game Unique Identifier. Available at MegaFans developer portal.
		/// Set its value once at start.
		/// </summary>
		/// <value>The Game Unique Identifier.</value>
		public string GameUID => gameUID;

		/// <summary>
		/// Gets or sets the URL of a Tutorial video, which a user can watch from the MegaFans landing screen.
		/// Set its value once at start.
		/// </summary>
		/// <value>The URL of tutorial video.</value>
		public string TutorialUrl =>  tutorialUrl;

        /// <summary>
        /// Gets or sets the Game Icon, which is needed throughout the MegaFans SDK for your games branding
        /// Set its value once at start.
        /// </summary>
        /// <value>The Sprite for the current games icon</value>
        public Sprite GameIcon => gameIcon;

        /// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 200 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
        /// Set its value once at start.
        /// </summary>
        /// <value>The ProductID for 200 MegaFans Token Purchase.</value>
        public string ProductID200Tokens => tokenPurchase200ProductID;

        /// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 1000 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
        /// Set its value once at start.
        /// </summary>
        /// <value>The ProductID for 1000 MegaFans Token Purchase.</value>
        public string ProductID1000Tokens => tokenPurchase1000ProductID;

        /// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 3000 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
        /// Set its value once at start.
        /// </summary>
        /// <value>The ProductID for 3000 MegaFans Token Purchase.</value>
        public string ProductID3000Tokens => tokenPurchase3000ProductID;

		/// <summary>
        /// Gets or sets the In-App ProductID for the purchase of 10000 MegaFans Tokens. This is the product ID given to you from the Apple App Store and Google Play store, should be the same for both stores.
		/// </summary>
        /// <value>The ProductID for 10000 MegaFans Token Purchase.</value>
		public string ProductID10000Tokens => tokenPurchase10000ProductID;

		/// <summary>
		/// Gets a value indicating whether the user is currently logged in to MegaFans.
		/// </summary>
		/// <value><c>true</c> if the user is logged in; otherwise, <c>false</c>.</value>
		public bool IsUserLoggedIn => !string.IsNullOrEmpty(MegafansPrefs.AccessToken);





		//Private constructor
		Megafans() { }


		void Awake() {
			if (instance == null) {
				instance = this;
				DontDestroyOnLoad (this.gameObject);
			}
			else if (instance != this) {
				Destroy(gameObject);
			}
		}


        public void ShowMegafans(IJoinGameCallback joinGameCallback, ILandingOptionsListener landingOptionsListener) {
            if (Instance.IsUserLoggedIn)
				Instance.ShowTournamentLobby(joinGameCallback, landingOptionsListener);
			else
                Instance.ShowLandingScreen(joinGameCallback, landingOptionsListener);
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
		/// <param name="onScoreSaved">Optional callback to take an action when score has been saved successfully.</param>
		/// <param name="onFailed">Optional callback to take an action when score could not be saved.</param>
        public void SaveScore(int score, string metaString, GameType gameType, Action onScoreSaved = null,
			Action<string> onFailed = null) {

            MegafansUI.Instance.EnableUI(true);
            if (string.IsNullOrEmpty(this.currentTournamentToken)) {
                Debug.Log("No Tournament token found");
                return;
            }
            MegafansUI.Instance.ShowLeaderboardWithScore(gameType, RankingType.RANKING, score, metaString, currentTournamentToken);
            MegafansWebService.Instance.SaveScore (this.currentTournamentToken, score,
				(SaveScoreResponse response) => {
                    this.currentTournamentToken = "";
                    if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
						if (onScoreSaved != null) {
                            Debug.Log("REPORT - SAVE SCORE");
                            onScoreSaved ();
						}
					}
					else {
						onFailed?.Invoke (response.message);
					}
				},
				(error) => onFailed?.Invoke (error));
		}

		/// <summary>
		/// Saves the tokens purchased by user to server. Call it after the user purchases tokens via IAP.
		/// </summary>
		/// <param name="transactionNumber">Transaction number returned by IAP.</param>
		/// <param name="numberOfTokens">Number of tokens purchased by user.</param>
		/// <param name="onSuccess">Optional callback to detect this operation's success.</param>
		/// <param name="onFailure">Optional callback to detect this operation's failure.</param>
		public void SaveTokens(string transactionNumber, string receipt, string productId, int numberOfTokens,
			Action onSuccess = null, Action<string> onFailure = null) {

			MegafansWebService.Instance.BuyTokens (GameUID, numberOfTokens, transactionNumber, productId, receipt,
				(BuyTokensResponse response) => {
					if(response.success.Equals(MegafansConstants.SUCCESS_CODE)) {
                        //MegafansUI.Instance.ShowStoreWindow(numberOfTokens, true);
                        MegafansPrefs.CurrentTokenBalance += numberOfTokens;
                        MegafansUI.Instance.ShowStoreWindow();
						onSuccess?.Invoke();
					}
					else {
						onFailure?.Invoke(response.message);
					}
				},
				(error) => onFailure?.Invoke(error));
		}


		public string ServerBaseUrl () {
			switch (deployment) {
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


		internal void ReportPlayGameClicked() {
			landingOptionsListener?.OnPlayGameClicked ();
		}


		internal void ReportUserLoggedIn(string userId) {
			landingOptionsListener?.OnUserLoggedIn (userId);
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
			joinGameCallback?.PurchaseTokens (numberOfTokens);
		}
    }
}