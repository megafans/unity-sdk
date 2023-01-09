#pragma warning disable 618
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;
using MegafansSDK.UI;

namespace MegafansSDK.Utils
{

    public class MegafansWebService : MonoBehaviour
    {

        private MegafansWebService() { }
        private static MegafansWebService instance = null;
        public static MegafansWebService Instance => instance;

        const string Current_Version = "/current_version";
        const string RegisterUrlEmail = "/register_usernamepassword";
        const string RegisterUrlNewUser = "/register_newuser";
        const string RegisterUrlFB = "/register_facebook";
        const string RegisterUrlPhone = "/register_phone";
        const string LoginUrlEmail = "/login";
        const string LoginUrlFB = "/login_fb";
        const string LoginUrlPhone = "/login_phone";
        const string OtpUrl = "/verifyphone";
        const string ResetPasswordUrl = "/forgot_password"; //Not updated
        const string UpdatePasswordUrl = "/change_password";
        const string GetTournamentsUrl = "/view_all_tournaments";
        const string GetTournamentsUrlV2 = "/view_all_tournaments_V2";// view all tournament v2 api
        const string ViewProfileUrl = "/view_profile";
        const string ViewProfileUrlV2 = "/view_profile_V2";
        const string EditProfileUrl = "/edit_profile"; //Not updated
        const string UploadImageUrl = "/image_upload";
        const string CheckCreditsUrl = "/show_credit";
        const string EnterTournamentUrl = "/enter_tournament";
        const string EnterTournamentUrlV2 = "/enter_tournament_V2";//enter tournament v2 api
        const string EnterPracticeUrl = "/enter_practice";
        const string SaveScoreUrl = "/save_game_scores";
        const string SaveScoreUrlNew = "/save_game_scores_new";
        const string ViewPracticeScoreboardUrl = "/practice_score_leaderboard";
        const string ViewScoreboardUrl = "/tournament_score_leaderboard";
        const string ViewCurrentUserPracticeScoreboardUrl = "/practice_my_leaderboard";
        const string ViewCurrentUserScoreboardUrl = "/my_history";//"/tournament_my_leaderboard";
        const string ViewleaderboardUrl = "/leaderboard";//" / tournament_rank_leaderboard";
        const string ViewleaderboardUrlV2 = "/leaderboard_V2";// leaderboard of tournament v2 api
        const string GetTournamentRulesUrl = "/rules";
        const string GetTournamentRulesUrlV2 = "/rules_V2";
        const string GetTermsOfUseUrl = "/TermsAndConditions";
        const string GetPrivacyPolicyUrl = "/Privacy";
        const string ViewPracticeLeaderboardUrl = "/practice_rank_leaderboard";
        const string BuyTokensUrlv2 = "/buy_tokens_v2"; //buy tokens of tournament v2 api
        const string BuyTokensUrl = "/buy_tokens";
        const string OfferwallUrl = "/ispostback";
        const string RefreshAccessTokenUrl = "/refresh";
        const string ResendVerificationEmailUrl = "/resend_confirmation_email";
        const string LogoutUrl = "/logout";
        const string LastTournamentResults = "/last_leaderboard";
        const string FreeTokensCountURL = "/ad_tokens";
        const string CheckPassword = "/checkpassword";
        const string deletAccountUrl = "/delete_user";

        //private MegafansFBHelper fbHelper;
        private bool isRefreshingToken = false;
        public VersionHandler _vH;
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

            //if (fbHelper == null)
            //{
            //    fbHelper = this.gameObject.AddComponent<MegafansFBHelper>();
            //}
        }

        void Start()
        {
            StartCoroutine(CheckLatestVersion());
        }


        private void Request<T>(Request request, string url, Action<T> responseCallback,
            Action<string> failureCallback, bool isSilent = false)
        {
            if (url != (Megafans.Instance.ServerBaseUrl() + RefreshAccessTokenUrl))
            {
                isRefreshingToken = false;
            }
            else
            {
                if (isRefreshingToken)
                {
                    MegafansUI.Instance.ShowPopup(MegafansConstants.UNAUTHORIZED_USER_ERROR_TITLE, MegafansConstants.UNAUTHORIZED_USER_ERROR_DESCRIPTION, () => MegafansUI.Instance.LogOutCurrentUser());
                    MegafansPrefs.ClearPrefs();
                    return;
                }
                else
                {
                    isRefreshingToken = true;
                }
            }
            StartCoroutine(RequestCoroutine<T>(request, url, responseCallback, failureCallback, isSilent));
        }

        private IEnumerator RequestCoroutine<T>(Request request, string url, Action<T> responseCallback,
            Action<string> failureCallback, bool isSilent)
        {

            if (!isSilent)
            {
                MegafansUI.Instance.ShowLoadingBar();
            }

            //	Debug.Log ("REQUEST:");
            //	Debug.Log (request.ToJson ());
            WWW www = request.GetWWW(url);
            yield return www;

            while (!www.isDone)
                yield return true;

            if (!isSilent)
            {
                MegafansUI.Instance.HideLoadingBar();
            }


            if (String.IsNullOrEmpty(www.error))
            {
                Debug.Log("RESPONSE:" + www.text);
                //Debug.Log(www.text);

                try
                {
                    T response = MegafansJsonHelper.FromJson<T>(www.text);
                    responseCallback(response);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());

                    string error = "Something went wrong. Please try later.";
                    failureCallback(error);
                }
            }
            else
            {
                Debug.Log("ERROR RESPONSE:");
                Debug.Log(www.error);
                string getStatusCode = Regex.Match(www.error, @"\d+").Value;
                int statusCode = int.Parse(getStatusCode);
                if (statusCode == 401)
                {
                    if (!string.IsNullOrEmpty(MegafansPrefs.AccessToken) && !string.IsNullOrEmpty(MegafansPrefs.RefreshToken))
                    {
                        this.RefreshAccessToken((RefreshAccessTokenResponse response) =>
                        {
                            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
                            {
                                if (response.data != null)
                                {
                                    MegafansPrefs.AccessToken = response.data.token;
                                    MegafansPrefs.RefreshToken = response.data.refresh;
                                    Request<T>(request, url, responseCallback, failureCallback);
                                }
                                else
                                {
                                    MegafansUI.Instance.ShowPopup(MegafansConstants.UNAUTHORIZED_USER_ERROR_TITLE, MegafansConstants.UNAUTHORIZED_USER_ERROR_DESCRIPTION, () => MegafansUI.Instance.LogOutCurrentUser());
                                    MegafansPrefs.ClearPrefs();
                                }
                            }
                            else
                            {
                                MegafansUI.Instance.ShowPopup(MegafansConstants.UNAUTHORIZED_USER_ERROR_TITLE, MegafansConstants.UNAUTHORIZED_USER_ERROR_DESCRIPTION, () => MegafansUI.Instance.LogOutCurrentUser());
                                MegafansPrefs.ClearPrefs();
                            }
                        }, (string error) =>
                        {
                            Debug.LogError(error);
                        });
                    }
                    else
                    {
                        Debug.Log("No access or refresh token found");
                    }
                }
                else if (!isSilent)
                {
                    if (isRefreshingToken)
                    {
                        MegafansUI.Instance.ShowPopup(MegafansConstants.UNAUTHORIZED_USER_ERROR_TITLE, MegafansConstants.UNAUTHORIZED_USER_ERROR_DESCRIPTION, () => MegafansUI.Instance.LogOutCurrentUser());
                    }
                    else
                    {
                        string error = "There was an error. Please check your internet connection.";
                        try
                        {
                            Response response = MegafansJsonHelper.FromJson<Response>(www.text);
                            error = response.message;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.ToString());
                        }
                        MegafansUI.Instance.ShowPopup("ERROR", error);
                    }
                }

                failureCallback(www.error);
            }
        }

        public void deleteAccount(Action<DeleteAccountResponse> responseCallback,
    Action<string> failureCallback)
        {
            DeleteAccountRequest deleteAccountRequest = new DeleteAccountRequest();
            deleteAccountRequest.appGameUid = Megafans.Instance.GameUID;
            string url = Megafans.Instance.ServerBaseUrl() + deletAccountUrl;
            Request<DeleteAccountResponse>(deleteAccountRequest, url, responseCallback, failureCallback);
        }

        public IEnumerator CheckLatestVersion()
        {
            string url = Megafans.Instance.ServerBaseUrl() + Current_Version + "?appGameUid=" + Megafans.Instance.GameUID;
            WWW www = new WWW(url);
            yield return www;

            if (String.IsNullOrEmpty(www.error))
            {
                Debug.Log("RESPONSE:");
                Debug.Log(www.text);

                try
                {
                    _vH = JsonUtility.FromJson<VersionHandler>(www.text);
                    string versionOnCloud = "";
#if UNITY_ANDROID
                    versionOnCloud = _vH.data.CurrentAndroidVersion;
                    if (int.Parse(versionOnCloud) > MegafansConstants.androidBuildVersion)
                    {
                        PlayerPrefs.SetInt("versionOnCloud", int.Parse(versionOnCloud));
                        MegafansUI.Instance.ForceUpdate();
                    }
#elif UNITY_IOS
                    versionOnCloud = _vH.data.CurrentIosVersion;
                    if (int.Parse(versionOnCloud) > MegafansConstants.IOSBuildVersion)
                    {
                        PlayerPrefs.SetInt("versionOnCloud", int.Parse(versionOnCloud));
                        MegafansUI.Instance.ForceUpdate();
                    }
#endif
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            else
            {
                Debug.Log("ERROR RESPONSE:" + www.text);
            }
        }

        public void RegisterNewUser(Action<RegisterFirstTimeResponse> responseCallback, Action<string> failureCallback)
        {

            RegisterFirstTimeUserRequest registerRequest = new RegisterFirstTimeUserRequest();
            registerRequest.device_token = DeviceInfo.DeviceToken;
            registerRequest.device_type = DeviceInfo.DeviceType;
            registerRequest.appGameUid = Megafans.Instance.GameUID;

            string url = Megafans.Instance.ServerBaseUrl() + RegisterUrlNewUser;
            Request<RegisterFirstTimeResponse>(registerRequest, url, responseCallback, failureCallback);
        }

        public void RegisterPhone(string username, string phoneNumber,
            Action<RegisterResponse> responseCallback, Action<string> failureCallback)
        {

            RegisterRequestPhone registerRequest = new RegisterRequestPhone();
            registerRequest.username = username;
            registerRequest.phone_number = phoneNumber;
            registerRequest.device_token = DeviceInfo.DeviceToken;
            //registerRequest.device_type = DeviceInfo.DeviceType;
            registerRequest.registerType = MegafansConstants.PHONE;

            string url = Megafans.Instance.ServerBaseUrl() + RegisterUrlPhone;
            Request<RegisterResponse>(registerRequest, url, responseCallback, failureCallback);
        }

        public void RegisterEmail(string email, string password,
            Action<RegisterResponse> responseCallback, Action<string> failureCallback)
        {

            RegisterRequestEmail registerRequest = new RegisterRequestEmail();
            registerRequest.email = email;
            registerRequest.password = password;
            registerRequest.device_token = DeviceInfo.DeviceToken;
            registerRequest.device_type = DeviceInfo.DeviceType;

            string url = Megafans.Instance.ServerBaseUrl() + RegisterUrlEmail;
            Request<RegisterResponse>(registerRequest, url, responseCallback, failureCallback);
        }

        //public void RegisterFB(Action<RegisterResponse> responseCallback, Action<string> failureCallback)
        //{
        //    MegafansUI.Instance.ShowLoadingBar();

        //    fbHelper.Logout();

        //    fbHelper.RegisterRequest((string userId, string email, string username, Texture2D newImageToUpload) =>
        //    {
        //        Debug.Log("Register Facebook");
        //        Debug.Log(userId);
        //        Debug.Log(username);
        //        Debug.Log(newImageToUpload);
        //        Debug.Log(email);

        //        RegisterRequestFB registerRequest = new RegisterRequestFB();
        //        registerRequest.facebook_login_id = userId;
        //        registerRequest.username = username;
        //        registerRequest.email = email;

        //        MegafansPrefs.Email = email;
        //        MegafansPrefs.Username = username;
        //        MegafansPrefs.ProfilePic = MegafansUtils.TextureToString(newImageToUpload);

        //        registerRequest.device_token = DeviceInfo.DeviceToken;
        //        registerRequest.device_type = DeviceInfo.DeviceType;
        //        registerRequest.registerType = MegafansConstants.FB;

        //        string url = Megafans.Instance.ServerBaseUrl() + RegisterUrlFB;
        //        Request<RegisterResponse>(registerRequest, url, responseCallback, failureCallback);
        //    },
        //        (string error) =>
        //        {
        //            MegafansUI.Instance.HideLoadingBar();
        //            failureCallback(error);
        //        });
        //}

        public void VerifyOtp(string otp, string phoneNumber, Action<LoginResponse> responseCallback,
            Action<string> failureCallback, bool isRegistering)
        {

            OtpRequest otpRequest = new OtpRequest();
            otpRequest.otp = otp;
            otpRequest.phone_number = phoneNumber;
            otpRequest.app_game_uid = Megafans.Instance.GameUID;

            string url = Megafans.Instance.ServerBaseUrl() + OtpUrl;
            Request<LoginResponse>(otpRequest, url, responseCallback, failureCallback);
        }

        public void LoginPhone(string phoneNumber, Action<LoginResponse> responseCallback,
            Action<string> failureCallback)
        {

            LoginRequestPhone loginRequest = new LoginRequestPhone();
            loginRequest.appGameUid = Megafans.Instance.GameUID;
            loginRequest.phone_number = phoneNumber;
            loginRequest.device_token = DeviceInfo.DeviceToken;
            //loginRequest.device_type = DeviceInfo.DeviceType;
            loginRequest.registerType = MegafansConstants.PHONE;

            string url = Megafans.Instance.ServerBaseUrl() + LoginUrlPhone;
            Request<LoginResponse>(loginRequest, url, responseCallback, failureCallback);
        }

        public void LoginEmail(string email, string password, Action<LoginResponse> responseCallback,
            Action<string> failureCallback)
        {

            LoginRequestEmail loginRequest = new LoginRequestEmail();
            loginRequest.email = email;
            loginRequest.appGameUid = Megafans.Instance.GameUID;
            loginRequest.password = password;
            loginRequest.device_token = DeviceInfo.DeviceToken;
            //loginRequest.device_type = DeviceInfo.DeviceType;

            string url = Megafans.Instance.ServerBaseUrl() + LoginUrlEmail;
            Request<LoginResponse>(loginRequest, url, responseCallback, failureCallback);
        }

        //public void LoginFB(Action<LoginResponse> responseCallback, Action<string> failureCallback)
        //{
        //    MegafansUI.Instance.ShowLoadingBar();

        //    fbHelper.LoginRequest((string userId, string email, string username) =>
        //    {
        //        MegafansUI.Instance.HideLoadingBar();

        //        LoginRequestFB loginRequest = new LoginRequestFB();
        //        loginRequest.facebook_login_id = userId;
        //        loginRequest.appGameUid = Megafans.Instance.GameUID;
        //        //loginRequest.username = username;
        //        //loginRequest.email = email;
        //        loginRequest.device_token = DeviceInfo.DeviceToken;
        //        //loginRequest.device_type = DeviceInfo.DeviceType;
        //        loginRequest.registerType = MegafansConstants.FB;

        //        string url = Megafans.Instance.ServerBaseUrl() + LoginUrlFB;
        //        Request<LoginResponse>(loginRequest, url, responseCallback, failureCallback);
        //    },
        //        (string error) =>
        //        {
        //            MegafansUI.Instance.HideLoadingBar();
        //            failureCallback(error);
        //        });
        //}

        //public void LinkFB(Action<EditProfileResponse> responseCallback, Action<string> failureCallback)
        //{
        //    MegafansUI.Instance.ShowLoadingBar();

        //    fbHelper.LoginRequest((string userId, string email, string username) =>
        //    {
        //        MegafansUI.Instance.HideLoadingBar();

        //        EditProfileRequest editProfileRequest = new EditProfileRequest();
        //        //editProfileRequest.facebook_login_id = "10216538300784796"; 
        //        editProfileRequest.facebook_login_id = userId;

        //        if (string.IsNullOrEmpty(MegafansPrefs.Email))
        //        {
        //            editProfileRequest.email_address = email;
        //        }
        //        //editProfileRequest.email_address = MegafansPrefs.Email;
        //        //}

        //        //editProfileRequest.username = MegafansPrefs.Username;
        //        //editProfileRequest.phone_number = MegafansPrefs.PhoneNumber;

        //        string url = Megafans.Instance.ServerBaseUrl() + EditProfileUrl;
        //        Request<EditProfileResponse>(editProfileRequest, url, responseCallback, failureCallback);
        //    }, (string error) =>
        //    {
        //        MegafansUI.Instance.HideLoadingBar();
        //        failureCallback(error);
        //    });
        //}

        public void ResetPassword(string email, Action<Response> responseCallback,
            Action<string> failureCallback)
        {

            ResetPasswordRequest resetRequest = new ResetPasswordRequest();
            resetRequest.email = email;
            //resetRequest.device_type = DeviceInfo.DeviceType;

            string url = Megafans.Instance.ServerBaseUrl() + ResetPasswordUrl;
            Request<Response>(resetRequest, url, responseCallback, failureCallback);
        }

        public void EditProfile(string username, string addPhoneNumber, string addEmailAddress,
            Action<EditProfileResponse> responseCallback, Action<string> failureCallback)
        {

            EditProfileRequest editRequest = new EditProfileRequest();
            editRequest.username = username;
            editRequest.email_address = addEmailAddress;
            editRequest.phone_number = addPhoneNumber;

            string url = Megafans.Instance.ServerBaseUrl() + EditProfileUrl;
            Request<EditProfileResponse>(editRequest, url, responseCallback, failureCallback);
        }

        public void GetCredits(int userId, Action<CheckCreditsResponse> responseCallback,
            Action<string> failureCallback)
        {

            CheckCreditsRequest creditsRequest = new CheckCreditsRequest();
            creditsRequest.userId = userId.ToString();

            string url = Megafans.Instance.ServerBaseUrl() + CheckCreditsUrl;
            Request<CheckCreditsResponse>(creditsRequest, url, responseCallback, failureCallback);
        }

        public void GetFreeTokensCount(int userId, Action<GetFreeTokensCountResponse> responseCallback,
                                       Action<string> failureCallback)
        {
            GetFreeTokensCountRequest creditsRequest = new GetFreeTokensCountRequest();

            string url = Megafans.Instance.ServerBaseUrl() + FreeTokensCountURL;
            Request<GetFreeTokensCountResponse>(creditsRequest, url, responseCallback, failureCallback, true);
        }

        public void GetFreeTokensCount(Action<GetFreeTokensCountResponse> responseCallback,
                                       Action<string> failureCallback)
        {
            GetFreeTokensCountRequest creditsRequest = new GetFreeTokensCountRequest();

            string url = Megafans.Instance.ServerBaseUrl() + FreeTokensCountURL;
            Request<GetFreeTokensCountResponse>(creditsRequest, url, responseCallback, failureCallback,true);
        }

        //public void GetCheckPassword(int tournamentId, string password, Action<GetPasswordCheckResponse> responseCallback, Action<string> failureCallback)
        public void GetCheckPassword(string tournamentGUID, string password, Action<GetPasswordCheckResponse> responseCallback, Action<string> failureCallback)
        {
            GetPasswordCheckRequest passwordRequest = new GetPasswordCheckRequest();

            passwordRequest.tournamentID = tournamentGUID.ToString();
            passwordRequest.password = password;

            string url = Megafans.Instance.ServerBaseUrl() + CheckPassword;
            Request<GetPasswordCheckResponse>(passwordRequest, url, responseCallback, failureCallback, true);
        }

        public void FetchImage(string url, Action<Texture2D> successCallback, Action<string> failureCallback,
            bool isSilent = true)
        {

            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            StartCoroutine(FetchImageCo(url, successCallback, failureCallback, isSilent));
        }

        public void GetLevels(string appGameId, GameType gameType, Action<LevelsResponse> responseCallback,
    Action<string> failureCallback)
        {

            GetLevelsRequest levelsRequest = new GetLevelsRequest();
            levelsRequest.app_game_uid = appGameId;
            levelsRequest.game_type_id = (int)gameType;

            string url = Megafans.Instance.ServerBaseUrl() + GetTournamentsUrlV2;
            Request<LevelsResponse>(levelsRequest, url, responseCallback, failureCallback);
        }

        public void ViewProfile(string code, Action<ViewProfileResponse> responseCallback,
            Action<string> failureCallback)
        {

            ViewProfileRequest profileRequest = new ViewProfileRequest();
            profileRequest.appGameUID = Megafans.Instance.GameUID;
            bool spinnerIsHidden = true;
            if (!string.IsNullOrEmpty(code))
            {
                profileRequest.code = code;
                spinnerIsHidden = false;
            }

            string url = Megafans.Instance.ServerBaseUrl() + ViewProfileUrlV2;
            Request<ViewProfileResponse>(profileRequest, url, responseCallback, failureCallback, spinnerIsHidden);
        }

        private IEnumerator FetchImageCo(string url, Action<Texture2D> successCallback,
            Action<string> failureCallback, bool isSilent)
        {

            if (!isSilent)
            {
                MegafansUI.Instance.ShowLoadingBar();
            }

            WWW www = new WWW(url);
            yield return www;

            while (!www.isDone)
                yield return true;

            if (!isSilent)
            {
                MegafansUI.Instance.HideLoadingBar();
            }

            if (String.IsNullOrEmpty(www.error))
            {
                successCallback(www.texture);
            }
            else
            {
                failureCallback(www.error);
            }
        }

        //public void JoinTournament(int tournamentId, string lastLocation, Action<JoinMatchResponse> responseCallback, Action<string> failureCallback)
        public void JoinTournament(string tournamentId, string lastLocation, Action<JoinMatchResponse> responseCallback, Action<string> failureCallback)
        {
            JoinMatchRequest joinRequest = new JoinMatchRequest();
            joinRequest.tournament_id = tournamentId;
            joinRequest.lastLocationValue = lastLocation;
            string url = Megafans.Instance.ServerBaseUrl() + EnterTournamentUrlV2;
            Request<JoinMatchResponse>(joinRequest, url, responseCallback, failureCallback);
        }

        public void PracticeTournament(string appGameId, Action<JoinMatchResponse> responseCallback, Action<string> failureCallback)
        {
            JoinMatchRequest joinRequest = new JoinMatchRequest();
            joinRequest.app_game_id = appGameId;
            string url = Megafans.Instance.ServerBaseUrl() + EnterPracticeUrl;
            Request<JoinMatchResponse>(joinRequest, url, responseCallback, failureCallback);
        }

        
        //public void GetTournamentRules(int tournamentId, Action<TournamentRulesResponse> responseCallback, Action<string> failureCallback)
        public void GetTournamentRules(string tournamentGUID, Action<TournamentRulesResponse> responseCallback, Action<string> failureCallback)
        {
            TournamentRulesRequest rulesRequest = new TournamentRulesRequest();
            rulesRequest.tournament_id = tournamentGUID;
            string url = Megafans.Instance.ServerBaseUrl() + GetTournamentRulesUrlV2;
            Request<TournamentRulesResponse>(rulesRequest, url, responseCallback, failureCallback);
        }

        public void GetPrivacyInfo(Action<TermsOrPrivacyPolicyResponse> responseCallback, Action<string> failureCallback)
        {
            TermsOrPrivacyPolicyRequest termsRequest = new TermsOrPrivacyPolicyRequest();
            string url = Megafans.Instance.ServerBaseUrl() + GetPrivacyPolicyUrl;
            Request<TermsOrPrivacyPolicyResponse>(termsRequest, url, responseCallback, failureCallback);
        }

        public void GetTermsOfUse(Action<TermsOrPrivacyPolicyResponse> responseCallback, Action<string> failureCallback)
        {
            TermsOrPrivacyPolicyRequest termsRequest = new TermsOrPrivacyPolicyRequest();
            string url = Megafans.Instance.ServerBaseUrl() + GetTermsOfUseUrl;
            Request<TermsOrPrivacyPolicyResponse>(termsRequest, url, responseCallback, failureCallback);
        }        

        public void SaveScoreNew(string token, float score, string lastLocation, Action<SaveScoreResponse> responseCallback, Action<string> failureCallback)
        {

            SaveScoreRequestNew scoreRequest = new SaveScoreRequestNew();
            var enc = new Encryption();
            scoreRequest.score = enc.Base64Encode(enc.EncryptString(score.ToString(), "CeilandMegaFansPassword"));
            scoreRequest.token = token;
            scoreRequest.lastLocationValue = lastLocation;

            string url = Megafans.Instance.ServerBaseUrl() + SaveScoreUrlNew;
            Request<SaveScoreResponse>(scoreRequest, url, responseCallback, failureCallback);
        }

        public void ViewScoreboard(string appGameId, bool currentUserScores, GameType gameType,
            Action<ViewScoreboardResponse> responseCallback, Action<string> failureCallback, string userCode = null)
        {

            ViewScoreboardRequest scoreboardRequest = new ViewScoreboardRequest();
            scoreboardRequest.app_game_uid = appGameId;

            string url = Megafans.Instance.ServerBaseUrl();
            if (gameType == GameType.PRACTICE)
            {
                if (currentUserScores)
                {
                    url += ViewCurrentUserPracticeScoreboardUrl;
                }
                else
                {
                    url += ViewPracticeScoreboardUrl;
                }
            }
            else
            {
                if (currentUserScores)
                {
                    url += ViewCurrentUserScoreboardUrl;
                }
                else
                {
                    url += ViewScoreboardUrl;
                }
            }

            if (userCode != null)
            {
                scoreboardRequest.code = userCode;
            }

            Request<ViewScoreboardResponse>(scoreboardRequest, url, responseCallback, failureCallback, true);
        }

        public void ViewLastTournamentResult(string leaderboardpage,
                                             string leaderboardsize,
                                             Action<LastTournamentResultResponse> responseCallback,
                                             Action<string> failureCallback)
        {
            LastTournamentResultRequest lastTournament = new LastTournamentResultRequest();

            lastTournament.leaderboardpage = leaderboardpage;
            lastTournament.leaderboardsize = leaderboardsize;

            string url = Megafans.Instance.ServerBaseUrl() + LastTournamentResults;

            Request<LastTournamentResultResponse>(lastTournament,
                                                  url,
                                                  responseCallback,
                                                  failureCallback);
        }

        public void ViewLeaderboard(string appGameId, string tournamentGUID, string leaderboardpage, string leaderboardsize, GameType gameType,
            Action<ViewLeaderboardResponse> responseCallback, Action<string> failureCallback)
        {

            ViewLeaderboardRequest leaderboardRequest = new ViewLeaderboardRequest();
            leaderboardRequest.app_game_uid = appGameId;
            leaderboardRequest.tournamentId = tournamentGUID.ToString();
            leaderboardRequest.leaderboardpage = leaderboardpage;
            leaderboardRequest.leaderboardsize = leaderboardsize;

            string url = Megafans.Instance.ServerBaseUrl() + ViewleaderboardUrlV2;

            Request<ViewLeaderboardResponse>(leaderboardRequest, url, responseCallback, failureCallback, true);
        }

        public void BuyTokens(string appGameId, int numberOfTokens, string transactionNumber, string productId, string receipt, Action<BuyTokensResponse> responseCallback, Action<string> failureCallback)
        {
            BuyTokensRequest tokensRequest = new BuyTokensRequest();
            tokensRequest.app_game_uid = appGameId;
            //tokensRequest.deviceType = DeviceInfo.DeviceType;
            tokensRequest.tokens = numberOfTokens.ToString();
            tokensRequest.receipt = receipt;
            tokensRequest.transaction_number = transactionNumber;
            tokensRequest.productId = productId;
            /*Text logText = GameObject.Find("ErrorLog").GetComponent<Text>();
            logText.text = logText.text + "\n" + tokensRequest.app_game_uid;
            //logText.text = logText.text + "\n" + tokensRequest.deviceType;
            logText.text = logText.text + "\n" + tokensRequest.tokens;
            logText.text = logText.text + "\n" + tokensRequest.transaction_number;
            logText.text = logText.text + "\n" + tokensRequest.receipt;
            logText.text = logText.text + "\n" + tokensRequest.productId;*/
            string url = Megafans.Instance.ServerBaseUrl() + BuyTokensUrlv2;
            Request<BuyTokensResponse>(tokensRequest, url, responseCallback, failureCallback);
        }

        public void OfferwallAddTokens(string appUserId, string numberOfTokens, Action<OfferwallTokensResponse> responseCallback, Action<string> failureCallback)
        {
            OfferwallTokenRequest offerwallRequest = new OfferwallTokenRequest();
            offerwallRequest.userID = appUserId;
            offerwallRequest.token = numberOfTokens;

            string url = Megafans.Instance.ServerBaseUrl() + OfferwallUrl;
            Request<OfferwallTokensResponse>(offerwallRequest, url, responseCallback, failureCallback);
        }


        public void RefreshAccessToken(Action<RefreshAccessTokenResponse> responseCallback, Action<string> failureCallback)
        {
            RefreshAccessTokenRequest accessTokensRequest = new RefreshAccessTokenRequest();
            accessTokensRequest.refresh_token = MegafansPrefs.RefreshToken;
            accessTokensRequest.access_token = MegafansPrefs.AccessToken;
            accessTokensRequest.appGameUid = Megafans.Instance.GameUID;

            string url = Megafans.Instance.ServerBaseUrl() + RefreshAccessTokenUrl;
            Request<RefreshAccessTokenResponse>(accessTokensRequest, url, responseCallback, failureCallback);
        }

        public void UpdatePassword(string oldPassword, string updatedPassword, Action<UpdatePasswordResponse> responseCallback, Action<string> failureCallback)
        {
            UpdatePasswordRequest updatePasswordRequest = new UpdatePasswordRequest();
            updatePasswordRequest.old_password = oldPassword;
            updatePasswordRequest.updated_password = updatedPassword;

            string url = Megafans.Instance.ServerBaseUrl() + UpdatePasswordUrl;
            Request<UpdatePasswordResponse>(updatePasswordRequest, url, responseCallback, failureCallback);
        }

        public IEnumerator UploadProfilePic(Texture2D imageTex, Action<string> responseCallback, Action<string> failureCallback)
        {
            // Get image upload url
            string url = Megafans.Instance.ServerBaseUrl() + UploadImageUrl;
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

            while (!www.isDone)
                yield return true;

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

        //    string url = Megafans.Instance.ServerBaseUrl();
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

        

        public void Logout(Action<LogoutResponse> responseCallback,
            Action<string> failureCallback)
        {

            LogoutRequest logoutRequest = new LogoutRequest();
            logoutRequest.appGameUid = Megafans.Instance.GameUID;
            string url = Megafans.Instance.ServerBaseUrl() + LogoutUrl;
            Request<LogoutResponse>(logoutRequest, url, responseCallback, failureCallback);
        }

        public void ResendEmailVerification(Action<Response> responseCallback,
            Action<string> failureCallback)
        {

            ResendVerificationEmailRequest resetRequest = new ResendVerificationEmailRequest();
            //resetRequest.device_type = DeviceInfo.DeviceType;

            string url = Megafans.Instance.ServerBaseUrl() + ResendVerificationEmailUrl;
            Request<Response>(resetRequest, url, responseCallback, failureCallback);
        }
        //public void FBLogout()
        //{
        //    fbHelper.Logout();
        //}

        public static string Authenticate(string username, string password)
        {
            return "Basic " + System.Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes(username + ":" + password));
        }

        public static string GetBearerToken()
        {
            string token = MegafansPrefs.AccessToken;
            Debug.Log("bearer token :" + MegafansPrefs.AccessToken);
            return "Bearer " + MegafansPrefs.AccessToken;
        }


        public void OnApplicationFocus(bool focus)
        {
            StartCoroutine(CheckLatestVersion());
        }
    }

}