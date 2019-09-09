using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MegaFans.Unity.iOS;
using MegaFans.Unity.Android;

using MegafansSDK.Utils;

namespace MegafansSDK.UI {

    public class MegafansUI : MonoBehaviour
    {

        private MegafansUI()
        {

        }

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
        [SerializeField] private TournamentLobbyScreenUI tournamentLobbyScreenUI;
        [SerializeField] private AlertDialogHandler alertDialogHandler;
        [SerializeField] private PopupHandler popupHandler;
        [SerializeField] private LoadingBar loadingBar;
        [SerializeField] private BuyCoinsSuccessScreen tokenPurchaseSuccessWindow;

        private GameType backToLeaderboardGameType;
        private RankingType backToLeaderboardRankingType;
        public Color primaryColor;
        public Color tealColor;

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
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            if (EventSystem.current == null)
            {
                Instantiate(eventSystemPrefab, this.transform);
            }
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
            }
        }

        public void ShowLandingWindow(bool IsLogin)
        {
            onboardingTutorialUI.HideAllWindows();
            //tournamentLobbyScreenUI.HideAllWindows();
            landingScreenUI.ShowLandingWindow(IsLogin);
        }

        public void HideLandingWindow()
        {
            landingScreenUI.HideAllWindows();
        }

        public void ShowRegistrationWindow(bool isEmail)
        {
            tournamentLobbyScreenUI.HideAllWindows();
            if (isEmail)
            {
                landingScreenUI.ShowRegistrationWindowEmail();
            }
            else
            {
                landingScreenUI.ShowRegistrationWindowPhone();
            }
        }

        public void ShowRegistrationSuccessWindow(bool isEmail)
        {
            tournamentLobbyScreenUI.HideAllWindows();
            landingScreenUI.ShowRegistrationSuccessWindow(isEmail);
        }

        public void ShowVerifyPhoneWindow(string phoneNumberToVerify, bool isRegistering, UnityAction backBtnAction)
        {
            tournamentLobbyScreenUI.HideAllWindows();
            landingScreenUI.ShowVerifyPhoneWindow(phoneNumberToVerify, isRegistering, backBtnAction);
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

        public void ShowViewProfileWindow(string userCode)
        {
            landingScreenUI.HideAllWindows();
            tournamentLobbyScreenUI.ShowViewProfileWindow(userCode);
        }

        public void ShowUpdateProfileWindow()
        {
            if (MegafansPrefs.IsRegisteredMegaFansUser) {
                landingScreenUI.HideAllWindows();
                tournamentLobbyScreenUI.ShowUpdatePasswordWindow();
            } else {
                onboardingTutorialUI.ShowRegisterNowMegaFansWindow();
            }
        }

        public void ShowSingleTournamentWindow(LevelsResponseData tournament) {
            tournamentLobbyScreenUI.ShowSingleTournamentWindow(tournament);
        }

        public void ShowSinglePracticeWindow()
        {
            tournamentLobbyScreenUI.ShowSinglePracticeWindow();
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
            if (isSignup) {
                landingScreenUI.ShowTermsOfUseOrPrivacyWindow(informationType);
            } else {
                tournamentLobbyScreenUI.ShowTermsOfUseOrPrivacyWindow(informationType);
            }
        }

        public void BackFromTermsOfUseOrRulesWindow()
        {
            tournamentLobbyScreenUI.HideTermsOfUseOrRulesWindow();
            landingScreenUI.HideTermsOfUseOrPrivacyWindow();
        }

        public void ShowLeaderboard(GameType gameType, RankingType rankingType) {
            if (MegafansPrefs.IsRegisteredMegaFansUser) {
                landingScreenUI.HideAllWindows();
                this.backToLeaderboardGameType = gameType;
                this.backToLeaderboardRankingType = rankingType;
                tournamentLobbyScreenUI.ShowLeaderboardWindow(gameType, rankingType);
            } else {
                onboardingTutorialUI.ShowRegisterNowMegaFansWindow();
            }
		}

        public void ShowLeaderboardWithScore(GameType gameType, RankingType rankingType, float score, string level = null) {
            //if (MegafansPrefs.IsRegisteredMegaFansUser) {
                landingScreenUI.HideAllWindows();
                this.backToLeaderboardGameType = gameType;
                this.backToLeaderboardRankingType = rankingType;
                tournamentLobbyScreenUI.ShowLeaderboardWindow(gameType, rankingType, score, level);
            //} else {
                //onboardingTutorialUI.ShowRegisterNowMegaFansWindow();
            //}
        }

        public void ShowStoreWindow() {
            if (MegafansPrefs.IsRegisteredMegaFansUser) {
                UnityEngine.Purchasing.ProductCollection allproducts = InAppPurchaser.Instance.GetProducts();
                tournamentLobbyScreenUI.ShowStoreWindow(MegafansPrefs.CurrentTokenBalance, false);
            } else {
                onboardingTutorialUI.ShowRegisterNowMegaFansWindow();
            }
		}

        public void BackFromStoreWindow() {
            tournamentLobbyScreenUI.HideStoreWindow();
        }

        public void ShowUserProfile(string userCode)
        {
            tournamentLobbyScreenUI.ShowViewProfileWindow(userCode);
        }

        public void ShowHelp() {
#if UNITY_EDITOR
            Debug.Log("Unity Editor");
#elif UNITY_IOS
            Debug.Log("IOS");
            IntercomWrapperiOS.ShowIntercom();
#elif UNITY_ANDROID
            Debug.Log("ANDROID show intercom");
            IntercomWrapperAndroid.ShowIntercom();
#endif
        }
        
        public void HideHelp() {
#if UNITY_EDITOR
            Debug.Log("Unity Editor");
#elif UNITY_IOS
            Debug.Log("IOS");
            IntercomWrapperiOS.HideIntercom();
#elif UNITY_ANDROID
            Debug.Log("ANDROID");
            IntercomWrapperAndroid.HideIntercom();
#endif
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
			string negativeBtnTxt, UnityAction positiveBtnAction, UnityAction negativeBtnAction) {

			alertDialogHandler.ShowAlertDialog (icon, heading, msg, positiveBtnTxt, negativeBtnTxt,
				positiveBtnAction, negativeBtnAction);
		}

		public void HideAlertDialog() {
			alertDialogHandler.HideAlertDialog ();
		}

        public void ShowBuyCoinsSuccessScreen(int tokenQuanity)
        {

            tokenPurchaseSuccessWindow.ShowBuyCoinsSuccessScreen(tokenQuanity);
        }

        public void HideBuyCoinsSuccessScreen()
        {
            tokenPurchaseSuccessWindow.HideBuyCoinsSuccessScreen();
        }

        public void ShowPopup(string heading, string msg) {
			popupHandler.ShowPopup (heading, msg);
		}

		public void ShowLoadingBar() {
			loadingBar.ShowLoadingBar ();
		}

		public void HideLoadingBar() {
			loadingBar.HideLoadingBar ();
		}

        // Onboarding Windows

        public void ShowOnboardingStartWindow()
        {
            tournamentLobbyScreenUI.HideAllWindows();
            onboardingTutorialUI.ShowStartWindow();
        }

        public void ShowOnboardingHowWindow()
        {
            //landingScreenUI.HideAllWindows();
            onboardingTutorialUI.ShowHowMegafansWindow();
        }

        public void ShowOnboardingPrizesWindow()
        {
            //landingScreenUI.HideAllWindows();
            onboardingTutorialUI.ShowPrizesMegafansWindow();
        }

        public void ShowOnboardingDetailsWindow()
        {
            //landingScreenUI.HideAllWindows();
            onboardingTutorialUI.ShowDetailsMegafansWindow();
        }

    }

}