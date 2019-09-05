using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using MegafansSDK.Utils;

namespace MegafansSDK.UI {

    public interface ICustomMessageTarget : IEventSystemHandler
    {
        void ViewUserProfile(string userCode);

    }

    public class LeaderboardWindowUI : MonoBehaviour, ICustomMessageTarget
    {

        [SerializeField] private GameObject leaderboardItemPrefab;
        [SerializeField] private ListBox listBox;
        [SerializeField] private Button rankingBtn;
        [SerializeField] private Button scoreBtn;
        [SerializeField] private Button historyBtn;
        //[SerializeField] private Button continueBtn;
        //[SerializeField] private Button btn2;
        [SerializeField] private Text scoreValueText;
        [SerializeField] private Text levelValueText;
        [SerializeField] private GameObject verifyAcctView;
        [SerializeField] private Text messageTxt;
        [SerializeField] private GameObject loadingView;
        [SerializeField] private Sprite verifyAccountIcon;

        [SerializeField] private GameObject afterScoreImageCircle;
        [SerializeField] private GameObject continueBtn;
        [SerializeField] private GameObject megaFansLogo;

        private GameType gameType = GameType.TOURNAMENT;
        private LeaderboardType leaderboardType;
        private RankingType rankingType = RankingType.RANKING;

        private JoinMatchAssistant matchAssistant;

        private List<Button> leaderboardOptionBtns;

        void Awake() {
			matchAssistant = this.gameObject.AddComponent<JoinMatchAssistant> ();

			if (leaderboardOptionBtns == null) {
				leaderboardOptionBtns = new List<Button> ();
			}
			else {
				leaderboardOptionBtns.Clear ();
			}

			leaderboardOptionBtns.Add (rankingBtn);
			leaderboardOptionBtns.Add (scoreBtn);
			leaderboardOptionBtns.Add (historyBtn);
            Debug.Log("*********************LeaderBoard Awake***************************");
		}

		void OnEnable() {
            Debug.Log("*********************LeaderBoard Enable***************************");
            if (!MegafansPrefs.IsRegisteredMegaFansUser)
            {
                MegafansWebService.Instance.ViewProfile("", OnViewProfileResponse, OnViewProfileFailure);
            }
        }

		void Start() {
            Debug.Log("*********************LeaderBoard Start***************************");
        }

		public void Init(GameType gameType, RankingType rankingType,
                         LeaderboardType leaderboardType = LeaderboardType.LEADERBOARD, float withScore = 0, string withLevel = null) {

			this.gameType = gameType;
            this.leaderboardType = leaderboardType;
            //if (gameType == GameType.TOURNAMENT) {
            //	SetBtn1 ("Practice", () => {
            //		matchAssistant.JoinPracticeMatch();
            //	});

            //	SetBtn2 ("Tournament", () => {
            //		MegafansUI.Instance.ShowLevelSelection();
            //	});
            //}
            //else if (gameType == GameType.PRACTICE) {
            //	SetBtn1 ("Tournament", () => {
            //		MegafansUI.Instance.ShowLevelSelection();
            //	});

            //	SetBtn2 ("Practice", () => {
            //		matchAssistant.JoinPracticeMatch();
            //	});
            //}
            if (withLevel == null)
            {
                afterScoreImageCircle.SetActive(false);
                megaFansLogo.SetActive(true);
                continueBtn.SetActive(false);
            }
            else
            {
                afterScoreImageCircle.SetActive(true);
                megaFansLogo.SetActive(false);
                continueBtn.SetActive(true);

                scoreValueText.text = withScore.ToString();
                if (withLevel != null)
                {
                    levelValueText.text = withLevel;
                }
                else
                {
                    levelValueText.text = "Level 1";
                }
            }

			RequestLeaderboard (gameType, leaderboardType, rankingType);
		}

        public void ContinueBtn_OnClick() {
            MegafansUI.Instance.EnableUI(true);
            MegafansUI.Instance.ShowTournamentLobby();
        }

        public void VerifyAccountBtn_OnClick()
        {
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
                },(error) => {
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

        public void ReplayBtn_OnClick() {
            if (gameType == GameType.TOURNAMENT) {
                matchAssistant.ReplayTournamentMatch();
            } else {
                matchAssistant.JoinPracticeMatch();
            }
        }

        public void BackBtn_OnClick() {
			MegafansUI.Instance.ShowTournamentLobby ();
		}

		public void RankingBtn_OnClick() {
			RequestLeaderboard (gameType, LeaderboardType.LEADERBOARD, RankingType.RANKING);
		}

		public void ScoreBtn_OnClick() {
			RequestLeaderboard (gameType, LeaderboardType.LEADERBOARD, RankingType.SCORE);
		}

		public void HistoryBtn_OnClick() {
			RequestLeaderboard (gameType, LeaderboardType.USER_SCOREBOARD, RankingType.SCORE);
		}

		private void RequestLeaderboard(GameType gameType, LeaderboardType leaderboardType,
			RankingType rankingType = RankingType.RANKING) {
            this.leaderboardType = leaderboardType;
            this.rankingType = rankingType;
            this.gameType = gameType;
            messageTxt.gameObject.SetActive(false);

            listBox.ClearList ();

			SetHeaderAndBtns (leaderboardType, rankingType);

			if (rankingType == RankingType.SCORE) {
                if (leaderboardType == LeaderboardType.USER_SCOREBOARD) {
                    SetVerifyAcctViewVisibility();
                } else {
                    listBox.gameObject.SetActive(true);
                    verifyAcctView.SetActive(false);
                    loadingView.SetActive(true);
                    MegafansWebService.Instance.ViewScoreboard(Megafans.Instance.GameUID, false,
                        gameType, OnScoreboardResponse, OnScoreboardFailure);
                }                   
			}
            else if (rankingType == RankingType.RANKING) {
				this.rankingType = rankingType;
                listBox.gameObject.SetActive(true);
                verifyAcctView.SetActive(false);
                loadingView.SetActive(true);
                MegafansWebService.Instance.ViewLeaderboard (Megafans.Instance.GameUID,
					gameType, OnLeaderboardResponse, OnLeaderboardFailure);
			}
		}

		private void OnScoreboardResponse(ViewScoreboardResponse response) {
            loadingView.SetActive(false);
            if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
				if (response.data != null) {
                    bool isUserAlreadyDisplayed = false;
                    if (response.data.user.Count == 0) {
                        if (this.gameType == GameType.PRACTICE) {
                            messageTxt.text = "No practice results";
                        } else {
                            messageTxt.text = "No tournament results";
                        }
                        messageTxt.gameObject.SetActive(true);
                    } else {
                        messageTxt.gameObject.SetActive(false);
                        for (int i = 0; i < response.data.user.Count; ++i)
                        {
                            GameObject item = Instantiate(leaderboardItemPrefab);
                            LeaderboardItem viewHandler = item.GetComponent<LeaderboardItem>();
                            if (viewHandler != null)
                            {
                                viewHandler.SetValues(response.data.user[i].score.ToString(), response.data.user[i].username, response.data.user[i].code);

                                if (response.data.user[i].code == response.data.me.code)
                                {
                                    isUserAlreadyDisplayed = true;
                                    if (leaderboardType == LeaderboardType.USER_SCOREBOARD) {
                                        viewHandler.SetKeyColor(Color.clear, MegafansUI.Instance.primaryColor);
                                        viewHandler.SetValueColor(Color.clear, MegafansUI.Instance.primaryColor);
                                    } else {
                                        viewHandler.SetKeyColor(Color.white, MegafansUI.Instance.tealColor);
                                        viewHandler.SetValueColor(Color.white, MegafansUI.Instance.tealColor);
                                    }
                                }
                                else
                                {
                                    viewHandler.SetKeyColor(Color.clear, MegafansUI.Instance.primaryColor);
                                    viewHandler.SetValueColor(Color.clear, MegafansUI.Instance.primaryColor);
                                }
                                listBox.AddItem(item);
                            }
                        }
                        if (response.data.me != null && response.data.me.score > 0 && !isUserAlreadyDisplayed)
                        {
                            GameObject item = Instantiate(leaderboardItemPrefab);
                            LeaderboardItem viewHandler = item.GetComponent<LeaderboardItem>();
                            if (viewHandler != null)
                            {
                                //if (rankingType == RankingType.RANKING)
                                //{
                                //    viewHandler.SetValues(response.data.me.rank.ToString(),
                                //                           response.data.me.username, response.data.me.code);
                                //}
                                //else if (rankingType == RankingType.SCORE)
                                //{
                                viewHandler.SetValues(response.data.me.score.ToString(),
                                                       response.data.me.username, response.data.me.code);
                                //}
                                viewHandler.SetKeyColor(Color.white, MegafansUI.Instance.tealColor);
                                viewHandler.SetValueColor(Color.white, MegafansUI.Instance.tealColor);

                                listBox.AddItem(item);
                            }
                        }
                    }                                    
                }
            } else {
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

		private void OnScoreboardFailure(string error) {
            loadingView.SetActive(false);
            Debug.LogError (error);
		}

		private void OnLeaderboardResponse(ViewLeaderboardResponse response) {
            loadingView.SetActive(false);
            if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
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

                                listBox.AddItem(item);
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

                            listBox.AddItem(item);
                        }
                    }
                }
            } else {
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

		private void OnLeaderboardFailure(string error) {
            loadingView.SetActive(false);
            Debug.LogError (error);
		}

		private void SetHeaderAndBtns(LeaderboardType leaderboardType,
			RankingType rankingType = RankingType.RANKING) {

			LeaderboardItem headerViewHandler = listBox.Header.GetComponent<LeaderboardItem> ();

			if (leaderboardType == LeaderboardType.USER_SCOREBOARD) {
				if (headerViewHandler != null) {
                    headerViewHandler.SetValues ("SCORE", "LEVEL", "");
				}

				SelectBtn (historyBtn);
			}
			else if (leaderboardType == LeaderboardType.LEADERBOARD) {
				if (rankingType == RankingType.RANKING) {
					if (headerViewHandler != null) {
						headerViewHandler.SetValues ("RANK", "NAME", "");
					}

					SelectBtn (rankingBtn);
				}
				else if (rankingType == RankingType.SCORE) {
					if (headerViewHandler != null) {
						headerViewHandler.SetValues ("SCORE", "NAME", "");
					}

					SelectBtn (scoreBtn);
				}
			}
		}

        private void SetVerifyAcctViewVisibility() {
            if (MegafansPrefs.IsRegisteredMegaFansUser)
            {
                listBox.gameObject.SetActive(true);
                verifyAcctView.SetActive(false);
                loadingView.SetActive(true);
                MegafansWebService.Instance.ViewScoreboard(Megafans.Instance.GameUID, true,
                    gameType, OnScoreboardResponse, OnScoreboardFailure);
            }
            else
            {
                listBox.gameObject.SetActive(false);
                verifyAcctView.SetActive(true);
            }
        }

		private void SelectBtn(Button selectedBtn) {
			foreach (Button btn in leaderboardOptionBtns) {
				if (btn == selectedBtn) {
                    btn.GetComponent<Image>().color = Color.clear;
                    btn.GetComponentsInChildren<Text>()[0].color = MegafansUI.Instance.primaryColor;

                }
				else {
                    btn.GetComponent<Image>().color = MegafansUI.Instance.primaryColor;
                    btn.GetComponentsInChildren<Text>()[0].color = Color.white;
                }
			}
		}

        public void ViewUserProfile(string userCode) {
            //if (this.leaderboardType != LeaderboardType.USER_SCOREBOARD && !string.IsNullOrEmpty(userCode)) {
            //    MegafansUI.Instance.ShowViewProfileWindow(userCode);
            //}
            Debug.Log("Show Profile");
        }

        private void OnViewProfileResponse(ViewProfileResponse response) {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansPrefs.UserStatus = response.data.status ?? 7;
                if (rankingType == RankingType.SCORE)
                {
                    if (leaderboardType == LeaderboardType.USER_SCOREBOARD)
                    {
                        SetVerifyAcctViewVisibility();
                    }
                }
            }
        }

        private void OnViewProfileFailure(string error)
        {
            Debug.LogError(error);
        }

        public void JoinTournamentBtn_OnClick()
        {
            MegafansUI.Instance.ShowSingleTournamentWindow(null);
        }
    }
}