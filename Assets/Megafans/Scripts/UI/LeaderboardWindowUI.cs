#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
//using AdsManagerAPI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class LeaderboardWindowUI : MonoBehaviour
    {
        [SerializeField] Text userTokensValueTxt;
        [SerializeField] GameObject leaderboardItemPrefab;
        [SerializeField] Text scoreValueText;
        [SerializeField] Text leaderboardPanelScoreValueText;

        [SerializeField] GameObject loadingView;
        [SerializeField] GameObject GameOverScreen;
        [SerializeField] GameObject LeaderboardScreen;
        [SerializeField] GameObject HighScoreView;

        [SerializeField] GameObject DoublePointsWithAd;

        [SerializeField] GameObject WatchedDoublerAd;
        [SerializeField] GameObject HasNotWatchedDoublerAd;
        [SerializeField] GameObject FreeEntriesRemainingText;


        GameType gameType = GameType.TOURNAMENT;
        LeaderboardType leaderboardType;
        RankingType rankingType = RankingType.LEADERBOARD;

        JoinMatchAssistant matchAssistant;

        int score = 0;
        string matchId = "";
        bool hasWatchedDoublePoints = false;
        private string m_CurrentHighscore;
        private int m_CurrentScore = 0;

        void Awake()
        {
            matchAssistant = gameObject.AddComponent<JoinMatchAssistant>();
        }

        void OnEnable()
        {
        }

        public void Init(GameType _gameType, RankingType _rankingType,
            LeaderboardType _leaderboardType = LeaderboardType.LEADERBOARD,
            int withScore = 0, string withLevel = null, string _matchId = null)
        {
            int currentTournamentFreeEntries = Megafans.Instance.CurrentTounamentFreeEntries;

            userTokensValueTxt.text = "N/A";

            if (!string.IsNullOrEmpty(MegafansPrefs.AccessToken))
            {
                MegafansWebService.Instance.GetCredits(MegafansPrefs.UserId, OnGetCreditsSuccess,
                OnGetCreditsFailure);
            }

            GameOverScreen.SetActive(false);
            LeaderboardScreen.SetActive(false);
            WatchedDoublerAd.SetActive(false);
            DoublePointsWithAd.SetActive(false);
            //HasNotWatchedDoublerAd.SetActive(true);

            gameType = _gameType;
            leaderboardType = _leaderboardType;
            rankingType = _rankingType;
            matchId = _matchId;
            score = withScore;
            m_CurrentScore = score;

            //Checks if using a free entry for tournament and if so displays the number of remaining free entries
            if (Megafans.Instance.CurrentUsingFreeEntry)
            {
                if (currentTournamentFreeEntries == 1)
                {
                    FreeEntriesRemainingText.GetComponent<Text>().text = "1 Free Entry";
                }
                else
                {
                    FreeEntriesRemainingText.GetComponent<Text>().text = currentTournamentFreeEntries + " Free Entries";
                }
                FreeEntriesRemainingText.SetActive(true);
            }
            else
            {
                FreeEntriesRemainingText.SetActive(false);
            }

            if (hasWatchedDoublePoints &&
               !GameOverScreen.activeSelf)
            {
                leaderboardPanelScoreValueText.text = "Current Score: " + "<Color=Orange>" + score.ToString() + "</Color>";
                RequestLeaderboard(gameType, leaderboardType, rankingType);
            }
            else
            {
                if (withLevel != null)
                {
                    scoreValueText.text = score.ToString();
                    leaderboardPanelScoreValueText.text = "Current Score: " + "<Color=Orange>" + score.ToString() + "</Color>";
                }

                //Show Game Over Screen
                GameOverScreen.SetActive(true);

                RequestLeaderboard(gameType, leaderboardType, rankingType);
            }
        }

        //@Gunslinger - Rerwad multiplier logic
        private void RewardMultiplierLogic(int _Score)
        {
            if(!MegafansSDK.Megafans.Instance.m_AdsManager.IsRewardedVideoAvailable())
            {
                MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier = MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier;
                HideRewardButtonsAndResetMultiplier();
                return;
            }

            int _FirstMultiplier = _Score * MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier;
            int _SecondMultiplier = _FirstMultiplier + _Score;
            int _CurrentHighScore = int.Parse(m_CurrentHighscore);

            //Need To Show Both Buttons
            if (hasWatchedDoublePoints)
            {
                HideRewardButtonsAndResetMultiplier();
            }
            //Already watched add on next screen 
            else
            {
                //Current Score for this round
                if (_Score > 0)
                {
                    //Show default 2X reward multiplier
                    HasNotWatchedDoublerAd.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "x" + MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier.ToString();
                    HasNotWatchedDoublerAd.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = _FirstMultiplier.ToString();
                    HasNotWatchedDoublerAd.SetActive(true);
                    DoublePointsWithAd.transform.GetChild(1).GetComponent<Text>().text = "x" + MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier.ToString();
                    DoublePointsWithAd.transform.GetChild(2).GetComponent<Text>().text = _FirstMultiplier.ToString();
                    DoublePointsWithAd.SetActive(true);
                    WatchedDoublerAd.SetActive(false);


                    //INCENTIVE based rewards

                    //if (_Score < _CurrentHighScore && _CurrentHighScore > 0)
                    //{
                    //    MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier = MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier;

                    //    if (_CurrentHighScore >= 0 && _FirstMultiplier > _CurrentHighScore)
                    //    {
                    //        //Show First reward button
                    //        HasNotWatchedDoublerAd.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "x" + MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier.ToString();
                    //        HasNotWatchedDoublerAd.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = _FirstMultiplier.ToString();
                    //        HasNotWatchedDoublerAd.SetActive(true);
                    //        DoublePointsWithAd.transform.GetChild(1).GetComponent<Text>().text = "x" + MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier.ToString();
                    //        DoublePointsWithAd.transform.GetChild(2).GetComponent<Text>().text = _FirstMultiplier.ToString();
                    //        DoublePointsWithAd.SetActive(true);
                    //        WatchedDoublerAd.SetActive(false);
                    //    }
                    //    else if (_CurrentHighScore >= 0 && _FirstMultiplier <= _CurrentHighScore && _SecondMultiplier > _CurrentHighScore)
                    //    {
                    //        //Show Second reward button
                    //        HasNotWatchedDoublerAd.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "x" + (MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier + 1).ToString();
                    //        HasNotWatchedDoublerAd.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = _SecondMultiplier.ToString();
                    //        HasNotWatchedDoublerAd.SetActive(true);
                    //        DoublePointsWithAd.transform.GetChild(1).GetComponent<Text>().text = "x" + (MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier + 1).ToString();
                    //        DoublePointsWithAd.transform.GetChild(2).GetComponent<Text>().text = _SecondMultiplier.ToString();
                    //        DoublePointsWithAd.SetActive(true);
                    //        WatchedDoublerAd.SetActive(false);

                    //        MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier = MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier + 1;
                    //    }
                    //    else
                    //    {
                    //        //Hide reward button and leave Continue button
                    //        HideRewardButtonsAndResetMultiplier();
                    //        MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier = MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier;
                    //    }
                    //}
                    //else if (_Score >= _CurrentHighScore && _CurrentHighScore > 0)
                    //{
                    //    _FirstMultiplier = _Score * MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier;
                    //    _SecondMultiplier = _FirstMultiplier + _Score;

                    //    if (_CurrentHighScore >= 0 && _FirstMultiplier > _CurrentHighScore)
                    //    {
                    //        //Show First reward button
                    //        HasNotWatchedDoublerAd.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "x" + MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier.ToString();
                    //        HasNotWatchedDoublerAd.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = _FirstMultiplier.ToString();
                    //        HasNotWatchedDoublerAd.SetActive(true);
                    //        DoublePointsWithAd.transform.GetChild(1).GetComponent<Text>().text = "x" + MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier.ToString();
                    //        DoublePointsWithAd.transform.GetChild(2).GetComponent<Text>().text = _FirstMultiplier.ToString();
                    //        DoublePointsWithAd.SetActive(true);
                    //        WatchedDoublerAd.SetActive(false);
                    //    }
                    //}
                }
                else
                {
                    HideRewardButtonsAndResetMultiplier();
                    MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier = MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier;
                }
            }
        }

        private void HideRewardButtonsAndResetMultiplier()
        {
            WatchedDoublerAd.SetActive(true);
            HasNotWatchedDoublerAd.SetActive(false);
            DoublePointsWithAd.SetActive(false);
        }
        //

        private void OnGetCreditsFailure(string obj)
        {
            Debug.Log("Getting credits FAILED");
        }

        public void UpdateCreditUI()
        {
            userTokensValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        private void OnGetCreditsSuccess(CheckCreditsResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansPrefs.CurrentTokenBalance = response.data.credits;
                userTokensValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
            }
        }

        public void ContinueBtn_OnClick()
        {
            GameOverScreen.SetActive(false);
            LeaderboardScreen.SetActive(true);

            if (!MegafansHelper.m_Instance.m_WasPlayingTournament)
            {
                MegafansSDK.AdsManagerAPI.AdsManager.instance.ApiCall_FullScreen(needtoShowThirdPartyAds => {

                    if (needtoShowThirdPartyAds)
                    {
                        MegafansSDK.Megafans.Instance.m_AdsManager.ShowInterstitial();
                    }


                });
            }
        }

        public void PlayAgainBtn_Click()
        {
            hasWatchedDoublePoints = false;
            matchAssistant.ReplayTournamentMatch();
        }

        public void DoubleBtn_OnClick()
        {
            Megafans.Instance.m_AdsManager.m_TenginInstance.SendEvent("VideoAdForDoubleScore");

            MegafansSDK.AdsManagerAPI.AdsManager.instance.ApiCall_FullScreen(needtoShowThirdPartyAds =>
            {

                if (needtoShowThirdPartyAds)
                {
                    MegafansSDK.Megafans.Instance.m_AdsManager.ShowRewardedVideo(() =>
                    {
                        hasWatchedDoublePoints = true;

                        HasNotWatchedDoublerAd.SetActive(false);
                        WatchedDoublerAd.SetActive(false);
                        DoublePointsWithAd.SetActive(false);

                        MegafansHelper.m_Instance.SaveUserScore(score * MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier, "Level " /*+ LevelManager.Instance.currentLevel*/);

                        //MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier = Mathf.Clamp(MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier, MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier, MegafansSDK.Megafans.Instance.m_AdsManager.m_MaxMultiplier);
                        MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier = MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier;
                        Debug.Log("ShowRewardedVideo - CALLBACK FINISHED");
                    }, false);
                }
                else
                {
                    hasWatchedDoublePoints = true;

                    HasNotWatchedDoublerAd.SetActive(false);
                    WatchedDoublerAd.SetActive(false);
                    DoublePointsWithAd.SetActive(false);

                    MegafansHelper.m_Instance.SaveUserScore(score * MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier, "Level " /*+ LevelManager.Instance.currentLevel*/);

                    //MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier = Mathf.Clamp(MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier, MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier, MegafansSDK.Megafans.Instance.m_AdsManager.m_MaxMultiplier);
                    MegafansSDK.Megafans.Instance.m_AdsManager.m_VideRewardMultiplier = MegafansSDK.Megafans.Instance.m_AdsManager.m_MinMultiplier;

                    Debug.Log("ShowRewardedVideo - CALLBACK FINISHED");
                }
            });
        }

        public void BackBtn_OnClick()
        {
            Megafans.Instance.CleanTokenWhenFinishedTournament();
            hasWatchedDoublePoints = false;
            MegafansUI.Instance.EnableUI(true);
            MegafansUI.Instance.ShowTournamentLobby();
        }

        void RequestLeaderboard(GameType _gameType, LeaderboardType _leaderboardType,
            RankingType _rankingType = RankingType.LEADERBOARD)
        {
            for (int i = 0; i < HighScoreView.transform.childCount; i++)
            {
                Destroy(HighScoreView.transform.GetChild(i).gameObject);
            }

            leaderboardType = _leaderboardType;
            rankingType = _rankingType;
            gameType = _gameType;

            if (rankingType == RankingType.LEADERBOARD)
            {
                rankingType = _rankingType;
                loadingView.SetActive(true);
                MegafansWebService.Instance.ViewLeaderboard(Megafans.Instance.GameUID,
                    Megafans.Instance.GetCurrentTournamentData().id, "","",
                    gameType, OnLeaderboardResponse, OnLeaderboardFailure);
            }
        }

        void OnLeaderboardResponse(ViewLeaderboardResponse response)
        {
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

                            item.transform.SetParent(HighScoreView.transform, false);
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

                            item.transform.SetParent(HighScoreView.transform, false);
                        }
                    }

                    m_CurrentHighscore = response.data.me.score.ToString();
                }
            }

            loadingView.SetActive(false);

            //@Gunslinger - Check reward logic after leaderboard results success
            RewardMultiplierLogic(m_CurrentScore);
            //

            if (hasWatchedDoublePoints)
            {
                GameOverScreen.SetActive(false);
                LeaderboardScreen.SetActive(true);
            }
        }

        void OnLeaderboardFailure(string error)
        {
            HideRewardButtonsAndResetMultiplier();

            loadingView.SetActive(false);
            Debug.LogError(error);
        }
    }
}