#pragma warning disable 649
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.Utils;
using OneSignalSDK;

namespace MegafansSDK.UI
{

    public class LandingWindowUI : MonoBehaviour
    {

        [SerializeField] private Button withFacebookBtn;
        [SerializeField] private Button withEmailBtn;
        [SerializeField] private Button withPhoneBtn;
        [SerializeField] private Button loginOrSignUpInsteadBtn;

        [SerializeField] private Text withFacebookTextLabel;
        [SerializeField] private Text withEmailTextLabel;
        [SerializeField] private Text withPhoneTextLabel;
        [SerializeField] private Text registerOrLoginTextLabel;
        [SerializeField] private Text registerOrLoginActionTextLabel;
        [SerializeField] private Text landingWindowDescriptionText;
        [SerializeField] private GameObject smsRegistrationOption;

        public bool IsLogin = false;

        public bool IsLinking = false;

        private void OnEnable()
        {
            withFacebookBtn.gameObject.SetActive(true);
            withEmailBtn.gameObject.SetActive(true);
            withPhoneBtn.gameObject.SetActive(true);
            updateUIForLoginOrSignUp();
            
        }

        public void CloseBtn_OnClick()
        {
            MegafansUI.Instance.HideLandingWindow();
        }

        public void LoginWithEmailBtn_OnClick()
        {
            if (IsLogin)
            {
                MegafansUI.Instance.ShowLoginWindow(true);
            }
            else
            {
                MegafansUI.Instance.ShowRegistrationWindow(true, IsLinking);
            }
        }

        public void LoginWithPhoneBtn_OnClick()
        {
            if (IsLogin)
            {
                MegafansUI.Instance.ShowLoginWindow(false);
            }
            else
            {
                MegafansUI.Instance.ShowRegistrationWindow(false, IsLinking);
            }
        }

        public void ToggleLoginSignUpBtn_OnClick()
        {
            IsLogin = !IsLogin;
            updateUIForLoginOrSignUp();
        }

        private void OnLoginFBResponse(LoginResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                Debug.Log("***************FB REGISTER/Login******************");
                Debug.Log(response.data);
                Debug.Log("*********************************");
                MegafansPrefs.ProfilePicUrl = response.data.image;
                MegafansPrefs.Email = response.data.email;
                MegafansPrefs.AccessToken = response.data.token;
                MegafansPrefs.RefreshToken = response.data.refresh;
                Megafans.Instance.ReportUserLoggedIn(MegafansPrefs.UserId.ToString());
            }
            else
            {
                Debug.LogError(response.message);
                string error = "There was an error: " + response.message;
                MegafansUI.Instance.ShowPopup("ERROR", error);
            }
        }

        private void OnLoginFBFailure(string error)
        {
            Debug.LogError(error);

            if (error.ToLower().Contains(MegafansConstants.UNAUTHORIZED_CODE.ToLower()))
            {
                string msg = "You need to register first.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);
            }
        }

        private void OnRegisterFBResponse(RegisterResponse response)
        {
            MegafansUI.Instance.HideLoadingBar();
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                Debug.Log("***************FB REGISTER******************");
                Debug.Log(response.data);
                Debug.Log("*********************************");
                //string msg = "You have registered successfully.";
                //MegafansUI.Instance.ShowPopup("SUCCESS", msg);
                Debug.Log(response.data);
                if (!string.IsNullOrEmpty(MegafansPrefs.ProfilePic))
                {
                    StartCoroutine(MegafansWebService.Instance.UploadProfilePic(MegafansUtils.StringToTexture(MegafansPrefs.ProfilePic), (obj) =>
                    {
                        Debug.Log("Successfully uploaded profile picture.");
                        //MegafansUI.Instance.ShowPopup("SUCCESS", msg);
                    }, (err) =>
                    {
                        Debug.Log("Error uploading profile image.  Please try again.");
                    }));
                }

                MegafansUI.Instance.ShowRegistrationSuccessWindow(false);

                Megafans.Instance.ReportUserRegistered(MegafansPrefs.UserId.ToString());
            }
            else
            {
                Debug.LogError(response.message);
                string error = "There was an error: " + response.message;
                MegafansUI.Instance.ShowPopup("ERROR", error);
                MegafansPrefs.Email = null;
                MegafansPrefs.Username = null;
            }
        }

        private void OnRegisterFBFailure(string error)
        {
            MegafansUI.Instance.HideLoadingBar();
            Debug.LogError(error);
            MegafansUI.Instance.ShowPopup("ERROR", error);
            MegafansPrefs.Email = null;
            MegafansPrefs.Username = null;
        }

        private void updateUIForLoginOrSignUp()
        {
            smsRegistrationOption.SetActive(MegafansPrefs.SMSAvailable);
            if (IsLogin)
            {
                withFacebookTextLabel.text = "Log in with FB";
                withEmailTextLabel.text = "Log in with email";
                withPhoneTextLabel.text = "Log in with phone";
                registerOrLoginTextLabel.text = "Need an account?";
                registerOrLoginActionTextLabel.text = "REGISTER";
                landingWindowDescriptionText.text = "Log in to your MegaFans account to get back into the tournament arena";
                loginOrSignUpInsteadBtn.gameObject.SetActive(true);
            }
            else if (IsLinking)
            {
                withEmailTextLabel.text = "Link email";
                withFacebookTextLabel.text = "Link FB";
                withPhoneTextLabel.text = "Link phone";
                loginOrSignUpInsteadBtn.gameObject.SetActive(false);
                if (!string.IsNullOrEmpty(MegafansPrefs.Email))
                {
                    withEmailBtn.gameObject.SetActive(false);
                }
                else
                {
                    withEmailBtn.gameObject.SetActive(true);
                }

                if (MegafansPrefs.IsPhoneVerified)
                {
                    withPhoneBtn.gameObject.SetActive(false);
                }
                else
                {
                    withPhoneBtn.gameObject.SetActive(true);
                }

                if (!string.IsNullOrEmpty(MegafansPrefs.FacebookID))
                {
                    withFacebookBtn.gameObject.SetActive(false);
                }
                else
                {
                    withFacebookBtn.gameObject.SetActive(true);
                }
            }
            else
            {
                withFacebookTextLabel.text = "Use FB";
                withEmailTextLabel.text = "Use email";
                withPhoneTextLabel.text = "Use phone";
                registerOrLoginTextLabel.text = "Have an account?";
                registerOrLoginActionTextLabel.text = "LOG IN";
                landingWindowDescriptionText.text = "Choose the most convenient way to save your progress and create your MegaFans account";
                loginOrSignUpInsteadBtn.gameObject.SetActive(true);
            }
        }

        private void OnRegisterNewUserResponse(RegisterFirstTimeResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansPrefs.AccessToken = response.data.token;
                MegafansPrefs.RefreshToken = response.data.refresh;
                MegafansPrefs.Username = response.data.username;
                MegafansPrefs.UserId = response.data.userId;
                MegafansPrefs.SMSAvailable = response.data.sms;
                OneSignal.Default.SetExternalUserId(response.data.userId.ToString());

            }
            else
            {
                MegafansUI.Instance.ShowPopup("ERROR", response.message);
            }
        }

        private void OnRegisterNewUserFailure(string error)
        {
            Debug.LogError(error);
        }

        private void OnEditProfileResponse(EditProfileResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansUI.Instance.ShowPopup("SUCCESS", "Successfully linked your account");
                MegafansUI.Instance.ShowTournamentLobby();
            }
            else
            {
                string msg = "Failed to save changes.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);
            }
        }

    }
}