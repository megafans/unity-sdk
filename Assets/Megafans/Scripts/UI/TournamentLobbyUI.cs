using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using MegafansSDK.Utils;
using UnityEngine.EventSystems;
using MegaFans.Unity.iOS;
using MegaFans.Unity.Android;

namespace MegafansSDK.UI {

    public interface TournamentCardItemCustomMessageTarget : IEventSystemHandler
    {
        void EnterTournamentBtn_OnClick(LevelsResponseData tournament);
        void ScrollViewDidFinishScrollingOnIndex(int index);
    }

    public class TournamentLobbyUI : MonoBehaviour, TournamentCardItemCustomMessageTarget
    {

		[SerializeField] private Text gameNameTxt;
        [SerializeField] private Text userTokensValueTxt;
        [SerializeField] private ListBox listBox;
        [SerializeField] private GameObject tournamentCardItemPrefab;
        [SerializeField] private GameObject countdownTimer;
        [SerializeField] private Text nextTournamentDateLabel;
        [SerializeField] private Image gameIconImg;
        [SerializeField] private Button joinNowBtn;
        [SerializeField] private Text joinNowBtnText;
        private float lastTokenBalance = 0;

        private JoinMatchAssistant matchAssistant;
		private Texture picPlaceholder;
        public List<LevelsResponseData> tournaments;
        //{
        //    get {
        //        return tournaments;
        //    }
        //    set {
        //        tournaments = value;
        //    }
        //}

		void Awake() {
			matchAssistant = this.gameObject.AddComponent<JoinMatchAssistant> ();

			//picPlaceholder = profilePicImg.texture;
        }

		void Start() {
			SetGameNameTxt ();
            gameIconImg.sprite = Megafans.Instance.GameIcon;
            //listBox.ClearList();
            //RequestTournaments();
        }

        void OnEnable() {
            //SetProfilePic();
            listBox.ClearList();
            if (!string.IsNullOrEmpty(MegafansPrefs.AccessToken))
            {
                MegafansWebService.Instance.ViewProfile("", OnViewProfileResponse, OnViewProfileFailure);
            }
            RequestTournaments();

#if UNITY_EDITOR
            Debug.Log("Unity Editor");
#elif UNITY_IOS
            Debug.Log("IOS");
            IntercomWrapperiOS.ShowIntercomIfUnreadMessages();
#elif UNITY_ANDROID
            Debug.Log("ANDROID show intercom");
            IntercomWrapperAndroid.ShowIntercomIfUnreadMessages();
#endif
            //UnityEngine.Purchasing.ProductCollection allproducts = InAppPurchaser.Instance.GetProducts();
        }

        public void RequestTournaments() {
            MegafansWebService.Instance.GetLevels(Megafans.Instance.GameUID, GameType.TOURNAMENT,
                OnGetTournamentsResponse, OnGetTournamentsFailure);
        }           

        public void ProfilePic_OnClick() {
			MegafansUI.Instance.ShowEditProfileWindow ();
		}

        public void EnterTournamentBtn_OnClick(LevelsResponseData tournament) {
            MegafansUI.Instance.ShowSingleTournamentWindow(tournament);
            //MegafansUI.Instance.ShowLevelSelection ();
        }

        public void ScrollViewDidFinishScrollingOnIndex(int index)
        {
            CountdownTimer timer = listBox.GetItem(index).transform.GetChild(0).gameObject.GetComponent<CountdownTimer>();
            LevelsResponseData tournamentAtIndex = tournaments[index];
            SetCountDownTimer_ForTournament(tournamentAtIndex, timer.secondsRemaining);
        }

        private void SetCountDownTimer_ForTournament(LevelsResponseData tournament, int withSeconds = 0) {
            CountdownTimer timerViewHandler = countdownTimer.GetComponent<CountdownTimer>();
            if (tournament.secondsToStart > 0)
            {                
                if (timerViewHandler != null)
                {
                    if (withSeconds != 0) {
                        timerViewHandler.Init(withSeconds);
                    } else {
                        timerViewHandler.Init(tournament.secondsToStart);
                    }
                }
                joinNowBtn.interactable = false;
                joinNowBtnText.text = "UPCOMING";
                DateTime startDate = DateTime.Parse(tournament.start);
                nextTournamentDateLabel.text = "Tournament Starts On " + MegafansUtils.GetNameOfMonth(startDate.Month) + " " + startDate.Day + ":";
            }
            else
            {
                if (timerViewHandler != null)
                {
                    if (withSeconds != 0)
                    {
                        timerViewHandler.Init(withSeconds);
                    } else {
                        timerViewHandler.Init(tournament.secondsLeft);
                    }
                }                           
                joinNowBtn.interactable = true;
                joinNowBtnText.text = "JOIN NOW";
                DateTime endDate = DateTime.Parse(tournament.end);
                nextTournamentDateLabel.text = "Tournament Ends " + MegafansUtils.GetNameOfMonth(endDate.Month) + " " + endDate.Day + ":";
            }
        }

        public void JoinNowBtn_OnClick() {
            if(tournaments == null || listBox == null || listBox.currentIndex >= tournaments.Count) {
                //TODO: Add popup to join now button that lets users know that there is no available tournaments.
                Debug.LogWarning($@"Error unable to join: Number of available tournaments ({tournaments?.Count.ToString()
                    ?? "Null"}), Attempting to join listbox number ({listBox?.currentIndex.ToString() ?? "Null"})");
                return;
            }
            LevelsResponseData tournament = tournaments[listBox.currentIndex];
            MegafansUI.Instance.ShowSingleTournamentWindow(tournament);
        }

        public void LeaderboardBtn_OnClick() {
			MegafansUI.Instance.ShowLeaderboard (GameType.TOURNAMENT, RankingType.RANKING);
		}

		public void ScoresBtn_OnClick() {
			MegafansUI.Instance.ShowLeaderboard (GameType.TOURNAMENT, RankingType.SCORE);
		}

		public void PracticeGameBtn_OnClick() {
            MegafansUI.Instance.ShowSinglePracticeWindow();
		}

        public void MyAccountBtn_OnClick() {
            MegafansUI.Instance.ShowMyAccountWindow();
        }

        public void PracticeLeaderboardBtn_OnClick() {
			MegafansUI.Instance.ShowLeaderboard (GameType.PRACTICE, RankingType.RANKING);
		}

		public void PracticeScoresBtn_OnClick() {
			MegafansUI.Instance.ShowLeaderboard (GameType.PRACTICE, RankingType.SCORE);
		}

        public void NextBtn_OnClick()
        {
            listBox?.Next();
        }

        public void PreviousBtn_OnClick()
        {
            listBox?.Back();
        }

        private void SetGameNameTxt() {
			string gameName = Application.productName;
			int maxLengthAllowed = 20;
			if (gameName.Length > maxLengthAllowed) {
				gameName = gameName.Substring (0, maxLengthAllowed);

				if (gameName [gameName.Length - 1].Equals (' ')) {
					gameName = gameName.Remove (gameName.Length - 1);
				}

				gameName += "...";
			}

			gameNameTxt.text = gameName;
		}

        private void FillLevels(List<LevelsResponseData> data)
        {
            listBox.SetUpForScreenCount(data.Count);
            for (int i = 0; i < data.Count; i++)
            {
                LevelsResponseData levelInfo = data[i];
                if (i == listBox.currentIndex) {
                    SetCountDownTimer_ForTournament(levelInfo);
                }
                GameObject tournamentItem = Instantiate(tournamentCardItemPrefab);
                TournamentCardItem viewHandler = tournamentItem.GetComponent<TournamentCardItem>();
                if (viewHandler != null)
                {
                    viewHandler.SetValues(levelInfo);
                    listBox.AddItem(tournamentItem);
                }
            }
        }

        //private void OnFetchPicSuccess(Texture2D tex) {
        //	if (tex != null) {
        //		profilePicImg.texture = tex;
        //          } else {
        //              profilePicImg.texture = picPlaceholder;
        //          }
        //}

        //private void OnFetchPicFailure(string error) {
        //	Debug.LogError (error);
        //          profilePicImg.texture = picPlaceholder;
        //      }

        //private void SetProfilePic() {
        //	profilePicImg.texture = picPlaceholder;
        //          string profilePic = MegafansPrefs.ProfilePic;
        //	if (!string.IsNullOrEmpty (MegafansPrefs.ProfilePic)) {
        //              profilePicImg.texture = MegafansUtils.StringToTexture (MegafansPrefs.ProfilePic);
        //	}
        //	else {
        //              if (MegafansPrefs.ProfilePicUrl != null && MegafansWebService.Instance != null) {
        //                  MegafansWebService.Instance.FetchImage(MegafansPrefs.ProfilePicUrl,
        //                      OnFetchPicSuccess, OnFetchPicFailure);
        //              } else {
        //                  profilePicImg.texture = picPlaceholder;
        //              }			
        //	}
        //}

        private void OnViewProfileResponse(ViewProfileResponse response) {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansPrefs.ProfilePicUrl = response.data.image;
                MegafansPrefs.Username = response.data.username;
                MegafansPrefs.UserStatus = response.data.status ?? 7;
                MegafansPrefs.CurrentTokenBalance = response.data.clientBalance;

                userTokensValueTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
                if (response.data.email != null) {
                    MegafansPrefs.Email = response.data.email;
                }

                if (response.data.phoneNumber != null) {
                    MegafansPrefs.PhoneNumber = response.data.phoneNumber;
                }
                //SetProfilePic();
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
                    tournaments = levelsData;
                    FillLevels(levelsData);
                }
            }
        }

        private void OnGetTournamentsFailure(string error)
        {
            Debug.LogError(error);
        }             
    }

}