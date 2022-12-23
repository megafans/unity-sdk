#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MegafansSDK.Utils;
using System;


namespace MegafansSDK.UI
{

    public class SingleTournamentLobbyWindow : MonoBehaviour {
    
        [SerializeField] private Text userTokensValueTxt;
        [SerializeField] private Text youCouldWinValueTxt;
        [SerializeField] private Text entryFeeValueTxt;
        [SerializeField] private Sprite coinsWarningIcon;
        [SerializeField] private TournamentCardItem tournamentCard;


        //[SerializeField] private GameObject tournamentCardItemPrefab;
        //[SerializeField] private RawImage profilePicImg;

        LevelsResponseData levelInfo;

        private float lastTokenBalance = 0.0f;

        private JoinMatchAssistant matchAssistant;

        public void Init(LevelsResponseData levelInfo)
        {
            this.levelInfo = levelInfo;
            if (this.levelInfo != null)
            {
                SetWinValueTxt();
                SetPlayNowBtnAmountTxt();
                tournamentCard.SetValues(this.levelInfo, true);
            }
            userTokensValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        void Awake()
        {
            matchAssistant = this.gameObject.AddComponent<JoinMatchAssistant>();
        }

        void OnEnable()
        {
            if (!string.IsNullOrEmpty(MegafansPrefs.AccessToken))
            {
                //MegafansWebService.Instance.ViewProfile("", OnViewProfileResponse, OnViewProfileFailure);
            }
        }

        public void PlayNowBtn_OnClick() {
            Megafans.Instance.CurrentTournamentId = levelInfo.guid;
            try
            {
                float userCredits = MegafansPrefs.CurrentTokenBalance;
                if (userCredits < levelInfo.entryFee)
                {
                    if (MegafansPrefs.IsRegisteredMegaFansUser)
                    {
                        ShowCreditsWarning(userCredits);
                    }
                    else
                    {
                        ShowUnregisteredUserWarning();
                    }
                }
                else
                {
                    matchAssistant.JoinTournamentMatch(levelInfo.guid);
                }

                //ShowCreditsWarning(userCredits);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                string error = "OOPS! Something went wrong. Please try later.";
                MegafansUI.Instance.ShowPopup("ERROR", error);
            }
        }

        public void LeaderboardBtn_OnClick()
        {
            MegafansUI.Instance.ShowSingleTournamentRankingAndHistoryWindow(GameType.TOURNAMENT, RankingType.LEADERBOARD, this.levelInfo);
        }

        public void ScoresBtn_OnClick()
        {
            MegafansUI.Instance.ShowSingleTournamentRankingAndHistoryWindow(GameType.TOURNAMENT, RankingType.HISTORY, this.levelInfo);
        }

        public void TokenBalanceBtn_OnClick()
        {
            MegafansUI.Instance.ShowStoreWindow();
        }

        public void ViewTournamentRulesBtn_OnClick()
        {
            MegafansUI.Instance.ShowRulesWindow(this.levelInfo);
        }

        public void BackBtn_OnClick()
        {
            MegafansUI.Instance.ShowTournamentLobby();
        }

        private void SetWinValueTxt()
        {
            youCouldWinValueTxt.text = levelInfo.name;
        }

        private void SetPlayNowBtnAmountTxt()
        {
            entryFeeValueTxt.text = levelInfo.entryFee.ToString();
        }

        private void OnGetCreditsSuccess(CheckCreditsResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                userTokensValueTxt.text = response.data.credits.ToString();
                lastTokenBalance = response.data.credits;
            }
        }

        private void OnGetCreditsFailure(string error)
        {
            Debug.LogError(error);
        }

        private void ShowCreditsWarning(float currentCredits)
        {
            MegafansUI.Instance.ShowAlertDialog(coinsWarningIcon, "Alert",
                MegafansConstants.NOT_ENOUGH_TOKENS_WARNING, "Buy Now", "Get <color=yellow>Free</color> Tokens",
                () => {
                    MegafansUI.Instance.HideAlertDialog();
                    MegafansUI.Instance.ShowStoreWindow();
                },
                () => {
                    MegafansUI.Instance.HideAlertDialog();
                    MegafansSDK.Megafans.Instance.m_AdsManager.ShowOfferwall();
                },
                () => {
                    MegafansUI.Instance.HideAlertDialog();
                });
        }

        private void ShowUnregisteredUserWarning()
        {
            MegafansUI.Instance.ShowAlertDialog(coinsWarningIcon, "Complete Registration",
                MegafansConstants.UNREGISTERED_USER_WARNING, "Register Now", "Later",
                () => {
                    MegafansUI.Instance.HideAlertDialog();
                    MegafansUI.Instance.ShowLandingWindow(false);
                },
                () => {
                    MegafansUI.Instance.HideAlertDialog();
                });
        }
    }

}