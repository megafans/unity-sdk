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
        [SerializeField] private Image usernameFieldHighlightedImage;
        [SerializeField] private Image isValidUserName;

        private Sprite focusedIcon;
        private Sprite unfocusedIcon;
        private JoinMatchAssistant matchAssistant;

        void OnEnable()
        {
            if (MegafansPrefs.Username != null) {
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
            } else if (usernameField.text != MegafansPrefs.Username) {
                MegafansUI.Instance.ShowLoadingBar ();
                MegafansWebService.Instance.EditProfile(usernameField.text, null, null,
                    OnEditProfileResponse, OnEditProfileFailure);
            } else {
                matchAssistant.JoinPracticeMatch();
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
                matchAssistant.JoinPracticeMatch();
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

        void Update()
        {
            if (usernameField.GetComponent<InputField>().isFocused == true)
            {
                usernameFieldHighlightedImage.gameObject.SetActive(true);
            }
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