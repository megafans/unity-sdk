#pragma warning disable 649
using UnityEngine;
using UnityEngine.UI;
using System;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class SinglePracticeLobbyWindow : MonoBehaviour
    {

        [SerializeField] private Text userTokensValueTxt;
        [SerializeField] private Image gameIconImg;

        private JoinMatchAssistant matchAssistant;

        private void Start()
        {
            gameIconImg.sprite = Megafans.Instance.GameIcon;
        }

        void Awake()
        {
            matchAssistant = this.gameObject.AddComponent<JoinMatchAssistant>();
        }

        void OnEnable()
        {
            userTokensValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        public void PlayNowBtn_OnClick()
        {
            matchAssistant.JoinPracticeMatch();
        }

        public void LeaderboardBtn_OnClick()
        {
            MegafansUI.Instance.ShowSingleTournamentRankingAndHistoryWindow(GameType.PRACTICE, RankingType.RANKING);
        }

        public void ScoresBtn_OnClick()
        {
            MegafansUI.Instance.ShowSingleTournamentRankingAndHistoryWindow(GameType.PRACTICE, RankingType.SCORE);
        }

        public void TokenBalanceBtn_OnClick()
        {
            MegafansUI.Instance.ShowStoreWindow();
        }

        public void BackBtn_OnClick()
        {
            MegafansUI.Instance.ShowTournamentLobby();
        }

        public void JoinTournamentBtn_OnClick()
        {
            MegafansUI.Instance.ShowSingleTournamentWindow(null);
        }
    }
}