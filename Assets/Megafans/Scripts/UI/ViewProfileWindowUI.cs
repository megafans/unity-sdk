#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class ViewProfileWindowUI : MonoBehaviour
    {

        [SerializeField] private GameObject leaderboardItemPrefab;
        [SerializeField] private Text userNameTxt;
        [SerializeField] private Text profileWinCount;
        [SerializeField] private Text profileTournamentCount;
        [SerializeField] private Text profileHighestScore;
        [SerializeField] private Text profileTournamentType;
        [SerializeField] private RawImage profilePicImg;
        [SerializeField] private RawImage countryFlagImg;
        [SerializeField] private CustomButton tournamentsBtn;
        [SerializeField] private CustomButton practiceBtn;
        [SerializeField] private ListBox listBox;

        private Texture2D picPlaceholder;
        public string viewingUserCode;
        private GameType gameType = GameType.TOURNAMENT;
        private List<CustomButton> gameTypeOptionBtns;
        private ViewProfileData currentProfileData;

        public void Init(string code)
        {
            this.viewingUserCode = code;
            MegafansWebService.Instance.ViewProfile(viewingUserCode,
               OnViewProfileResponse, OnViewProfileFailure);
            MegafansWebService.Instance.ViewScoreboard(Megafans.Instance.GameUID, true,
                    gameType, OnScoreboardResponse, OnScoreboardFailure, this.viewingUserCode);
            SetHeader();
            if (this.currentProfileData != null)
            {
                userNameTxt.text = "";
                this.currentProfileData = null;
                countryFlagImg.texture = null;
                profilePicImg.texture = picPlaceholder;
                SetProfileStats();
            }
        }

        void Awake()
        {

            if (gameTypeOptionBtns == null)
            {
                gameTypeOptionBtns = new List<CustomButton>();
            }
            else
            {
                gameTypeOptionBtns.Clear();
            }

            gameTypeOptionBtns.Add(tournamentsBtn);
            gameTypeOptionBtns.Add(practiceBtn);
            picPlaceholder = (Texture2D)profilePicImg.texture;
        }

        public void BackBtn_OnClick()
        {
            MegafansUI.Instance.BackFromUserProfile();
        }

        public void TournamentBtn_OnClick()
        {
            if (this.gameType != GameType.TOURNAMENT)
            {
                this.gameType = GameType.TOURNAMENT;
                SelectBtn(tournamentsBtn);
                SetProfileStats();
                MegafansWebService.Instance.ViewScoreboard(Megafans.Instance.GameUID, true,
                    this.gameType, OnScoreboardResponse, OnScoreboardFailure, this.viewingUserCode);
            }
        }

        public void PracticeBtn_OnClick()
        {
            if (this.gameType != GameType.PRACTICE)
            {
                this.gameType = GameType.PRACTICE;
                SelectBtn(practiceBtn);
                SetProfileStats();
                MegafansWebService.Instance.ViewScoreboard(Megafans.Instance.GameUID, true,
                    this.gameType, OnScoreboardResponse, OnScoreboardFailure, this.viewingUserCode);
            }
        }

        private void OnViewProfileResponse(ViewProfileResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                userNameTxt.text = response.data.username;
                this.currentProfileData = response.data;
                SetProfileStats();

                MegafansWebService.Instance.FetchImage(response.data.image, OnFetchProfilePicSuccess, OnFetchProfilePicFailure, false);
                if (response.data.countryFlag != null)
                {
                    MegafansWebService.Instance.FetchImage(response.data.countryFlag, OnFetchCountryFlagSuccess, OnFetchCountryFlagFailure, false);
                }
            }
        }

        private void OnViewProfileFailure(string error)
        {
            Debug.LogError(error);
        }

        private void OnFetchProfilePicSuccess(Texture2D tex)
        {
            if (tex != null)
            {
                profilePicImg.texture = tex;
            }
        }

        private void OnFetchProfilePicFailure(string error)
        {
            Debug.LogError(error);
        }

        private void OnFetchCountryFlagSuccess(Texture2D tex)
        {
            if (tex != null)
            {
                countryFlagImg.texture = tex;
            }
        }

        private void OnFetchCountryFlagFailure(string error)
        {
            Debug.LogError(error);
        }

        private void SetHeader()
        {
              
        }

        private void SetProfileStats()
        {

            if (this.currentProfileData == null)
            {
                profileWinCount.text = "0";
                profileTournamentCount.text = "0";
                profileHighestScore.text = "0";
            }
            else
            {
                if (this.gameType == GameType.PRACTICE)
                {
                    profileTournamentType.text = "Practices";
                    if (this.currentProfileData.tournamentsWon != null)
                    {
                        profileWinCount.text = this.currentProfileData.tournamentsWon.ToString();
                    }
                    else
                    {
                        profileWinCount.text = "0";
                    }

                    if (this.currentProfileData.practiceGames != null)
                    {
                        profileTournamentCount.text = this.currentProfileData.practiceGames.ToString();
                    }
                    else
                    {
                        profileTournamentCount.text = "0";
                    }

                    if (this.currentProfileData.highestPracticeScore != null)
                    {
                        profileHighestScore.text = this.currentProfileData.highestPracticeScore.ToString();
                    }
                    else
                    {
                        profileHighestScore.text = "0";
                    }
                }
                else
                {
                    profileTournamentType.text = "Tournaments";
                    if (this.currentProfileData.tournamentsWon != null)
                    {
                        profileWinCount.text = this.currentProfileData.tournamentsWon.ToString();
                    }
                    else
                    {
                        profileWinCount.text = "0";
                    }

                    if (this.currentProfileData.tournamentsEntered != null)
                    {
                        profileTournamentCount.text = this.currentProfileData.tournamentsEntered.ToString();
                    }
                    else
                    {
                        profileTournamentCount.text = "0";
                    }

                    if (this.currentProfileData.highestTournamentScore != null)
                    {
                        profileHighestScore.text = this.currentProfileData.highestTournamentScore.ToString();
                    }
                    else
                    {
                        profileHighestScore.text = "0";
                    }
                }
            }
        }


        private void SelectBtn(CustomButton selectedBtn)
        {
            foreach (CustomButton btn in gameTypeOptionBtns)
            {
                if (btn == selectedBtn)
                {
                    btn.SelectBtn();
                }
                else
                {
                    btn.DeselectBtn();
                }
            }
        }


        private void OnScoreboardResponse(ViewScoreboardResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                if (response.data != null)
                {
                    listBox.ClearList();

                    for (int i = 0; i < response.data.Count; i++)
                    {
                        GameObject item = Instantiate(leaderboardItemPrefab);
                        LeaderboardItem viewHandler = item.GetComponent<LeaderboardItem>();
                        if (viewHandler != null)
                        {
                            /*
                            viewHandler.SetValues(response.data[i].score.ToString(),
                                this.gameType.ToString(),
                                response.data.me.score.ToString(),
                                response.data[i].code);
                              
                            listBox.AddItem(item);
                              */

                        }
                    }
                }
            }
        }

        private void OnScoreboardFailure(string error)
        {
            Debug.LogError(error);
        }

    }

}