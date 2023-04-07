#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using MegafansSDK.Utils;

namespace MegafansSDK.UI
{
    public interface ICustomMessageTarget : IEventSystemHandler
    {
        void ViewUserProfile(string userCode);
    }

    public class TournamentRankingAndHistoryWindowUI : MonoBehaviour
    {
        [SerializeField] GameObject Container;
        [SerializeField] private Text userTournamentValueTxt;
        [SerializeField] private Text userCurrentValueTxt;
        [SerializeField] private Text leaderboardTitleTxt;
        [SerializeField] private Text panelTitleTxt;
        [SerializeField] private GameObject leaderboardItemPrefab;
        [SerializeField] private GameObject historyItemPrefab;
        [SerializeField] private ListBox rankingListBox;
        [SerializeField] private ListBox historyListBox;

        [SerializeField] private GameObject verifyAcctView;
        [SerializeField] private Text verifyAcctViewTextLabel;
        [SerializeField] private Text verifyAcctViewButtonTextLabel;

        [SerializeField] private GameObject loadingView;
        [SerializeField] private Sprite verifyAccountIcon;

        private GameType gameType = GameType.TOURNAMENT;
        private LeaderboardType leaderboardType;
        private RankingType rankingType = RankingType.LEADERBOARD;

        private void Start()
        {
            if (Screen.orientation != ScreenOrientation.Portrait)
            {
                Container.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                Container.transform.position = new Vector3(Container.transform.position.x, Container.transform.position.y + 34, Container.transform.position.z);
            }
        }

        void OnEnable()
        {
            if (!MegafansPrefs.IsRegisteredMegaFansUser)
            {
                verifyAcctViewTextLabel.text = "Register your account to view your game history";
                verifyAcctViewButtonTextLabel.text = "Register now";
            }
            //
            userTournamentValueTxt.text = MegafansPrefs.TournamentEntryTokens.ToString();
            userCurrentValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        public void UpdateCreditUI()
        {
            userTournamentValueTxt.text = MegafansPrefs.TournamentEntryTokens.ToString();
            userCurrentValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        public void Init(GameType gameType, RankingType rankingType, LevelsResponseData levelInfo,
                         LeaderboardType leaderboardType = LeaderboardType.LEADERBOARD)
        {

            this.gameType = gameType;

            this.leaderboardType = leaderboardType;

            if (levelInfo != null)
            {
                leaderboardTitleTxt.text = levelInfo.name;
            }

            if (rankingType == RankingType.LEADERBOARD)
            {
                panelTitleTxt.text = "LEADERBOARD";
            }
            else if (rankingType == RankingType.HISTORY)
            {
                panelTitleTxt.text = "HISTORY";
                leaderboardTitleTxt.text = "";
            }

            this.rankingType = rankingType;

            RequestLeaderboard(gameType, leaderboardType, rankingType);
        }

        public void BackBtn_OnClick()
        {
            MegafansUI.Instance.ShowTournamentLobby();
        }

        public void VerifyAccountBtn_OnClick()
        {
            if (!MegafansPrefs.IsRegisteredMegaFansUser)
            {
                MegafansUI.Instance.ShowLandingWindow(false);
            }
            else
            {
                if (!string.IsNullOrEmpty(MegafansPrefs.Email) && MegafansPrefs.IsPhoneVerified)
                {
                    string msg = "Please select your preferred method of account verification";

                    MegafansUI.Instance.ShowAlertDialog(verifyAccountIcon, "Verify Account",
                        msg, "Email", "Phone",
                        () =>
                        {
                            MegafansUI.Instance.HideAlertDialog();
                        },
                        () =>
                        {
                            MegafansUI.Instance.HideAlertDialog();
                        });
                }
                else if (!string.IsNullOrEmpty(MegafansPrefs.Email))
                {
                    MegafansUI.Instance.ShowLoadingBar();
                    MegafansWebService.Instance.ResendEmailVerification((response) =>
                    {
                        MegafansUI.Instance.HideLoadingBar();
                        string msg = "Please check your email for a verification link.";
                        MegafansUI.Instance.ShowPopup("SUCCESS", msg);
                    }, (error) =>
                    {
                        MegafansUI.Instance.HideLoadingBar();
                        MegafansUI.Instance.ShowPopup("ERROR", error);
                    });
                }
                else if (MegafansPrefs.IsPhoneVerified)
                {
                    MegafansUI.Instance.ShowLoadingBar();
                    MegafansWebService.Instance.LoginPhone(MegafansPrefs.PhoneNumber, (response) =>
                    {
                        MegafansUI.Instance.HideLoadingBar();
                        string msg = "Please check your email for a verification link.";
                        MegafansUI.Instance.ShowPopup("SUCCESS", msg);
                    }, (error) =>
                    {
                        MegafansUI.Instance.HideLoadingBar();
                        MegafansUI.Instance.ShowPopup("ERROR", error);
                    });
                }
            }
        }

        private void RequestLeaderboard(GameType gameType, LeaderboardType leaderboardType,
            RankingType rankingType = RankingType.LEADERBOARD)
        {
            rankingListBox.ClearList();
            historyListBox.ClearList();

            if (rankingType == RankingType.LEADERBOARD)
            {
                rankingListBox.gameObject.SetActive(true);
                historyListBox.gameObject.SetActive(false);
                verifyAcctView.SetActive(false);
                loadingView.SetActive(true);
                MegafansWebService.Instance.ViewLeaderboard(Megafans.Instance.GameUID,
                    Megafans.Instance.GetCurrentTournamentData().guid, "", "",
                    gameType, OnLeaderboardResponse, OnLeaderboardFailure);
            }
            else if (rankingType == RankingType.HISTORY)
            {
                if (!MegafansPrefs.IsRegisteredMegaFansUser)
                {
                    verifyAcctView.SetActive(true);
                    historyListBox.gameObject.SetActive(false);
                    rankingListBox.gameObject.SetActive(false);
                }
                else
                {
                    historyListBox.gameObject.SetActive(true);
                    rankingListBox.gameObject.SetActive(false);
                    verifyAcctView.SetActive(false);
                    loadingView.SetActive(true);
                    MegafansWebService.Instance.ViewScoreboard(Megafans.Instance.GameUID, true,
                        gameType, OnScoreboardResponse, OnScoreboardFailure);
                }
            }
        }

        private void OnScoreboardResponse(ViewScoreboardResponse response)
        {
            loadingView.SetActive(false);

            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                if (response.data != null)
                {
                    for (int i = 0; i < response.data.Count; ++i)
                    {
                        GameObject item = Instantiate(historyItemPrefab);
                        HistoryItem viewHandler = item.GetComponent<HistoryItem>();
                        if (viewHandler != null)
                        {
                            DateTime parsedDate = DateTime.Parse(response.data[i].created_at).ToLocalTime();
                            string dateAsString = parsedDate.Day + "/" + parsedDate.Month + "/" + parsedDate.Year + "  " + parsedDate.Hour + ":" + parsedDate.Minute + ":" + parsedDate.Second;
                            viewHandler.SetValues(response.data[i].score.ToString(), (i + 1).ToString(), dateAsString, response.data[i].name);//response.data.user[i].code);
                            historyListBox.AddItem(item);
                        }
                    }
                }
            }
        }

        private void OnScoreboardFailure(string error)
        {
            loadingView.SetActive(false);
            Debug.LogError(error);
        }

        private void OnLeaderboardResponse(ViewLeaderboardResponse response)
        {
            loadingView.SetActive(false);
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                if (response.data != null)
                {
                    bool isUserAlreadyDisplayed = false;

                    for (int i = 0; i < response.data.players.Count; ++i)
                    {
                        GameObject item = Instantiate(leaderboardItemPrefab);
                        LeaderboardItem viewHandler = item.GetComponent<LeaderboardItem>();
                        if (viewHandler != null)
                        {
                            if (response.data.players[i].code == response.data.me.code)
                                isUserAlreadyDisplayed = true;

                            if (rankingType == RankingType.LEADERBOARD)
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
                            }

                            rankingListBox.AddItem(item);
                        }
                    }

                    if (response.data.me != null && response.data.me.position > 0 && !isUserAlreadyDisplayed)
                    {
                        GameObject item = Instantiate(leaderboardItemPrefab);
                        LeaderboardItem viewHandler = item.GetComponent<LeaderboardItem>();
                        if (viewHandler != null)
                        {
                            if (rankingType == RankingType.LEADERBOARD)
                            {
                                viewHandler.SetValues(response.data.me.position.ToString(),
                                                       response.data.me.username + " (you)",
                                                       response.data.me.score.ToString(),
                                                       response.data.me.code,
                                                       response.data.me.isCash,
                                                       true,
                                                       response.data.me.image,
                                                       true,
                                                       response.data.me.flag,
                                                       response.data.me.payoutAmount);
                            }
                            rankingListBox.AddItem(item);
                        }
                    }
                }
            }
        }

        private void OnLeaderboardFailure(string error)
        {
            loadingView.SetActive(false);
            Debug.LogError(error);
        }

        private void SetVerifyAcctViewVisibility()
        {
            if (MegafansPrefs.UserStatus == 1 ||
                MegafansPrefs.UserStatus == 11 ||
                MegafansPrefs.UserStatus == 13)
            {
                verifyAcctView.SetActive(false);
                loadingView.SetActive(true);
                MegafansWebService.Instance.ViewLeaderboard(Megafans.Instance.GameUID,
                     Megafans.Instance.GetCurrentTournamentData().guid, "", "",
                    gameType, OnLeaderboardResponse, OnLeaderboardFailure);
            }
            else
            {
                verifyAcctView.SetActive(true);
            }
        }
    }
}