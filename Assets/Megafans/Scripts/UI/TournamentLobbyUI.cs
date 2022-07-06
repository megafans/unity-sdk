#pragma warning disable 649

using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using MegafansSDK.Utils;
using UnityEngine.EventSystems;
using Megafan.NativeWrapper;

namespace MegafansSDK.UI
{

    public interface TournamentCardItemCustomMessageTarget : IEventSystemHandler
    {
        void EnterTournamentBtn_OnClick(LevelsResponseData tournament);
        void ScrollViewDidFinishScrollingOnIndex(int index);
    }

    public class TournamentLobbyUI : MonoBehaviour, TournamentCardItemCustomMessageTarget
    {
        [SerializeField] private Text userTokensValueTxt;
        [SerializeField] internal ListBox listBox;
        [SerializeField] private GameObject tournamentCardItemPrefab;
        [SerializeField] private GameObject lastTournamentItemPrefab;
        [SerializeField] private ListBox lastTournamentListBox;
        [SerializeField] private TournamentPasswordUI tournamentPasswordUI;

        //Moved from Old tournament window
        [SerializeField] private Sprite coinsWarningIcon;
        //
        private JoinMatchAssistant matchAssistant;
        private Texture picPlaceholder;

        void Awake()
        {
            matchAssistant = this.gameObject.AddComponent<JoinMatchAssistant>();
        }


        public void EnterTournamentBtn_OnClick(LevelsResponseData tournament) { }

        void OnEnable()
        {
            //SetProfilePic();
            listBox.ClearList();
            if (!string.IsNullOrEmpty(MegafansPrefs.AccessToken))
            {
                //MegafansWebService.Instance.ViewProfile("", OnViewProfileResponse, OnViewProfileFailure);
                MegafansWebService.Instance.GetFreeTokensCount(OnFreeTokensCountResponse, OnFreeTokensCountFailure);
                MegafansWebService.Instance.ViewLastTournamentResult("", "",
                OnLastTournamentResponse, OnFreeTokensCountFailure);
                RequestTournaments();
            }
            userTokensValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();

            Megafan.NativeWrapper.MegafanNativeWrapper.RegisterUserWithUserId(MegafansPrefs.UserId.ToString(),
                                                          Megafans.Instance.GameUID,
                                                          Application.productName);

            Megafan.NativeWrapper.MegafanNativeWrapper.ShowIntercomIfUnreadMessages();        
        }
        private void OnFreeTokensCountResponse(GetFreeTokensCountResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                if (response.data != null)
                    MegafansUI.Instance.freeTokensUI.UpdateTokensGotGet(response.data.tokens.ToString());
            }
        }

        private void OnFreeTokensCountFailure(string error)
        {
        }

        private void OnLastTournamentResponse(LastTournamentResultResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                if (response.data != null)
                {
                    for (int i = 0; i < response.data.players.Count; ++i)
                    {
                        GameObject item = Instantiate(lastTournamentItemPrefab);
                        LeaderboardItem viewHandler = item.GetComponent<LeaderboardItem>();
                        if (viewHandler != null)
                        {
                            viewHandler.SetValues(response.data.players[i].position.ToString(),
                                                        response.data.players[i].code == response.data.me.code ? response.data.players[i].username + " (you)" : response.data.players[i].username,
                                                        response.data.players[i].score.ToString(),
                                                        response.data.players[i].code,
                                                        response.data.players[i].isCash,
                                                        true,
                                                        response.data.players[i].image,
                                                        true,
                                                        response.data.players[i].flag,
                                                        response.data.players[i].payoutAmount);

                            lastTournamentListBox.AddItem(item);
                        }
                    }

                    lastTournamentListBox.transform.parent.gameObject.SetActive(true);
                }
            }
        }

        private void OnLastTournamentFailure(string error)
        {
        }

        public void CloseLastTournamentView()
        {
            lastTournamentListBox.ClearList();
            lastTournamentListBox.transform.parent.gameObject.SetActive(false);
        }

        public void UpdateCreditUI()
        {
            userTokensValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        public void RequestTournaments()
        {
            MegafansWebService.Instance.GetLevels(Megafans.Instance.GameUID, GameType.TOURNAMENT,
                OnGetTournamentsResponse, OnGetTournamentsFailure);
        }

        public void ProfilePic_OnClick()
        {
            MegafansUI.Instance.ShowEditProfileWindow();
        }

        public void ScrollViewDidFinishScrollingOnIndex(int index)
        {
            foreach (Transform _tr in listBox.GetItem().transform)
            {
                _tr.GetComponent<TournamentCardItem>().GetPlayButton().interactable = false;
            }

            listBox.GetItem(index).GetComponent<TournamentCardItem>().GetPlayButton().interactable = true;

            CountdownTimer timer = listBox.GetItem(index).transform.GetChild(1).gameObject.GetComponent<CountdownTimer>();
            LevelsResponseData tournamentAtIndex = Megafans.Instance.m_AllTournaments[index];

            Megafans.Instance.CurrentTournamentId = tournamentAtIndex.id;

            SetCountDownTimer_ForTournament(tournamentAtIndex, timer.secondsRemaining);
        }

        private void SetCountDownTimer_ForTournament(LevelsResponseData tournament, int withSeconds = 0)
        {
        }

        public void JoinNowBtn_OnClick()
        {
            LevelsResponseData tournament = Megafans.Instance.m_AllTournaments[listBox.currentIndex];

            if (tournament.secondsLeft <= 0)
                return;
            Debug.Log(tournament.askPassword);
            if (tournament.askPassword)
            {
                tournamentPasswordUI.currentTournamentID = tournament.id;
                tournamentPasswordUI.gameObject.SetActive(true);
            }
            else
            {
                try
                {
                    matchAssistant.JoinTournamentMatch(tournament.id);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    string error = "OOPS! Something went wrong. Please try later.";
                    MegafansUI.Instance.ShowPopup("ERROR", error);
                }
            }
        }

        internal void ShowCreditsWarning(float currentCredits)
        {
            MegafansUI.Instance.ShowAlertDialog(coinsWarningIcon, "Alert",
                MegafansConstants.NOT_ENOUGH_TOKENS_WARNING, "Buy Now", "Get <color=yellow>Free</color> Tokens",
                () =>
                {
                    MegafansUI.Instance.HideAlertDialog();
                    MegafansUI.Instance.ShowStoreWindow();
                },
                () =>
                {
                    MegafansUI.Instance.HideAlertDialog();
                    MegafansSDK.Megafans.Instance.m_AdsManager.ShowOfferwall();
                },
                () =>
                {
                    MegafansUI.Instance.HideAlertDialog();
                });
        }

        internal void ShowUnregisteredUserWarning()
        {
            MegafansUI.Instance.ShowAlertDialog(coinsWarningIcon, "Complete Registration",
                MegafansConstants.UNREGISTERED_USER_WARNING, "Register Now", "Later",
                () =>
                {
                    MegafansUI.Instance.HideAlertDialog();
                    MegafansUI.Instance.ShowLandingWindow(false);
                },
                () =>
                {
                    MegafansUI.Instance.HideAlertDialog();
                });
        }

        internal void ShowLocationServicesDeniedWarning()
        {
            MegafansUI.Instance.ShowAlertDialog(coinsWarningIcon, "Location Services",
                MegafansConstants.LOCATION_SERVICES_DENIED_WARNING, "Update Now", "Later",
                () =>
                {
                    //  Navigate to settings
#if !UNITY_EDITOR
            MegafanNativeWrapper.OpenSettings();
#endif
                    MegafansUI.Instance.HideAlertDialog();
                },
                () => { },
                () => { },
                true
                );
        }

        public void LeaderboardBtn_OnClick()
        {
            LevelsResponseData tournament = Megafans.Instance.m_AllTournaments[listBox.currentIndex];

            if (tournament != null)
                MegafansUI.Instance.ShowSingleTournamentRankingAndHistoryWindow(GameType.TOURNAMENT, RankingType.LEADERBOARD, tournament);
        }

        public void ViewTournamentRulesBtn_OnClick()
        {
            LevelsResponseData tournament = Megafans.Instance.m_AllTournaments[listBox.currentIndex];

            if (tournament != null)
                MegafansUI.Instance.ShowRulesWindow(tournament);
        }

        public void ScoresBtn_OnClick()
        {
            LevelsResponseData tournament = Megafans.Instance.m_AllTournaments[listBox.currentIndex];

            if (tournament != null)
                MegafansUI.Instance.ShowSingleTournamentRankingAndHistoryWindow(GameType.TOURNAMENT, RankingType.HISTORY, tournament);
        }

        public void MyAccountBtn_OnClick()
        {
            MegafansUI.Instance.ShowMyAccountWindow();
        }

        public void PracticeLeaderboardBtn_OnClick()
        {
            MegafansUI.Instance.ShowLeaderboard(GameType.PRACTICE, RankingType.LEADERBOARD);
        }

        public void PracticeScoresBtn_OnClick()
        {
            MegafansUI.Instance.ShowLeaderboard(GameType.PRACTICE, RankingType.HISTORY);
        }

        public void OfferWallBtn_OnClick()
        {
            MegafansSDK.Megafans.Instance.m_AdsManager.ShowOfferwall();
        }

        public void NextBtn_OnClick()
        {
            listBox.next();
        }

        public void PreviousBtn_OnClick()
        {
            listBox.back();
        }

        private void FillLevels(List<LevelsResponseData> data)
        {
            listBox.SetUpForScreenCount(data.Count);

            for (int i = 0; i < data.Count; i++)
            {
                LevelsResponseData levelInfo = data[i];
                if (i == listBox.currentIndex)
                {
                    SetCountDownTimer_ForTournament(levelInfo);
                }
                GameObject tournamentItem = Instantiate(tournamentCardItemPrefab);
                TournamentCardItem viewHandler = tournamentItem.GetComponent<TournamentCardItem>();
                if (viewHandler != null)
                {
                    viewHandler.SetValues(levelInfo, true);
                    listBox.AddItem(tournamentItem);
                }
            }
        }

        private void OnViewProfileResponse(ViewProfileResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansPrefs.ProfilePicUrl = response.data.image;
                MegafansPrefs.Username = response.data.username;
                MegafansPrefs.UserStatus = response.data.status ?? 7;
                MegafansPrefs.CurrentTokenBalance = response.data.clientBalance;
                MegafansPrefs.FacebookID = response.data.facebookLoginId;

                userTokensValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
                if (response.data.email != null)
                {
                    MegafansPrefs.Email = response.data.email;
                }

                if (response.data.phoneNumber != null)
                {
                    MegafansPrefs.PhoneNumber = response.data.phoneNumber;
                }

                MegafansPrefs.IsPhoneVerified = response.data.isPhoneVerified;
            }
        }

        private void OnViewProfileFailure(string error)
        {
            Debug.LogError(error);
        }

        private void OnGetCreditsSuccess(CheckCreditsResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansPrefs.CurrentTokenBalance = response.data.credits;
                userTokensValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
            }
        }

        private void OnGetCreditsFailure(string error)
        {
            Debug.LogError(error);
        }

        private void OnGetTournamentsResponse(LevelsResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                List<LevelsResponseData> levelsData = response.data;
                if (levelsData.Count == 0 || levelsData == null)
                {
                    //messageTxt.gameObject.SetActive(true);
                    //messageTxt.text = "No tournaments running right now";
                }
                else
                {
                    //messageTxt.gameObject.SetActive(false);
                    if (Megafans.Instance.m_AllTournaments == null || Megafans.Instance.m_AllTournaments.Count == 0)
                        Megafans.Instance.m_AllTournaments = new List<LevelsResponseData>();

                    Megafans.Instance.m_AllTournaments = levelsData;

                    FillLevels(levelsData);

                    if (Megafans.Instance.CurrentTournamentId == 0)
                        ScrollViewDidFinishScrollingOnIndex(0);
                    else
                        ScrollViewDidFinishScrollingOnIndex(Megafans.Instance.GetCurrentTournamentIndex());
                }
            }
        }

        private void OnGetTournamentsFailure(string error)
        {
            Debug.LogError(error);
        }
    }

}