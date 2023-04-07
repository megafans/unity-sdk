#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class OnboardingDetailsTutorialWindowUI : MonoBehaviour
    {

        [SerializeField] private InputField usernameField;
        [SerializeField] private Image isValidUserName;

        private Sprite focusedIcon;
        private Sprite unfocusedIcon;
        private JoinMatchAssistant matchAssistant;

        void OnEnable()
        {
            if (MegafansPrefs.Username != null)
            {
                usernameField.text = MegafansPrefs.Username;
            }
        }

        void Awake()
        {
            matchAssistant = this.gameObject.AddComponent<JoinMatchAssistant>();
        }

        public void PlayGameBtn_OnClick()
        {
            if (usernameField.text == "")
            {
                string msg = "Please enter a username";
                MegafansUI.Instance.ShowPopup("ERROR", msg);
                return;
            }
            else if (usernameField.text != MegafansPrefs.Username)
            {
                MegafansUI.Instance.ShowLoadingBar();
                MegafansWebService.Instance.EditProfile(usernameField.text, null, null,
                    OnEditProfileResponse, OnEditProfileFailure);
            }
            else
            {
                MegafansUI.Instance.ShowLoadingBar();
                MegafansWebService.Instance.GetLevels(Megafans.Instance.GameUID, GameType.TOURNAMENT,
                    OnGetTournamentsResponse, OnGetTournamentsFailure);
                //matchAssistant.JoinPracticeMatch();
            }
        }

        public void BackBtn_OnClick()
        {
            MegafansUI.Instance.ShowOnboardingPrizesWindow();
        }

        public void LoginBtn_OnClick()
        {
            MegafansUI.Instance.ShowLoginWindow(true);
        }

        private void OnEditProfileResponse(EditProfileResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                if (!string.IsNullOrEmpty(usernameField.text))
                {
                    MegafansPrefs.Username = usernameField.text;
                }

                MegafansUI.Instance.HideLoadingBar();

                MegafansWebService.Instance.GetLevels(Megafans.Instance.GameUID, GameType.TOURNAMENT,
                    OnGetTournamentsFromProfileResponse, OnGetTournamentsFromProfileFailure);

                //TODO: Send user to free tourmanent
                //matchAssistant.JoinPracticeMatch();
            }
            else
            {
                string msg = "Failed to save changes.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);
            }
        }

        private void OnEditProfileFailure(string error)
        {
            Debug.LogError(error);
            string errorString = "Error updating profile.  Please try again.";
            MegafansUI.Instance.ShowPopup("Error", errorString);
        }

        public void onUsernameInputChange()
        {

            if (MegafansUtils.IsUsernameValid(usernameField.text))
            {

                isValidUserName.gameObject.SetActive(true);
            }
            else
            {
                isValidUserName.gameObject.SetActive(false);
            }
        }

        private void OnGetTournamentsResponse(LevelsResponse response)
        {
            MegafansUI.Instance.HideLoadingBar();
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansConstants.UserBanned = false;

                List<LevelsResponseData> levelsData = response.data;

                if (levelsData.Count == 0 || levelsData == null)
                {
                    //TODO: Send user to free tourmanent
                    //matchAssistant.JoinPracticeMatch();
                    OnGetTournamentsFailure("ERROR - Failed to join tournament");
                }
                else
                {
                    LevelsResponseData currentF2PLevel = null;
                    foreach (LevelsResponseData level in levelsData)
                    {
                        if (level.secondsLeft > 0 && level.f2p)
                        {
                            currentF2PLevel = level;
                        }
                    }

                    Megafans.Instance.m_AllTournaments = levelsData;

                    MegafansUI.Instance.tournamentLobbyScreenUI.tournamentLobby.GetComponent<TournamentLobbyUI>().listBox.SetUpForScreenCount(levelsData.Count);
                    MegafansUI.Instance.tournamentLobbyScreenUI.tournamentLobby.GetComponent<TournamentLobbyUI>().listBox.ManualDrag(Megafans.Instance.m_AllTournaments.FindIndex(w => w.guid == currentF2PLevel.guid));
                    MegafansUI.Instance.ShowTournamentLobby();
                }
            }
            else
            {
                MegafansUI.Instance.ShowPopup("ALERT", response.message);
                MegafansConstants.UserBanned = true;
                Debug.Log("alert msg :  " + response.message);
            }
        }

        private void OnGetTournamentsFailure(string error)
        {
            Debug.LogError(error);
            MegafansUI.Instance.HideLoadingBar();
        }

        //TODO: Temporary fix for profile change verification
        private void OnGetTournamentsFromProfileResponse(LevelsResponse response)
        {
            MegafansUI.Instance.HideLoadingBar();
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansConstants.UserBanned = false;

                List<LevelsResponseData> levelsData = response.data;
                if (levelsData.Count == 0 || levelsData == null)
                {
                    //TODO: Send user to free tourmanent
                    //matchAssistant.JoinPracticeMatch();
                    OnGetTournamentsFromProfileFailure("ERROR - Failed to join tournament");
                }
                else
                {
                    LevelsResponseData currentF2PLevel = null;
                    foreach (LevelsResponseData level in levelsData)
                    {
                        if (level.secondsLeft > 0 && level.f2p)
                        {
                            currentF2PLevel = level;
                        }
                    }
                    MegafansUI.Instance.ShowTournamentLobby();

                }
            }
            else
            {
                MegafansConstants.UserBanned = true;
                MegafansUI.Instance.ShowPopup("ALERT", response.message);
            }
        }

        private void OnGetTournamentsFromProfileFailure(string error)
        {
            Debug.LogError(error);
            MegafansUI.Instance.HideLoadingBar();
            string errorString = "Error updating profile.  Please try again.";
            MegafansUI.Instance.ShowPopup("Error", errorString);
        }

        //private void OnVerifyPhoneResponse(LoginResponse response)
        //{
        //    if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
        //    {
        //        if (IsRegistering)
        //        {
        //            string msg = "You have registered successfully.";
        //            MegafansUI.Instance.ShowPopup("SUCCESS", msg);

        //            MegafansUI.Instance.ShowLoginWindow();

        //            Megafans.Instance.ReportUserRegistered();
        //        }
        //        else
        //        {
        //            MegafansPrefs.UserId = response.data.id;
        //            MegafansPrefs.ProfilePicUrl = response.data.image;
        //            MegafansPrefs.AccessToken = response.data.token;
        //            MegafansPrefs.RefreshToken = response.data.refresh;
        //            Megafans.Instance.ReportUserLoggedIn(MegafansPrefs.UserId.ToString());
        //        }
        //    }
        //    else
        //    {
        //        string msg = "Invalid OTP.";
        //        MegafansUI.Instance.ShowPopup("ERROR", msg);
        //    }
        //}

        //private void OnVerifyPhoneFailure(string error)
        //{
        //    Debug.LogError(error);
        //}

        //private void OnResendOTPResponse(LoginResponse response)
        //{
        //    if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
        //    {
        //        string msg = "OTP has been resent successfully.";
        //        MegafansUI.Instance.ShowPopup("OTP RESENT", msg);
        //    }
        //}

        //private void OnResendOTPFailure(string error)
        //{
        //    Debug.LogError(error);
        //}

    }

}