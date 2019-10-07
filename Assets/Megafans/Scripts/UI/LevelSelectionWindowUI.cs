#pragma warning disable 649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.Utils;

namespace MegafansSDK.UI {

	public class LevelSelectionWindowUI : MonoBehaviour {

		[SerializeField] private Text userTokensTxt;
		[SerializeField] private GameObject levelItemPrefab;
		[SerializeField] private ListBox listBox;
		[SerializeField] private Text messageTxt;
		[SerializeField] private Sprite coinsWarningIcon;

		private JoinMatchAssistant matchAssistant;
        private float lastTokenBalance = 0;

		void Awake() {
			matchAssistant = this.gameObject.AddComponent<JoinMatchAssistant> ();
		}

		void OnEnable() {
			userTokensTxt.text = "";
			listBox.ClearList ();

			SetUserTokens ();
            UnityEngine.Purchasing.ProductCollection allproducts = InAppPurchaser.Instance.GetProducts();
            //RequestLevels ();
        }

		void Start() {
			
		}

		public void BackBtn_OnClick() {
			MegafansUI.Instance.ShowTournamentLobby ();
		}

        public void BuyBtn_OnClick() {
            if (MegafansPrefs.IsRegisteredMegaFansUser) {
                MegafansUI.Instance.ShowStoreWindow();
            } else {
                ShowUnregisteredUserWarning();
            }
        }

        public void RequestLevels() {
			MegafansWebService.Instance.GetLevels (Megafans.Instance.GameUID, GameType.TOURNAMENT,
				OnGetLevelsResponse, OnGetLevelsFailure);
		}

		private void SetUserTokens() {
            if (!string.IsNullOrEmpty(MegafansPrefs.AccessToken)) {
                MegafansWebService.Instance.GetCredits(MegafansPrefs.UserId, OnGetCreditsSuccess,
                OnGetCreditsFailure);
            }
		}

		private void OnGetLevelsResponse(LevelsResponse response) {
			if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
				List<LevelsResponseData> levelsData = response.data;
                if (levelsData.Count == 0 || levelsData == null) {
					messageTxt.gameObject.SetActive (true);
					messageTxt.text = "No tournaments running right now";
				}
				else {
					messageTxt.gameObject.SetActive (false);
					FillLevels (levelsData);
				}
			}
		}

		private void OnGetLevelsFailure(string error) {
			Debug.LogError (error);
		}

		private void OnGetCreditsSuccess(CheckCreditsResponse response) {
			if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
				userTokensTxt.text = response.data.credits.ToString ();
                lastTokenBalance = response.data.credits;
                RequestLevels ();
			}
		}

		private void OnGetCreditsFailure(string error) {
			Debug.LogError (error);
		}

		private void FillLevels(List<LevelsResponseData> data) {
			for (int i = 0; i < data.Count; i++) {
				LevelsResponseData levelInfo = data [i];
				GameObject levelItem = Instantiate (levelItemPrefab);
				LevelItem viewHandler = levelItem.GetComponent<LevelItem> ();
				if (viewHandler != null) {
                    viewHandler.SetValues (levelInfo.name, levelInfo.entryFee, () => {
						Megafans.Instance.CurrentTournamentId = levelInfo.id;

						try {
							int userCredits = int.Parse(userTokensTxt.text);
							if(userCredits < levelInfo.entryFee) {
                                if (MegafansPrefs.IsRegisteredMegaFansUser) {
                                    ShowCreditsWarning(userCredits);
                                } else {
                                    ShowUnregisteredUserWarning();
                                }                                   
							} else {
                                matchAssistant.JoinTournamentMatch(levelInfo.id);
                                //if (MegafansPrefs.IsRegisteredMegaFansUser)
                                //{
                                //    matchAssistant.JoinTournamentMatch(levelInfo.id);
                                //} else {
                                //    matchAssistant.JoinTournamentMatch(levelInfo.id);
                                //    //ShowUnregisteredUserWarning();
                                //}
							}
						}
						catch(Exception e) {
							Debug.LogError(e.ToString());
							string error = "OOPS! Something went wrong. Please try later.";
							MegafansUI.Instance.ShowPopup("ERROR", error);
						}
					});

					listBox.AddItem (levelItem);
				}
			}
		}

		private void ShowCreditsWarning(int currentCredits) {
			MegafansUI.Instance.ShowAlertDialog (coinsWarningIcon, "Alert",
				MegafansConstants.NOT_ENOUGH_TOKENS_WARNING, "Buy Now", "Dismiss",
				() => {
					MegafansUI.Instance.HideAlertDialog();
					MegafansUI.Instance.ShowStoreWindow();
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