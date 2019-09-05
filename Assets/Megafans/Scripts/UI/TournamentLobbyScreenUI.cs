using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MegafansSDK.Utils;
using MegaFans.Unity.iOS;
using MegaFans.Unity.Android;

namespace MegafansSDK.UI {

	public class TournamentLobbyScreenUI : MonoBehaviour
    {

		[SerializeField] private GameObject background;
		[SerializeField] private GameObject tournamentLobby;
        [SerializeField] private GameObject singleTournamentLobby;
        [SerializeField] private GameObject singlePracticeLobby;
        [SerializeField] private GameObject tournamentRankingAndHistoryWindow;
        [SerializeField] private GameObject editProfileWindow;
        [SerializeField] private GameObject myAccountWindow;
        [SerializeField] private GameObject updatePasswordWindow;
		[SerializeField] private GameObject leaderboardWindow;
        [SerializeField] private GameObject viewProfileWindow;
        [SerializeField] private GameObject storeWindow;
        [SerializeField] private GameObject termsOfUseAndRulesWindow;

        private List<GameObject> windows;

		void Awake() {
			FillWindows ();
        }

		public void ShowTournamentLobby() {
			ShowWindow (tournamentLobby);
            //tournamentLobby.SetActive(true);
            Debug.Log("SHOW LOBBY");
        }

        public void ShowEditProfileWindow() {
			ShowWindow (editProfileWindow);
		}

        public void ShowMyAccountWindow()
        {
            ShowWindow(myAccountWindow);
        }


        public void ShowUpdatePasswordWindow() {
            ShowWindow(updatePasswordWindow);
        }

        public void ShowViewProfileWindow(string userCode)
        {
            ShowWindow(viewProfileWindow);

            ViewProfileWindowUI handler = viewProfileWindow.GetComponent<ViewProfileWindowUI>();
            if (handler != null) {
                handler.Init(userCode);
            } else {
                handler.viewingUserCode = userCode;
            }
        }

        public void ShowSingleTournamentWindow(LevelsResponseData levelInfo)
        {
            SingleTournamentLobbyWindow handler = singleTournamentLobby.GetComponent<SingleTournamentLobbyWindow>();
            if (handler != null)
            {
                if (levelInfo != null) {
                    ShowWindow(singleTournamentLobby);
                    handler.Init(levelInfo);
                } else {
                    TournamentLobbyUI tournamentLobbyUI = tournamentLobby.GetComponent<TournamentLobbyUI>();
                    if (tournamentLobbyUI.tournaments != null && tournamentLobbyUI.tournaments.Count > 0) {
                        ShowWindow(singleTournamentLobby);
                        handler.Init(tournamentLobbyUI.tournaments[0]);
                    } else {
                        MegafansUI.Instance.ShowPopup("No Tournaments Running", "Sorry there are not tournaments currently running.  Check back soon!");
                    }                   
                }
            }
        }

        public void ShowSinglePracticeWindow()
        {
            ShowWindow(singlePracticeLobby);
        }

        public void ShowSingleTournamentRankingAndHistoryWindow(GameType gameType, RankingType rankingType, LevelsResponseData levelItem = null)
        {
            ShowWindow(tournamentRankingAndHistoryWindow);

            TournamentRankingAndHistoryWindowUI handler = tournamentRankingAndHistoryWindow.GetComponent<TournamentRankingAndHistoryWindowUI>();
            if (handler != null)
            {
                handler.Init(gameType, rankingType, levelItem);
            }
        }

		public void ShowLeaderboardWindow(GameType gameType, RankingType rankingType, float withScore = 0, string withLevel = null) {
			ShowWindow (leaderboardWindow);

			LeaderboardWindowUI handler = leaderboardWindow.GetComponent<LeaderboardWindowUI> ();
			if (handler != null) {
                if (withLevel == null) {
                    handler.Init(gameType, rankingType, LeaderboardType.LEADERBOARD, withScore, null);
                } else {
                    handler.Init(gameType, rankingType, LeaderboardType.LEADERBOARD, withScore, withLevel);
                }                
			}
		}

		public void ShowStoreWindow(float credits, bool addToExisting) {
            MegafansUI.Instance.HideHelp();
            storeWindow.gameObject.SetActive(true);
            StoreWindowUI handler = storeWindow.GetComponent<StoreWindowUI> ();
			if (handler != null) {
				handler.Init (credits, addToExisting);
			}
		}

        public void HideStoreWindow() {
            storeWindow.gameObject.SetActive(false);
        }

        public void ShowRulesWindow(LevelsResponseData levelInfo = null)
        {
            termsOfUseAndRulesWindow.gameObject.SetActive(true);
            TermsOfUseAndRulesWindow handler = termsOfUseAndRulesWindow.GetComponent<TermsOfUseAndRulesWindow>();
            if (handler != null)
            {
                handler.Init(levelInfo);
            }
        }

        public void ShowTermsOfUseOrPrivacyWindow(string informationType)
        {
            TermsOfUseAndRulesWindow handler = termsOfUseAndRulesWindow.GetComponent<TermsOfUseAndRulesWindow>();
            if (handler != null)
            {
                handler.Init(informationType, false);
            }
            termsOfUseAndRulesWindow.gameObject.SetActive(true);
        }

        public void HideTermsOfUseOrRulesWindow()
        {
            termsOfUseAndRulesWindow.gameObject.SetActive(false);
        }

        private void FillWindows() {
			if (windows == null) {
				windows = new List<GameObject> ();
			}
			else {
				windows.Clear ();
			}

			windows.Add (background);
			windows.Add (tournamentLobby);
			windows.Add (editProfileWindow);
            windows.Add (viewProfileWindow);
			windows.Add (leaderboardWindow);
			windows.Add (storeWindow);
            windows.Add (updatePasswordWindow);
            windows.Add (myAccountWindow);
            windows.Add (singleTournamentLobby);
            windows.Add (tournamentRankingAndHistoryWindow);
            windows.Add (singlePracticeLobby);
            windows.Add (termsOfUseAndRulesWindow);
        }

		private void ShowWindow(GameObject windowToShow) {
            if (windowToShow != tournamentLobby) {
                MegafansUI.Instance.HideHelp();
            }

			foreach (GameObject window in windows) {
				if (window == windowToShow || window == background) {
					window.SetActive (true);
				}
				else {
					window.SetActive (false);
				}
			}
		}

		public void HideAllWindows() {
            Debug.Log("HIDE ALL WINDOWS");
#if UNITY_EDITOR
            Debug.Log("Unity Editor");
#elif UNITY_IOS
            Debug.Log("IOS");
            IntercomWrapperiOS.HideIntercom();
#elif UNITY_ANDROID
            Debug.Log("ANDROID");
            IntercomWrapperAndroid.HideIntercom();
#endif
            if (windows != null) {
                foreach (GameObject window in windows)
                {
                    window.SetActive(false);
                }
            }
		}
    }
}