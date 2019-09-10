using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using Newtonsoft.Json;

using MegafansSDK.UI;

namespace MegafansSDK.Utils {
	
	public class MegafansWebService : MonoBehaviour {

		private MegafansWebService() {

		}

		private static MegafansWebService instance = null;
		public static MegafansWebService Instance {
			get {
				return instance;
			}
		}

		private string ServerBaseUrl {
			get {
                //return "https://megafansapi-dev.azurewebsites.net";
                //return "http://192.168.0.24:3001";
                //return "https://gameapi.megafans.com";
                return "https://gameapi-dev.megafans.com";
            }
		}

        // Updated
		private string RegisterUrlEmail {
			get {
				return "/register_usernamepassword";
			}
		}

        // Updated
        private string RegisterUrlNewUser
        {
            get
            {
                return "/register_newuser";
            }
        }

        // Updated
        private string RegisterUrlFB {
			get {
				return "/register_facebook";
			}
		}

        // Updated
        private string RegisterUrlPhone
        {
            get
            {
                return "/register_phone";
            }
        }

        // Updated
        private string LoginUrlEmail {
			get {
				return "/login";
			}
		}

        // Updated
        private string LoginUrlFB {
            get
            {
                return "/login_fb";
            }
        }

        // Updated
        private string LoginUrlPhone {
			get {
				return "/login_phone";
			}
		}

        // Updated
        private string OtpUrl {
			get {
				return "/verifyphone";
			}
		}

		private string ResetPasswordUrl {
			get {
				return "/forgot_password";
			}
		}

        // Updated
        private string UpdatePasswordUrl
        {
            get
            {
                return "/change_password";
            }
        }

        // Updated
        private string GetTournamentsUrl {
			get {
				return "/view_all_tournaments";
			}
		}

        // Updated
        private string ViewProfileUrl {
			get {
                return "/view_profile";
			}
		}

		private string EditProfileUrl {
			get {
				return "/edit_profile";
			}
		}

        // Updated
        private string UploadImageUrl {
            get {
                return "/image_upload";
            }
        }

        // Updated
        private string CheckCreditsUrl {
			get {
				return "/show_credit";
			}
		}

        // Updated
        private string EnterTournamentUrl {
			get {
				return "/enter_tournament";
			}
		}

        // Updated
        private string EnterPracticeUrl {
            get
            {
                return "/enter_practice";
            }
        }

        // Updated
        private string SaveScoreUrl {
			get {
				return "/save_game_scores";
			}
		}

        // Updated
        private string ViewPracticeScoreboardUrl {
			get {
				return "/practice_score_leaderboard";
			}
		}

        // Updated
        private string ViewScoreboardUrl {
            get {
                return "/tournament_score_leaderboard";
            }
        }

        // Updated
        private string ViewCurrentUserPracticeScoreboardUrl {
            get
            {
                return "/practice_my_leaderboard";
            }
        }

        // Updated
        private string ViewCurrentUserScoreboardUrl {
            get
            {
                return "/tournament_my_leaderboard";
            }
        }

        // Updated
        private string ViewleaderboardUrl {
			get {
				return "/tournament_rank_leaderboard";
			}
		}

        // Updated
        private string GetTournamentRulesUrl
        {
            get
            {
                return "/rules";
            }
        }

        // Updated
        private string GetTermsOfUseUrl
        {
            get
            {
                return "/terms_of_use";
            }
        }

        // Updated
        private string GetPrivacyPolicyUrl
        {
            get
            {
                return "/privacy_policy";
            }
        }

        // Updated
        private string ViewPracticeLeaderboardUrl{
            get {
                return "/practice_rank_leaderboard";
            }
        }

        // Updated
        private string BuyTokensUrl {
			get {
				return "/buy_tokens";
			}
		}

        // Updated
        private string RefreshAccessTokenUrl {
            get {
                return "/refresh";
            }
        }

        // Updated
        private string ResendVerificationEmailUrl {
            get
            {
                return "/resend_confirmation_email";
            }
        }

        // Updated
        private string LogoutUrl {
			get {
				return "/logout";
			}
		}

		private MegafansFBHelper fbHelper;
        private bool isRefreshingToken = false;

        void Awake() {
			if (instance == null) {
				instance = this;
				DontDestroyOnLoad (this.gameObject);
			}
			else if (instance != this) {
				Destroy(gameObject);
			}

			if (fbHelper == null) {
				fbHelper = this.gameObject.AddComponent<MegafansFBHelper> ();
			}
		}

		void Start() {
			
		}

		private void Request<T>(Request request, string url, Action<T> responseCallback,
			Action<string> failureCallback, bool isSilent = false) {
            if (url != (ServerBaseUrl + RefreshAccessTokenUrl)) {
                isRefreshingToken = false;
            } else {
                if (isRefreshingToken) {
                    string error = "You current session is unauthorized.  Please log back into MegaFans.";
                    MegafansUI.Instance.ShowPopup("Unauthorized", error);
                    MegafansPrefs.ClearPrefs();
                    return;
                } else {
                    isRefreshingToken = true;
                }
            }
			StartCoroutine (RequestCoroutine<T> (request, url, responseCallback, failureCallback, isSilent));
		}

		private IEnumerator RequestCoroutine<T>(Request request, string url, Action<T> responseCallback,
			Action<string> failureCallback, bool isSilent) {

			if (!isSilent) {
				MegafansUI.Instance.ShowLoadingBar ();
			}

		//	Debug.Log ("REQUEST:");
		//	Debug.Log (request.ToJson ());
			WWW www = request.GetWWW (url);
			yield return www;

			if (!isSilent) {
				MegafansUI.Instance.HideLoadingBar ();
			}


            if (String.IsNullOrEmpty (www.error)) {
				Debug.Log ("RESPONSE:");
				Debug.Log (www.text);

				try {
					T response = MegafansJsonHelper.FromJson<T> (www.text);
					responseCallback (response);
				}
				catch(Exception e) {
					Debug.LogError (e.ToString());

					string error = "Something went wrong. Please try later.";
					failureCallback (error);
				}
			}
			else {
                Debug.Log("ERROR RESPONSE:");
                Debug.Log(www.error);
                string getStatusCode = Regex.Match(www.error, @"\d+").Value;
                int statusCode = int.Parse(getStatusCode);
                if (statusCode == 401) {
                    if (!string.IsNullOrEmpty(MegafansPrefs.AccessToken) && !string.IsNullOrEmpty(MegafansPrefs.RefreshToken)) {
                        this.RefreshAccessToken((RefreshAccessTokenResponse response) => {
                            if (response.success.Equals(MegafansConstants.SUCCESS_CODE)) {
                                if (response.data != null)
                                {
                                    MegafansPrefs.AccessToken = response.data.token;
                                    MegafansPrefs.RefreshToken = response.data.refresh;
                                    Request <T> (request, url, responseCallback, failureCallback);
                                }
                                else
                                {
                                    string error = "You current session is unauthorized.  Please log back into MegaFans.";
                                    MegafansUI.Instance.ShowPopup("Unauthorized", error);
                                    MegafansPrefs.ClearPrefs();
                                }
                            }
                            else
                            {
                                string error = "You current session is unauthorized.  Please log back into MegaFans.";
                                MegafansUI.Instance.ShowPopup("Unauthorized", error);
                                MegafansPrefs.ClearPrefs();
                            }
                        }, (string error) => {
                            Debug.LogError(error);
                        });
                    } else {
                        Debug.Log("No access or refresh token found");
                    }
                } else if (!isSilent) {
                    if (isRefreshingToken) {
                        string error = "You current session is unauthorized.  Please log back into MegaFans.";
                        MegafansUI.Instance.ShowPopup("Unauthorized", error);
                        MegafansPrefs.ClearPrefs();
                        MegafansUI.Instance.ShowLandingWindow(true);
                        this.isRefreshingToken = false;
                    } else {
                        string error = "There was an error. Please check your internet connection.";
                        try {
                            Response response = MegafansJsonHelper.FromJson<Response>(www.text);
                            error = response.message;
                        } catch (Exception e) {
                            Debug.LogError(e.ToString());
                        }
                        MegafansUI.Instance.ShowPopup("ERROR", error);
                    }			
				}

				failureCallback (www.error);
			}
		}

        public void RegisterNewUser(Action<RegisterFirstTimeResponse> responseCallback, Action<string> failureCallback)
        {

            RegisterFirstTimeUserRequest registerRequest = new RegisterFirstTimeUserRequest();
            registerRequest.device_token = DeviceInfo.DeviceToken;
            registerRequest.device_type = DeviceInfo.DeviceType;
            registerRequest.appGameUid = Megafans.Instance.GameUID;

            string url = ServerBaseUrl + RegisterUrlNewUser;
            Request<RegisterFirstTimeResponse>(registerRequest, url, responseCallback, failureCallback);
        }

        public void RegisterPhone(string username, string phoneNumber,
			Action<RegisterResponse> responseCallback, Action<string> failureCallback) {

			RegisterRequestPhone registerRequest = new RegisterRequestPhone ();
			registerRequest.username = username;
			registerRequest.phone_number = phoneNumber;
			registerRequest.device_token = DeviceInfo.DeviceToken;
			//registerRequest.device_type = DeviceInfo.DeviceType;
			registerRequest.registerType = MegafansConstants.PHONE;

            string url = ServerBaseUrl + RegisterUrlPhone;
			Request<RegisterResponse> (registerRequest, url, responseCallback, failureCallback);
		}

		public void RegisterEmail(string email, string password,
			Action<RegisterResponse> responseCallback, Action<string> failureCallback) {

			RegisterRequestEmail registerRequest = new RegisterRequestEmail ();
			registerRequest.email = email;
			registerRequest.password = password;
			registerRequest.device_token = DeviceInfo.DeviceToken;
			registerRequest.device_type = DeviceInfo.DeviceType;

			string url = ServerBaseUrl + RegisterUrlEmail;
			Request<RegisterResponse> (registerRequest, url, responseCallback, failureCallback);
		}

		public void RegisterFB(Action<RegisterResponse> responseCallback, Action<string> failureCallback) {
			MegafansUI.Instance.ShowLoadingBar ();

			fbHelper.Logout ();

            fbHelper.RegisterRequest ((string userId, string email, string username, Texture2D newImageToUpload) => {
                Debug.Log("Register Facebook");
                Debug.Log(userId);
                Debug.Log(username);
                Debug.Log(newImageToUpload);
                Debug.Log(email);

                RegisterRequestFB registerRequest = new RegisterRequestFB();
				registerRequest.facebook_login_id = userId;
				registerRequest.username = username;
                registerRequest.email = email;

                MegafansPrefs.Email = email;
                MegafansPrefs.Username = username;
                MegafansPrefs.ProfilePic = MegafansUtils.TextureToString(newImageToUpload);

                registerRequest.device_token = DeviceInfo.DeviceToken;
				registerRequest.device_type = DeviceInfo.DeviceType;
				registerRequest.registerType = MegafansConstants.FB;

				string url = ServerBaseUrl + RegisterUrlFB;
				Request<RegisterResponse>(registerRequest, url, responseCallback, failureCallback);
			},
				(string error) => {
					MegafansUI.Instance.HideLoadingBar();
					failureCallback(error);
				});
		}

		public void VerifyOtp(string otp, string phoneNumber, Action<LoginResponse> responseCallback,
			Action<string> failureCallback, bool isRegistering) {

			OtpRequest otpRequest = new OtpRequest ();
			otpRequest.otp = otp;
			otpRequest.phone_number = phoneNumber;
            otpRequest.app_game_uid = Megafans.Instance.GameUID;

            string url = ServerBaseUrl + OtpUrl;
			Request<LoginResponse> (otpRequest, url, responseCallback, failureCallback);
		}

		public void LoginPhone(string phoneNumber, Action<LoginResponse> responseCallback,
			Action<string> failureCallback) {

			LoginRequestPhone loginRequest = new LoginRequestPhone ();
            loginRequest.appGameUid = Megafans.Instance.GameUID;
            loginRequest.phone_number = phoneNumber;
			loginRequest.device_token = DeviceInfo.DeviceToken;
			//loginRequest.device_type = DeviceInfo.DeviceType;
			loginRequest.registerType = MegafansConstants.PHONE;

			string url = ServerBaseUrl + LoginUrlPhone;
			Request<LoginResponse> (loginRequest, url, responseCallback, failureCallback);
		}

		public void LoginEmail(string email, string password, Action<LoginResponse> responseCallback,
			Action<string> failureCallback) {

			LoginRequestEmail loginRequest = new LoginRequestEmail ();
			loginRequest.email = email;
            loginRequest.appGameUid = Megafans.Instance.GameUID;
            loginRequest.password = password;
			loginRequest.device_token = DeviceInfo.DeviceToken;
			//loginRequest.device_type = DeviceInfo.DeviceType;

			string url = ServerBaseUrl + LoginUrlEmail;
			Request<LoginResponse> (loginRequest, url, responseCallback, failureCallback);
		}

		public void LoginFB(Action<LoginResponse> responseCallback, Action<string> failureCallback) {
			MegafansUI.Instance.ShowLoadingBar ();

			fbHelper.LoginRequest ((string userId, string email, string username) => {
				MegafansUI.Instance.HideLoadingBar();

				LoginRequestFB loginRequest = new LoginRequestFB();
				loginRequest.facebook_login_id = userId;
                loginRequest.appGameUid = Megafans.Instance.GameUID;
                //loginRequest.username = username;
                //loginRequest.email = email;
                loginRequest.device_token = DeviceInfo.DeviceToken;
				//loginRequest.device_type = DeviceInfo.DeviceType;
				loginRequest.registerType = MegafansConstants.FB;

				string url = ServerBaseUrl + LoginUrlFB;
				Request<LoginResponse>(loginRequest, url, responseCallback, failureCallback);
			},
				(string error) => {
					MegafansUI.Instance.HideLoadingBar();
					failureCallback(error);
				});
		}

        public void LinkFB(Action<EditProfileResponse> responseCallback, Action<string> failureCallback)
        {
            MegafansUI.Instance.ShowLoadingBar();

            fbHelper.LoginRequest((string userId, string email, string username) => {
                MegafansUI.Instance.HideLoadingBar();

                EditProfileRequest editProfileRequest = new EditProfileRequest();
                //editProfileRequest.facebook_login_id = "10216538300784796";
                editProfileRequest.facebook_login_id = userId;

                if (string.IsNullOrEmpty(MegafansPrefs.Email)) {
                    editProfileRequest.email_address = email;
                } else {
                    editProfileRequest.email_address = MegafansPrefs.Email;
                }

                editProfileRequest.username = MegafansPrefs.Username;
                editProfileRequest.phone_number = MegafansPrefs.PhoneNumber;

                string url = ServerBaseUrl + EditProfileUrl;
                Request<EditProfileResponse>(editProfileRequest, url, responseCallback, failureCallback);
            }, (string error) => {
                MegafansUI.Instance.HideLoadingBar();
                failureCallback(error);
            });
        }

        public void ResetPassword(string email, Action<Response> responseCallback,
			Action<string> failureCallback) {

			ResetPasswordRequest resetRequest = new ResetPasswordRequest ();
			resetRequest.email = email;
			//resetRequest.device_type = DeviceInfo.DeviceType;

			string url = ServerBaseUrl + ResetPasswordUrl;
			Request<Response> (resetRequest, url, responseCallback, failureCallback);
		}

		public void GetLevels(string appGameId, GameType gameType, Action<LevelsResponse> responseCallback,
			Action<string> failureCallback) {

			GetLevelsRequest levelsRequest = new GetLevelsRequest ();
			levelsRequest.app_game_uid = appGameId;
			levelsRequest.game_type_id = (int)gameType;

			string url = ServerBaseUrl + GetTournamentsUrl;
			Request<LevelsResponse> (levelsRequest, url, responseCallback, failureCallback);
		}

		public void ViewProfile(string code, Action<ViewProfileResponse> responseCallback,
			Action<string> failureCallback) {

			ViewProfileRequest profileRequest = new ViewProfileRequest ();
            profileRequest.appGameUID = Megafans.Instance.GameUID;
            bool spinnerIsHidden = true;
            if (!string.IsNullOrEmpty(code)) {
                profileRequest.code = code;
                spinnerIsHidden = false;
            }

			string url = ServerBaseUrl + ViewProfileUrl;
            Request<ViewProfileResponse> (profileRequest, url, responseCallback, failureCallback, spinnerIsHidden);
		}

		public void EditProfile(string username, string addPhoneNumber, string addEmailAddress,
            Action<EditProfileResponse> responseCallback, Action<string> failureCallback) {

			EditProfileRequest editRequest = new EditProfileRequest ();
			editRequest.username = username;
            editRequest.email_address = addEmailAddress;
            editRequest.phone_number = addPhoneNumber;
       
            string url = ServerBaseUrl + EditProfileUrl;
			Request<EditProfileResponse> (editRequest, url, responseCallback, failureCallback);
		}

		public void GetCredits(int userId, Action<CheckCreditsResponse> responseCallback,
			Action<string> failureCallback) {

			CheckCreditsRequest creditsRequest = new CheckCreditsRequest ();
			creditsRequest.userId = userId.ToString ();

			string url = ServerBaseUrl + CheckCreditsUrl;
			Request<CheckCreditsResponse> (creditsRequest, url, responseCallback, failureCallback);
		}

		public void FetchImage(string url, Action<Texture2D> successCallback, Action<string> failureCallback,
			bool isSilent = true) {

			if (string.IsNullOrEmpty(url)) {
				return;
			}

			StartCoroutine (FetchImageCo (url, successCallback, failureCallback, isSilent));
		}

		private IEnumerator FetchImageCo(string url, Action<Texture2D> successCallback,
			Action<string> failureCallback, bool isSilent) {

			if (!isSilent) {
				MegafansUI.Instance.ShowLoadingBar ();
			}

			WWW www = new WWW (url);
			yield return www;

			if (!isSilent) {
				MegafansUI.Instance.HideLoadingBar ();
			}

			if (String.IsNullOrEmpty (www.error)) {
				successCallback (www.texture);
			}
			else {
				failureCallback (www.error);
			}
		}

		public void JoinTournament(int tournamentId, Action<JoinMatchResponse> responseCallback, Action<string> failureCallback) {
			JoinMatchRequest joinRequest = new JoinMatchRequest ();
            joinRequest.tournament_id = tournamentId;
			string url = ServerBaseUrl + EnterTournamentUrl;
			Request<JoinMatchResponse> (joinRequest, url, responseCallback, failureCallback);
		}

        public void PracticeTournament(string appGameId, Action<JoinMatchResponse> responseCallback, Action<string> failureCallback) {
            JoinMatchRequest joinRequest = new JoinMatchRequest();
            joinRequest.app_game_id = appGameId;
            string url = ServerBaseUrl + EnterPracticeUrl;
            Request<JoinMatchResponse>(joinRequest, url, responseCallback, failureCallback);
        }

        public void GetTournamentRules(int tournamentId, Action<TournamentRulesResponse> responseCallback, Action<string> failureCallback)
        {
            TournamentRulesRequest rulesRequest = new TournamentRulesRequest();
            rulesRequest.tournament_id = tournamentId;
            string url = ServerBaseUrl + GetTournamentRulesUrl;
            Request<TournamentRulesResponse>(rulesRequest, url, responseCallback, failureCallback);
        }

        public void GetTermsOfUse(Action<TournamentRulesResponse> responseCallback, Action<string> failureCallback)
        {
            //TournamentRulesRequest rulesRequest = new TournamentRulesRequest();
            //rulesRequest.tournament_id = tournamentId;
            //string url = ServerBaseUrl + GetTournamentRulesUrl;
            //Request<TournamentRulesResponse>(rulesRequest, url, responseCallback, failureCallback);
        }

        public void GetPrivacyInfo(Action<TournamentRulesResponse> responseCallback, Action<string> failureCallback)
        {
            //TournamentRulesRequest rulesRequest = new TournamentRulesRequest();
            //rulesRequest.tournament_id = tournamentId;
            //string url = ServerBaseUrl + GetTournamentRulesUrl;
            //Request<TournamentRulesResponse>(rulesRequest, url, responseCallback, failureCallback);
        }

        public void SaveScore(string token, float score, Action<SaveScoreResponse> responseCallback, Action<string> failureCallback) {

			SaveScoreRequest scoreRequest = new SaveScoreRequest ();

            scoreRequest.score = score;
            scoreRequest.token = token;

			string url = ServerBaseUrl + SaveScoreUrl;
			Request<SaveScoreResponse> (scoreRequest, url, responseCallback, failureCallback);
		}

        public void ViewScoreboard(string appGameId, bool currentUserScores, GameType gameType,
			Action<ViewScoreboardResponse> responseCallback, Action<string> failureCallback, string userCode = null) {

			ViewScoreboardRequest scoreboardRequest = new ViewScoreboardRequest ();
			scoreboardRequest.app_game_uid = appGameId;

            string url = ServerBaseUrl;
            if (gameType == GameType.PRACTICE) {
                if (currentUserScores) {
                    url += ViewCurrentUserPracticeScoreboardUrl;
                } else {
                    url += ViewPracticeScoreboardUrl;
                }
            } else {
                if (currentUserScores) {
                    url += ViewCurrentUserScoreboardUrl;
                } else {
                    url += ViewScoreboardUrl;
                }
            }

            if (userCode != null) {
                scoreboardRequest.code = userCode;
            }
            Request<ViewScoreboardResponse> (scoreboardRequest, url, responseCallback, failureCallback, true);
		}

		public void ViewLeaderboard(string appGameId, GameType gameType,
			Action<ViewLeaderboardResponse> responseCallback, Action<string> failureCallback) {

			ViewLeaderboardRequest leaderboardRequest = new ViewLeaderboardRequest ();
			leaderboardRequest.app_game_uid = appGameId;
			
            string url = ServerBaseUrl;
            if (gameType == GameType.PRACTICE) {
                url += ViewPracticeLeaderboardUrl;
            } else {
                url += ViewleaderboardUrl;
            }
			Request<ViewLeaderboardResponse> (leaderboardRequest, url, responseCallback, failureCallback, true);
		}

		public void BuyTokens(string appGameId, int numberOfTokens, string transactionNumber, string productId, string receipt, Action<BuyTokensResponse> responseCallback, Action<string> failureCallback) {
			BuyTokensRequest tokensRequest = new BuyTokensRequest ();
			tokensRequest.app_game_uid = appGameId;
			tokensRequest.tokens = numberOfTokens.ToString ();		
			tokensRequest.transaction_number = transactionNumber;
            tokensRequest.receipt = receipt;
            tokensRequest.productId = productId;
            //tokensRequest.deviceType = DeviceInfo.DeviceType;

            string url = ServerBaseUrl + BuyTokensUrl;
			Request<BuyTokensResponse> (tokensRequest, url, responseCallback, failureCallback);
		}

        public void RefreshAccessToken(Action<RefreshAccessTokenResponse> responseCallback, Action<string> failureCallback) {
            RefreshAccessTokenRequest accessTokensRequest = new RefreshAccessTokenRequest();
            accessTokensRequest.refresh_token = MegafansPrefs.RefreshToken;
            accessTokensRequest.access_token = MegafansPrefs.AccessToken;
            accessTokensRequest.appGameUid = Megafans.Instance.GameUID;

            string url = ServerBaseUrl + RefreshAccessTokenUrl;
            Request<RefreshAccessTokenResponse>(accessTokensRequest, url, responseCallback, failureCallback);
        }

        public void UpdatePassword(string oldPassword, string updatedPassword, Action<UpdatePasswordResponse> responseCallback, Action<string> failureCallback)
        {
            UpdatePasswordRequest updatePasswordRequest = new UpdatePasswordRequest();
            updatePasswordRequest.old_password = oldPassword;
            updatePasswordRequest.updated_password = updatedPassword;

            string url = ServerBaseUrl + UpdatePasswordUrl;
            Request<UpdatePasswordResponse>(updatePasswordRequest, url, responseCallback, failureCallback);
        }

        public IEnumerator UploadProfilePic(Texture2D imageTex, Action<string> responseCallback, Action<string> failureCallback)
        {
            // Get image upload url
            string url = ServerBaseUrl + UploadImageUrl;
            // Get authorization token for header
            string authorization = MegafansWebService.GetBearerToken();
            // Encode texture2d to png image
            var bytes = imageTex.EncodeToPNG();

            List<IMultipartFormSection> form = new List<IMultipartFormSection>
            {
                new MultipartFormFileSection("file", bytes, "profile-pic.png", "image/png")
            };
            // generate a boundary then convert the form to byte[]
            byte[] boundary = UnityWebRequest.GenerateBoundary();
            byte[] formSections = UnityWebRequest.SerializeFormSections(form, boundary);
            // my termination string consisting of CRLF--{boundary}--
            byte[] terminate = Encoding.UTF8.GetBytes(String.Concat("\r\n--", Encoding.UTF8.GetString(boundary), "--"));
            // Make my complete body from the two byte arrays
            byte[] body = new byte[formSections.Length + terminate.Length];
            Buffer.BlockCopy(formSections, 0, body, 0, formSections.Length);
            Buffer.BlockCopy(terminate, 0, body, formSections.Length, terminate.Length);
            // Set the content type - NO QUOTES around the boundary

            string contentType = String.Concat("multipart/form-data; boundary=", Encoding.UTF8.GetString(boundary));
            //// Make my request object and add the raw body. Set anything else you need here
            UnityWebRequest www = new UnityWebRequest();
            UploadHandler uploader = new UploadHandlerRaw(body);
            string result = Convert.ToBase64String(body);

            uploader.contentType = contentType;
            www.url = url;
            www.chunkedTransfer = true;
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Authorization", authorization);
            www.uploadHandler = uploader;

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log("Form upload complete with Error!");
            }
            else
            {
                Debug.Log("Form upload complete!");
            }

            Debug.Log(www.uploadHandler.data);
        }

        //public void VerifyUserName(string userNameValue,
        //    Action<ViewLeaderboardResponse> responseCallback, Action<string> failureCallback)
        //{

        //    ViewLeaderboardRequest leaderboardRequest = new ViewLeaderboardRequest();
        //    leaderboardRequest.app_game_uid = appGameId;

        //    string url = ServerBaseUrl;
        //    if (gameType == GameType.PRACTICE)
        //    {
        //        url += ViewPracticeLeaderboardUrl;
        //    }
        //    else
        //    {
        //        url += ViewleaderboardUrl;
        //    }
        //    Request<ViewLeaderboardResponse>(leaderboardRequest, url, responseCallback, failureCallback);
        //}

        public void ResendEmailVerification(Action<Response> responseCallback,
            Action<string> failureCallback) {

            ResendVerificationEmailRequest resetRequest = new ResendVerificationEmailRequest();
            //resetRequest.device_type = DeviceInfo.DeviceType;

            string url = ServerBaseUrl + ResendVerificationEmailUrl;
            Request<Response>(resetRequest, url, responseCallback, failureCallback);
        }

        public void Logout(Action<LogoutResponse> responseCallback,
			Action<string> failureCallback) {

			LogoutRequest logoutRequest = new LogoutRequest ();
            logoutRequest.appGameUid = Megafans.Instance.GameUID;
            string url = ServerBaseUrl + LogoutUrl;
			Request<LogoutResponse> (logoutRequest, url, responseCallback, failureCallback);
		}

		public void FBLogout() {
			fbHelper.Logout ();
		}

		public static string Authenticate(string username, string password) {
			return "Basic " + System.Convert.ToBase64String(
				System.Text.Encoding.ASCII.GetBytes(username + ":" + password));
		}

        public static string GetBearerToken()
        {
            string token = MegafansPrefs.AccessToken;
            Debug.Log(MegafansPrefs.AccessToken);
            return "Bearer " + MegafansPrefs.AccessToken;
        }

    }

}