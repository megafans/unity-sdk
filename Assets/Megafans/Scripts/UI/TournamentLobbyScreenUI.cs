#pragma warning disable 649
using System.Collections.Generic;
using UnityEngine;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class TournamentLobbyScreenUI : MonoBehaviour
    {

        [SerializeField] private GameObject background;
        [SerializeField] internal GameObject tournamentLobby;
        [SerializeField] private GameObject tournamentRankingAndHistoryWindow;
        [SerializeField] private GameObject editProfileWindow;
        [SerializeField] private GameObject myAccountWindow;
        [SerializeField] private GameObject updatePasswordWindow;
        [SerializeField] private GameObject leaderboardWindow;
        [SerializeField] private GameObject storeWindow;
        [SerializeField] private GameObject termsOfUseAndRulesWindow;

        private List<GameObject> windows;

        void Awake()
        {
            FillWindows();
        }

        public void ShowTournamentLobby()
        {
            ShowWindow(tournamentLobby);
            //tournamentLobby.SetActive(true);
            Debug.Log("SHOW LOBBY");
        }

        public void ShowEditProfileWindow()
        {
            ShowWindow(editProfileWindow);
        }

        public void ShowMyAccountWindow()
        {
            ShowWindow(myAccountWindow);
        }


        public void ShowUpdatePasswordWindow()
        {
            ShowWindow(updatePasswordWindow);
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

        public void ShowLeaderboardWindow(GameType gameType, RankingType rankingType, int withScore = 0, string withLevel = null, string matchId = null)
        {
            ShowWindow(leaderboardWindow);

            LeaderboardWindowUI handler = leaderboardWindow.GetComponent<LeaderboardWindowUI>();
            if (handler != null)
            {
                if (withLevel == null)
                {
                    handler.Init(gameType, rankingType, LeaderboardType.LEADERBOARD, withScore);
                }
                else
                {
                    handler.Init(gameType, rankingType, LeaderboardType.LEADERBOARD, withScore, withLevel, matchId);
                }
            }
        }

        public void ShowStoreWindow(float credits, bool addToExisting)
        {
            MegafansUI.Instance.HideHelp();
            StoreWindowUI handler = storeWindow.GetComponent<StoreWindowUI>();
            if (handler != null)
            {
                handler.Init(credits, addToExisting);
            }
            storeWindow.gameObject.SetActive(true);
        }

        public void HideStoreWindow()
        {
            UpdateTokenUI();
            storeWindow.gameObject.SetActive(false);
        }

        public void UpdateTokenUI()
        {
            TournamentLobbyUI tournamentLobbyUI = tournamentLobby.GetComponent<TournamentLobbyUI>();
            tournamentLobbyUI.UpdateCreditUI();

            LeaderboardWindowUI leaderboardWindowUI = leaderboardWindow.GetComponent<LeaderboardWindowUI>();
            leaderboardWindowUI.UpdateCreditUI();

            TournamentRankingAndHistoryWindowUI tournamentRankingAndHistoryWindowUI = tournamentRankingAndHistoryWindow.GetComponent<TournamentRankingAndHistoryWindowUI>();
            tournamentRankingAndHistoryWindowUI.UpdateCreditUI();

            MyAccountWindowUI myAccountWindowUI = myAccountWindow.GetComponent<MyAccountWindowUI>();
            myAccountWindowUI.UpdateCreditUI();

            EditProfileWindowUI editProfileWindowUI = editProfileWindow.GetComponent<EditProfileWindowUI>();
            editProfileWindowUI.UpdateCreditUI();

            UpdatePasswordWindowUI updatePasswordWindowUI = updatePasswordWindow.GetComponent<UpdatePasswordWindowUI>();
            updatePasswordWindowUI.UpdateCreditUI();

            StoreWindowUI storeWindowUI = storeWindow.GetComponent<StoreWindowUI>();
            storeWindowUI.UpdateCreditUI();

            MegafansUI.Instance.freeTokensUI.UpdateCreditUI();
        }

        public void ShowRulesWindow(LevelsResponseData levelInfo = null)
        {
            TermsOfUseAndRulesWindow handler = termsOfUseAndRulesWindow.GetComponent<TermsOfUseAndRulesWindow>();
            if (handler != null)
            {
                handler.Init(levelInfo);
            }
            termsOfUseAndRulesWindow.gameObject.SetActive(true);
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

        private void FillWindows()
        {
            if (windows == null)
            {
                windows = new List<GameObject>();
            }
            else
            {
                windows.Clear();
            }

            windows.Add(background);
            windows.Add(tournamentLobby);
            windows.Add(editProfileWindow);
            windows.Add(leaderboardWindow);
            windows.Add(storeWindow);
            windows.Add(updatePasswordWindow);
            windows.Add(myAccountWindow);
            windows.Add(tournamentRankingAndHistoryWindow);
            windows.Add(termsOfUseAndRulesWindow);
        }

        private void ShowWindow(GameObject windowToShow)
        {
            if (windowToShow != tournamentLobby)
            {
                MegafansUI.Instance.HideHelp();
            }

            foreach (GameObject window in windows)
            {
                if (window == windowToShow || window == background)
                {
                    window.SetActive(true);
                }
                else
                {
                    window.SetActive(false);
                }
            }
        }

        public void HideAllWindows()
        {
            Debug.Log("HIDE ALL WINDOWS");
            if (windows != null)
            {
                foreach (GameObject window in windows)
                {
                    window.SetActive(false);
                }
            }
        }

        public bool IsLeaderboardWindowActive()
        {
            return leaderboardWindow.activeSelf;
        }

        public void ShowCreditsWarning()
        {
            TournamentLobbyUI tournamentLobbyUI = tournamentLobby.GetComponent<TournamentLobbyUI>();
            float userCredits = MegafansPrefs.CurrentTokenBalance;
            tournamentLobbyUI.ShowCreditsWarning(userCredits);
        }
        public void ShowUnregisteredUserWarning()
        {
            TournamentLobbyUI tournamentLobbyUI = tournamentLobby.GetComponent<TournamentLobbyUI>();
            tournamentLobbyUI.ShowUnregisteredUserWarning();
        }

        public void ShowLocationServicesDeniedWarning()
        {
            TournamentLobbyUI tournamentLobbyUI = tournamentLobby.GetComponent<TournamentLobbyUI>();
            tournamentLobbyUI.ShowLocationServicesDeniedWarning();
        }
    }
}