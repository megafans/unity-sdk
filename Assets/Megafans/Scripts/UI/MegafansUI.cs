#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MegafansSDK.Utils;
using System;

namespace MegafansSDK.UI
{

    public class MegafansUI : MonoBehaviour
    {
        private static MegafansUI instance = null;
        public static MegafansUI Instance
        {
            get
            {
                return instance;
            }
        }

        [SerializeField] private GameObject uiParent;
        [SerializeField] private GameObject eventSystemPrefab;
        [SerializeField] private OnboardingTutorialScreenUI onboardingTutorialUI;
        [SerializeField] private LandingScreenUI landingScreenUI;
        [SerializeField] internal TournamentLobbyScreenUI tournamentLobbyScreenUI;
        [SerializeField] private AlertDialogHandler alertDialogHandler;
        [SerializeField] private PopupHandler popupHandler;
        [SerializeField] private LoadingBar loadingBar;
        [SerializeField] private BuyCoinsSuccessScreen tokenPurchaseSuccessWindow;
        [SerializeField] internal FreeTokensUI freeTokensUI;
        [SerializeField] private GameObject ForceUpdateUI;

        private GameType backToLeaderboardGameType;
        private RankingType backToLeaderboardRankingType;
        public Color primaryColor;
        public Color tealColor;

        RectTransform LobbyScreenPanel;
        RectTransform LandingScreenPanel;
        RectTransform OnboardingTutorialPanel;
        Rect LastSafeArea = new Rect(0, 0, 0, 0);

        private bool UIenabled = false;
        public bool isUIenabled
        {
            get
            {
                return UIenabled;
            }
        }

        void Awake()
        {
            if (instance == null)
                instance = this;

            if (EventSystem.current == null)
            {
                Instantiate(eventSystemPrefab, this.transform);
            }

            Refresh();
        }

        private void OnEnable()
        {
            Refresh();
            if (tournamentLobbyScreenUI != null)
            {
                LobbyScreenPanel = tournamentLobbyScreenUI.transform.GetComponent<RectTransform>();
                LandingScreenPanel = landingScreenUI.transform.GetComponent<RectTransform>();
                OnboardingTutorialPanel = onboardingTutorialUI.transform.GetComponent<RectTransform>();
            }
        }

        public void Update()
        {
            if (uiParent.activeInHierarchy)
            {
                AdsManagerAPI.AdsManager.instance.redirectionButtonBanner.interactable = false;
            }
            Refresh();
        }

        void Refresh()
        {
            Rect safeArea = GetSafeArea();

            if (safeArea != LastSafeArea)
                ApplySafeArea(safeArea);
        }

        Rect GetSafeArea()
        {
            if (Screen.orientation == ScreenOrientation.Portrait)
            {
                return Screen.safeArea;
            }
            else
            {
                Rect obj = new Rect(Screen.safeArea.x, 0, Screen.safeArea.width, Screen.height);
                return obj;
            }
        }

        void ApplySafeArea(Rect r)
        {
            LastSafeArea = r;

            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            if (LobbyScreenPanel)
            {
                Debug.Log("Sohaib " + anchorMin + " " + anchorMax);
                LobbyScreenPanel.anchorMin = anchorMin;
                LobbyScreenPanel.anchorMax = anchorMax;
            }
            if (LandingScreenPanel)
            {
                LandingScreenPanel.anchorMin = anchorMin;
                LandingScreenPanel.anchorMax = anchorMax;
            }
            if (OnboardingTutorialPanel)
            {
                OnboardingTutorialPanel.anchorMin = anchorMin;
                OnboardingTutorialPanel.anchorMax = anchorMax;
            }

            Debug.LogFormat("Sohaib New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }

        public void ShowLandingWindow(bool IsLogin, bool IsLinking = false)
        {
            landingScreenUI.ShowLandingWindow(IsLogin, IsLinking);
        }

        public void HideLandingWindow()
        {
            landingScreenUI.HideAllWindows();

            UIenabled = tournamentLobbyScreenUI.IsLeaderboardWindowActive();

            uiParent.SetActive(UIenabled);
            // todo : 
            if (!UIenabled)
            {
                Debug.Log("Ashish: HideLandingWindow ");
                MegafansSDK.AdsManagerAPI.AdsManager.instance.ApiCall_Banner(needtoShowThirdPartyAds => {
                    Debug.Log("Ashish: HideLandingWindow 1"+ needtoShowThirdPartyAds);
                    if (needtoShowThirdPartyAds)
                    {
                        AdsManagerAPI.AdsManager.instance.adImage.enabled = false;
                        AdsManagerAPI.AdsManager.instance.adImage1.enabled = false;
                        Debug.Log("Ashish: HideLandingWindow 2" + needtoShowThirdPartyAds);
                        IronSource.Agent.displayBanner();
                    }
                });
            }
        }

        public void ShowRegistrationWindow(bool isEmail, bool IsLinking = false)
        {
            //tournamentLobbyScreenUI.HideAllWindows();
            if (isEmail)
            {
                landingScreenUI.ShowRegistrationWindowEmail(IsLinking);
            }
            else
            {
                landingScreenUI.ShowRegistrationWindowPhone(IsLinking);
            }
        }

        public void ShowRegistrationSuccessWindow(bool isEmail)
        {
            tournamentLobbyScreenUI.HideAllWindows();
            landingScreenUI.ShowRegistrationSuccessWindow(isEmail);
        }

        public void ShowVerifyPhoneWindow(string phoneNumberToVerify, bool isRegistering, UnityAction backBtnAction)
        {
            //tournamentLobbyScreenUI.HideAllWindows();
            landingScreenUI.ShowVerifyPhoneWindow(phoneNumberToVerify, isRegistering, backBtnAction);
        }

        public void ShowVerifyPhoneWindowFromEdit(string phoneNumberToVerify, UnityAction backBtnAction)
        {
            landingScreenUI.ShowVerifyPhoneWindowEdit(phoneNumberToVerify, backBtnAction);
        }

        public void BackFromVerifyOTP()
        {
            landingScreenUI.BackFromVerifyOTP();
        }

        public void ShowLoginWindow(bool isEmail)
        {
            onboardingTutorialUI.HideAllWindows();
            tournamentLobbyScreenUI.HideAllWindows();
            landingScreenUI.HideAllWindows();
            if (isEmail)
            {
                landingScreenUI.ShowLoginWindowEmail();
            }
            else
            {
                landingScreenUI.ShowLoginWindowPhone();
            }
        }

        public void ShowTournamentLobby()
        {
            Megafans.Instance.CheckForLocationPermissions();
            onboardingTutorialUI.HideAllWindows();
            landingScreenUI.HideAllWindows();
            tournamentLobbyScreenUI.ShowTournamentLobby();
        }

        public void ShowEditProfileWindow()
        {
            landingScreenUI.HideAllWindows();
            tournamentLobbyScreenUI.ShowEditProfileWindow();
        }

        public void ShowMyAccountWindow()
        {
            landingScreenUI.HideAllWindows();
            tournamentLobbyScreenUI.ShowMyAccountWindow();
        }

        public void ShowUpdateProfileWindow()
        {
            if (MegafansPrefs.IsRegisteredMegaFansUser)
            {
                landingScreenUI.HideAllWindows();
                tournamentLobbyScreenUI.ShowUpdatePasswordWindow();
            }
            else
            {
                onboardingTutorialUI.ShowRegisterNowMegaFansWindow();
            }
        }

        public void ShowSingleTournamentRankingAndHistoryWindow(GameType gameType, RankingType rankingType, LevelsResponseData tournament = null)
        {
            tournamentLobbyScreenUI.ShowSingleTournamentRankingAndHistoryWindow(gameType, rankingType, tournament);
        }

        public void ShowRulesWindow(LevelsResponseData levelInfo = null)
        {
            tournamentLobbyScreenUI.ShowRulesWindow(levelInfo);
        }

        public void ShowTermsOfUseOrPrivacyWindow(string informationType, bool isSignup)
        {
            if (isSignup)
            {
                landingScreenUI.ShowTermsOfUseOrPrivacyWindow(informationType);
            }
            else
            {
                tournamentLobbyScreenUI.ShowTermsOfUseOrPrivacyWindow(informationType);
            }
        }

        public void BackFromTermsOfUseOrRulesWindow()
        {
            tournamentLobbyScreenUI.HideTermsOfUseOrRulesWindow();
            landingScreenUI.HideTermsOfUseOrPrivacyWindow();
        }

        public void EnableUI(bool enable)
        {
            uiParent.SetActive(enable);
            UIenabled = enable;

            if (!enable)
            {
                onboardingTutorialUI.HideAllWindows();
                landingScreenUI.HideAllWindows();
                tournamentLobbyScreenUI.HideAllWindows();

                Debug.Log("Ashish: EnableUI ");
                MegafansSDK.AdsManagerAPI.AdsManager.instance.ApiCall_Banner(needtoShowThirdPartyAds => {
                    Debug.Log("Ashish: EnableUI1 " + needtoShowThirdPartyAds);
                    if (needtoShowThirdPartyAds)
                    {
                        //AdsManagerAPI.AdsManager.instance.adImage.enabled = false;
                        //AdsManagerAPI.AdsManager.instance.adImage1.enabled = false;
                        Debug.Log("Ashish: EnableUI2 " + needtoShowThirdPartyAds);
                        IronSource.Agent.displayBanner();
                    }
                });

            }
            else
            {
                IronSource.Agent.hideBanner();
            }
        }
        public void ShowLeaderboard(GameType gameType, RankingType rankingType)
        {
            if (MegafansPrefs.IsRegisteredMegaFansUser)
            {
                landingScreenUI.HideAllWindows();
                this.backToLeaderboardGameType = gameType;
                this.backToLeaderboardRankingType = rankingType;
                tournamentLobbyScreenUI.ShowLeaderboardWindow(gameType, rankingType);
            }
            else
            {
                onboardingTutorialUI.ShowRegisterNowMegaFansWindow();
            }
        }

        void takeThemToStore()
        {
#if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id="+Application.identifier);
#elif UNITY_IOS
            Application.OpenURL("https://apps.apple.com/us/app/subway-surfers/id1447419350");
#endif
            ForceUpdate();
        }

        internal void ForceUpdate()
        {
            if (!uiParent.activeInHierarchy)
            {
                uiParent.SetActive(true);
            }
            ShowPopup("Update Required", "This is an older version of application please update.", () => takeThemToStore());
        }

        public void ShowLeaderboardWithScore(GameType gameType, RankingType rankingType, int score, string level = null, string matchId = null)
        {
            landingScreenUI.HideAllWindows();
            backToLeaderboardGameType = gameType;
            backToLeaderboardRankingType = rankingType;
            tournamentLobbyScreenUI.ShowLeaderboardWindow(gameType, rankingType, score, level, matchId);
        }

        public void ShowStoreWindow()
        {
            if (MegafansPrefs.IsRegisteredMegaFansUser)
            {
                UnityEngine.Purchasing.ProductCollection allproducts = InAppPurchaser.Instance.GetProducts();
                tournamentLobbyScreenUI.ShowStoreWindow(MegafansPrefs.CurrentTokenBalance, false);
            }
            else
            {
                onboardingTutorialUI.ShowRegisterNowMegaFansWindow();
            }
        }

        public void BackFromStoreWindow()
        {
            tournamentLobbyScreenUI.HideStoreWindow();
        }

        public void ShowHelp()
        {

        }

        public void HideHelp()
        {

        }

        public void BackFromUserProfile()
        {
            landingScreenUI.HideAllWindows();
            tournamentLobbyScreenUI.ShowLeaderboardWindow(this.backToLeaderboardGameType, this.backToLeaderboardRankingType);
        }

        public void BackToGame()
        {
            EnableUI(false);
        }

        public void ShowAlertDialog(Sprite icon, string heading, string msg, string positiveBtnTxt,
            string negativeBtnTxt, UnityAction positiveBtnAction, UnityAction negativeBtnAction)
        {

            alertDialogHandler.ShowAlertDialog(icon, heading, msg, positiveBtnTxt, negativeBtnTxt,
                positiveBtnAction, negativeBtnAction);
        }

        public void ShowAlertDialog(Sprite icon, string heading, string msg, string positiveBtnTxt,
            string negativeBtnTxt, UnityAction positiveBtnAction, UnityAction negativeBtnAction, UnityAction closeBtnAction, bool hideCloseBtn = false)
        {

            alertDialogHandler.ShowAlertDialog(icon, heading, msg, positiveBtnTxt, negativeBtnTxt,
                positiveBtnAction, negativeBtnAction, closeBtnAction, hideCloseBtn);
        }

        public void HideAlertDialog()
        {
            alertDialogHandler.HideAlertDialog();
        }

        public void ShowBuyCoinsSuccessScreen(int tokenQuanity)
        {

            tokenPurchaseSuccessWindow.ShowBuyCoinsSuccessScreen(tokenQuanity);
        }

        public void HideBuyCoinsSuccessScreen()
        {
            tokenPurchaseSuccessWindow.HideBuyCoinsSuccessScreen();
        }

        public void ShowPopup(string heading, string msg, UnityAction okBtnAction = null)
        {
            popupHandler.ShowPopup(heading, msg, okBtnAction);
        }

        public void ShowLoadingBar()
        {
            loadingBar.ShowLoadingBar();
        }

        public void HideLoadingBar()
        {
            loadingBar.HideLoadingBar();
        }

        // Onboarding Windows

        public void ShowOnboardingStartWindow()
        {
            tournamentLobbyScreenUI.HideAllWindows();
            onboardingTutorialUI.ShowStartWindow();
        }

        public void ShowOnboardingHowWindow()
        {
            onboardingTutorialUI.ShowHowMegafansWindow();
        }

        public void ShowOnboardingPrizesWindow()
        {
            onboardingTutorialUI.ShowPrizesMegafansWindow();
        }

        public void ShowOnboardingDetailsWindow()
        {
            onboardingTutorialUI.ShowDetailsMegafansWindow();
        }

        public void ShowOnboardingBoosterWindow()
        {
            onboardingTutorialUI.ShowBoosterMegafansWindow();
        }

        public void ShowOnboardingLeaderboardWindow()
        {
            onboardingTutorialUI.ShowLeaderboardMegafansWindow();
        }

        public void ShowOnboardingFreeTokensWindow()
        {
            onboardingTutorialUI.ShowTokensMegafansWindow();
        }

        public void UpdateAllTokenTexts()
        {
            tournamentLobbyScreenUI.UpdateTokenUI();
        }

        //private void OnGUI() {
        //    if (uiParent.activeInHierarchy)
        //        GUI.enabled = false;
        //    else
        //        GUI.enabled = true;
        //}

        public void OpenIntercom()
        {
            Megafan.NativeWrapper.MegafanNativeWrapper.ShowIntercom();
        }

        public void ShowCreditsWarning()
        {
            tournamentLobbyScreenUI.ShowCreditsWarning();
        }
        public void ShowUnregisteredUserWarning()
        {
            tournamentLobbyScreenUI.ShowUnregisteredUserWarning();
        }
        public void OpenDiscord()
        {
            Application.OpenURL(Megafans.Instance.discordChannelURL);
        }
        public void ShowLocationServicesDeniedWarning()
        {
            tournamentLobbyScreenUI.ShowLocationServicesDeniedWarning();
        }

        public void LogOutCurrentUser()
        {
            MegafansWebService.Instance.Logout(OnLogoutResponse, OnLogoutFailure);            
        }

        private void OnLogoutResponse(LogoutResponse response)
        {
            //if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
            MegafansPrefs.UserId = 0;
            MegafansPrefs.ProfilePicUrl = "";
            MegafansPrefs.ClearPrefs();
            //MegafansWebService.Instance.FBLogout();
            MegafansUI.Instance.ShowOnboardingStartWindow();
            Megafan.NativeWrapper.MegafanNativeWrapper.LogoutFromIntercom();
            //}
        }

        private void OnLogoutFailure(string error)
        {
            Debug.LogError(error.ToString());
        }
    }
}