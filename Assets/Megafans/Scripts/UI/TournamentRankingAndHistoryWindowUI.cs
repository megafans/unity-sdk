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
    public class TournamentRankingAndHistoryWindowUI : MonoBehaviour, ICustomMessageTarget
    {

        [SerializeField] private Text userTokensValueTxt;
        [SerializeField] private Text headerTitleLabel;
        [SerializeField] private GameObject leaderboardItemPrefab;
        [SerializeField] private GameObject historyItemPrefab;
        [SerializeField] private ListBox rankingListBox;
        [SerializeField] private ListBox historyListBox;

        //[SerializeField] private Text scoreValueText;
        //[SerializeField] private Text levelValueText;
        [SerializeField] private GameObject verifyAcctView;
        [SerializeField] private Text verifyAcctViewTextLabel;
        [SerializeField] private Text verifyAcctViewButtonTextLabel;

        [SerializeField] private Text messageTxt;
        [SerializeField] private GameObject loadingView;
        [SerializeField] private Sprite verifyAccountIcon;

        //[SerializeField] private GameObject afterScoreImageCircle;
        //[SerializeField] private GameObject continueBtn;
        //[SerializeField] private GameObject megaFansLogo;

        public LevelsResponseData levelInfo;

        private GameType gameType = GameType.TOURNAMENT;
        private LeaderboardType leaderboardType;
        private RankingType rankingType = RankingType.RANKING;

        //private JoinMatchAssistant matchAssistant;

        //private List<Button> leaderboardOptionBtns;

        void OnEnable()
        {
            if (!MegafansPrefs.IsRegisteredMegaFansUser) {
                verifyAcctViewTextLabel.text = "Register your account to view your game history";
                verifyAcctViewButtonTextLabel.text = "Register now";
            }
            userTokensValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
            //Debug.Log("*********************LeaderBoard Enable***************************");
            //if (MegafansPrefs.UserStatus != 1 && MegafansPrefs.UserStatus != 11 && MegafansPrefs.UserStatus != 13)
            //{
            //    MegafansWebService.Instance.ViewProfile("", OnViewProfileResponse, OnViewProfileFailure);
            //}
        }

        void Start()
        {
            Debug.Log("*********************LeaderBoard Start***************************");
        }

        public void Init(GameType gameType, RankingType rankingType, LevelsResponseData levelInfo,
                         LeaderboardType leaderboardType = LeaderboardType.LEADERBOARD)
        {

            this.gameType = gameType;
            this.leaderboardType = leaderboardType;
            if (levelInfo != null) {
                this.levelInfo = levelInfo;
            }
            this.rankingType = rankingType;
            if (rankingType == RankingType.RANKING) {
                if (gameType == GameType.PRACTICE) {
                    this.headerTitleLabel.text = "Practice Ranking";
                } else {
                    this.headerTitleLabel.text = "Ranking";
                }
            } else if (rankingType == RankingType.SCORE) {
                if (gameType == GameType.PRACTICE)
                {
                    this.headerTitleLabel.text = "Practice History";
                }
                else
                {
                    this.headerTitleLabel.text = "History";
                }
            }

            //if (gameType == GameType.TOURNAMENT) {
            //  SetBtn1 ("Practice", () => {
            //      matchAssistant.JoinPracticeMatch();
            //  });

            //  SetBtn2 ("Tournament", () => {
            //      MegafansUI.Instance.ShowLevelSelection();
            //  });
            //}
            //else if (gameType == GameType.PRACTICE) {
            //  SetBtn1 ("Tournament", () => {
            //      MegafansUI.Instance.ShowLevelSelection();
            //  });

            //  SetBtn2 ("Practice", () => {
            //      matchAssistant.JoinPracticeMatch();
            //  });
            //}          
            RequestLeaderboard(gameType, leaderboardType, rankingType);
        }

        public void BackBtn_OnClick()
        {
            if (this.gameType == GameType.PRACTICE) {
                MegafansUI.Instance.ShowSinglePracticeWindow();
            } else {
                MegafansUI.Instance.ShowSingleTournamentWindow(this.levelInfo);
            }
        }

        public void VerifyAccountBtn_OnClick()
        {
            if (!MegafansPrefs.IsRegisteredMegaFansUser)
            {
                MegafansUI.Instance.ShowLandingWindow(false);
            } else {
                if (!string.IsNullOrEmpty(MegafansPrefs.Email) && !string.IsNullOrEmpty(MegafansPrefs.PhoneNumber))
                {
                    string msg = "Please select your preferred method of account verification";

                    MegafansUI.Instance.ShowAlertDialog(verifyAccountIcon, "Verify Account",
                        msg, "Email", "Phone",
                        () => {
                            MegafansUI.Instance.HideAlertDialog();
                        },
                        () => {
                            MegafansUI.Instance.HideAlertDialog();
                        });
                }
                else if (!string.IsNullOrEmpty(MegafansPrefs.Email))
                {
                    MegafansUI.Instance.ShowLoadingBar();
                    MegafansWebService.Instance.ResendEmailVerification((response) => {
                        MegafansUI.Instance.HideLoadingBar();
                        string msg = "Please check your email for a verification link.";
                        MegafansUI.Instance.ShowPopup("SUCCESS", msg);
                    }, (error) => {
                        MegafansUI.Instance.HideLoadingBar();
                        MegafansUI.Instance.ShowPopup("ERROR", error);
                    });
                }
                else if (!string.IsNullOrEmpty(MegafansPrefs.PhoneNumber))
                {
                    MegafansUI.Instance.ShowLoadingBar();
                    MegafansWebService.Instance.LoginPhone(MegafansPrefs.PhoneNumber, (response) => {
                        MegafansUI.Instance.HideLoadingBar();
                        string msg = "Please check your email for a verification link.";
                        MegafansUI.Instance.ShowPopup("SUCCESS", msg);
                    }, (error) => {
                        MegafansUI.Instance.HideLoadingBar();
                        MegafansUI.Instance.ShowPopup("ERROR", error);
                    });
                }
            }
        }

        private void RequestLeaderboard(GameType gameType, LeaderboardType leaderboardType,
            RankingType rankingType = RankingType.RANKING)
        {       
            messageTxt.gameObject.SetActive(false);

            rankingListBox.ClearList();
            historyListBox.ClearList();

            //SetHeaderAndBtns(leaderboardType, rankingType);

            if (rankingType == RankingType.SCORE)
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
            else if (rankingType == RankingType.RANKING)
            {
                //this.rankingType = rankingType;
                rankingListBox.gameObject.SetActive(true);
                historyListBox.gameObject.SetActive(false);
                verifyAcctView.SetActive(false);
                loadingView.SetActive(true);
                MegafansWebService.Instance.ViewLeaderboard(Megafans.Instance.GameUID,
                    gameType, OnLeaderboardResponse, OnLeaderboardFailure);
            }
        }

        private void OnScoreboardResponse(ViewScoreboardResponse response)
        {
            loadingView.SetActive(false);
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                if (response.data != null)
                {
                    bool isUserAlreadyDisplayed = false;
                    if (response.data.user.Count == 0)
                    {
                        if (this.gameType == GameType.PRACTICE)
                        {
                            messageTxt.text = "No practice results";
                        }
                        else
                        {
                            messageTxt.text = "No tournament results";
                        }
                        messageTxt.gameObject.SetActive(true);
                    }
                    else
                    {
                        messageTxt.gameObject.SetActive(false);
                        for (int i = 0; i < response.data.user.Count; ++i)
                        {
                            GameObject item = Instantiate(historyItemPrefab);
                            HistoryItem viewHandler = item.GetComponent<HistoryItem>();
                            if (viewHandler != null)
                            {
                                DateTime parsedDate = DateTime.Parse(response.data.user[i].created_at).ToLocalTime();
                                string dateAsString = parsedDate.Day + "/" + parsedDate.Month + "/" + parsedDate.Year + "  " + parsedDate.Hour + ":" + parsedDate.Minute + ":" + parsedDate.Second;
                                viewHandler.SetValues(response.data.user[i].score.ToString(), (i + 1).ToString(), dateAsString, response.data.user[i].code);

                                if (response.data.user[i].code == response.data.me.code)
                                {
                                    isUserAlreadyDisplayed = true;
                                    if (leaderboardType == LeaderboardType.USER_SCOREBOARD)
                                    {
                                        viewHandler.SetDateColor(Color.clear, MegafansUI.Instance.primaryColor);
                                        viewHandler.SetScoreColor(Color.clear, MegafansUI.Instance.primaryColor);
                                        viewHandler.SetPositionColor(Color.clear, MegafansUI.Instance.primaryColor);
                                    }
                                    else
                                    {
                                        viewHandler.SetDateColor(Color.white, MegafansUI.Instance.tealColor);
                                        viewHandler.SetScoreColor(Color.white, MegafansUI.Instance.tealColor);
                                        viewHandler.SetPositionColor(Color.white, MegafansUI.Instance.tealColor);
                                    }
                                }
                                else
                                {
                                    viewHandler.SetDateColor(Color.clear, MegafansUI.Instance.primaryColor);
                                    viewHandler.SetScoreColor(Color.clear, MegafansUI.Instance.primaryColor);
                                    viewHandler.SetPositionColor(Color.clear, MegafansUI.Instance.primaryColor);
                                }
                                historyListBox.AddItem(item);
                            }
                        }
                        if (response.data.me != null && response.data.me.score > 0 && !isUserAlreadyDisplayed)
                        {
                            GameObject item = Instantiate(historyItemPrefab);
                            HistoryItem viewHandler = item.GetComponent<HistoryItem>();
                            if (viewHandler != null)
                            {
                                //if (rankingType == RankingType.RANKING)
                                //{
                                //    viewHandler.SetValues(response.data.me.rank.ToString(),
                                //                           response.data.me.username, response.data.me.code);
                                //}
                                //else if (rankingType == RankingType.SCORE)
                                //{
                                DateTime parsedDate = DateTime.Parse(response.data.me.created_at).ToLocalTime();
                                string dateAsString = parsedDate.Day + "/" + parsedDate.Month + "/" + parsedDate.Year + "  " + parsedDate.Hour + ":" + parsedDate.Minute + ":" + parsedDate.Second;
                                viewHandler.SetValues(response.data.me.score.ToString(), response.data.me.rank.ToString(), dateAsString, response.data.me.code);
                                //}
                                viewHandler.SetDateColor(Color.white, MegafansUI.Instance.tealColor);
                                viewHandler.SetScoreColor(Color.white, MegafansUI.Instance.tealColor);
                                viewHandler.SetPositionColor(Color.white, MegafansUI.Instance.tealColor);

                                historyListBox.AddItem(item);
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.gameType == GameType.PRACTICE)
                {
                    messageTxt.text = "No practice results";
                }
                else
                {
                    messageTxt.text = "No tournament results";
                }
                messageTxt.gameObject.SetActive(true);
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
                    if (response.data.user.Count == 0)
                    {
                        if (this.gameType == GameType.PRACTICE)
                        {
                            messageTxt.text = "No practice results";
                        }
                        else
                        {
                            messageTxt.text = "No tournament results";
                        }
                        messageTxt.gameObject.SetActive(true);
                    }
                    else
                    {
                        for (int i = 0; i < response.data.user.Count; ++i)
                        {
                            GameObject item = Instantiate(leaderboardItemPrefab);
                            LeaderboardItem viewHandler = item.GetComponent<LeaderboardItem>();
                            if (viewHandler != null)
                            {
                                if (rankingType == RankingType.RANKING)
                                {
                                    viewHandler.SetValues(response.data.user[i].rank.ToString(),
                                                           response.data.user[i].username, response.data.user[i].code);
                                }
                                else if (rankingType == RankingType.SCORE)
                                {
                                    viewHandler.SetValues(response.data.user[i].score.ToString(),
                                                           response.data.user[i].username, response.data.user[i].code);
                                }

                                if (response.data.user[i].code == response.data.me.code)
                                {
                                    isUserAlreadyDisplayed = true;
                                    viewHandler.SetKeyColor(Color.white, MegafansUI.Instance.tealColor);
                                    viewHandler.SetValueColor(Color.white, MegafansUI.Instance.tealColor);
                                }
                                else
                                {
                                    viewHandler.SetKeyColor(Color.clear, MegafansUI.Instance.primaryColor);
                                    viewHandler.SetValueColor(Color.clear, MegafansUI.Instance.primaryColor);
                                }

                                rankingListBox.AddItem(item);
                            }
                        }
                    }

                    if (response.data.me != null && response.data.me.rank > 0 && !isUserAlreadyDisplayed)
                    {
                        GameObject item = Instantiate(leaderboardItemPrefab);
                        LeaderboardItem viewHandler = item.GetComponent<LeaderboardItem>();
                        if (viewHandler != null)
                        {
                            if (rankingType == RankingType.RANKING)
                            {
                                viewHandler.SetValues(response.data.me.rank.ToString(),
                                                       response.data.me.username, response.data.me.code);
                            }
                            else if (rankingType == RankingType.SCORE)
                            {
                                viewHandler.SetValues(response.data.me.score.ToString(),
                                                       response.data.me.username, response.data.me.code);
                            }
                            viewHandler.SetKeyColor(Color.white, MegafansUI.Instance.tealColor);
                            viewHandler.SetValueColor(Color.white, MegafansUI.Instance.tealColor);

                            rankingListBox.AddItem(item);
                        }
                    }
                }
            }
            else
            {
                if (this.gameType == GameType.PRACTICE)
                {
                    messageTxt.text = "No practice results";
                }
                else
                {
                    messageTxt.text = "No tournament results";
                }
                messageTxt.gameObject.SetActive(true);
            }
        }

        private void OnLeaderboardFailure(string error)
        {
            loadingView.SetActive(false);
            Debug.LogError(error);
        }

        private void SetHeaderAndBtns(LeaderboardType leaderboardType,
            RankingType rankingType = RankingType.RANKING)
        {

            //LeaderboardItem headerViewHandler = listBox.Header.GetComponent<LeaderboardItem>();

            //if (leaderboardType == LeaderboardType.USER_SCOREBOARD)
            //{
            //    if (headerViewHandler != null)
            //    {
            //        headerViewHandler.SetValues("SCORE", "LEVEL", "");
            //    }

            //    SelectBtn(historyBtn);
            //}
            //else if (leaderboardType == LeaderboardType.LEADERBOARD)
            //{
            //    if (rankingType == RankingType.RANKING)
            //    {
            //        if (headerViewHandler != null)
            //        {
            //            headerViewHandler.SetValues("RANK", "NAME", "");
            //        }

            //        SelectBtn(rankingBtn);
            //    }
            //    else if (rankingType == RankingType.SCORE)
            //    {
            //        if (headerViewHandler != null)
            //        {
            //            headerViewHandler.SetValues("SCORE", "NAME", "");
            //        }

            //        SelectBtn(scoreBtn);
            //    }
            //}
        }

        private void SetVerifyAcctViewVisibility()
        {
            if (MegafansPrefs.UserStatus == 1 || MegafansPrefs.UserStatus == 11 || MegafansPrefs.UserStatus == 13)
            {
                //listBox.gameObject.SetActive(true);
                verifyAcctView.SetActive(false);
                loadingView.SetActive(true);
                MegafansWebService.Instance.ViewScoreboard(Megafans.Instance.GameUID, true,
                    gameType, OnScoreboardResponse, OnScoreboardFailure);
            }
            else
            {
                //listBox.gameObject.SetActive(false);
                verifyAcctView.SetActive(true);
            }
        }

        public void ViewUserProfile(string userCode)
        {
            //if (this.leaderboardType != LeaderboardType.USER_SCOREBOARD && !string.IsNullOrEmpty(userCode))
            //{
            //    MegafansUI.Instance.ShowViewProfileWindow(userCode);
            //}
        }

        //private void OnViewProfileResponse(ViewProfileResponse response)
        //{
        //    if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
        //    {
        //        MegafansPrefs.UserStatus = response.data.status ?? 7;
        //        if (rankingType == RankingType.SCORE)
        //        {
        //            if (leaderboardType == LeaderboardType.USER_SCOREBOARD)
        //            {
        //                SetVerifyAcctViewVisibility();
        //            }
        //        }
        //    }
        //}

        //private void OnViewProfileFailure(string error)
        //{
        //    Debug.LogError(error);
        //}
    }
}