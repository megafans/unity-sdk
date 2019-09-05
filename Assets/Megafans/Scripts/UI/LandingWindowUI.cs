using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.Utils;
using MegaFans.Unity.iOS;
using MegaFans.Unity.Android;

namespace MegafansSDK.UI {

	public class LandingWindowUI : MonoBehaviour {

        [SerializeField] private Text withFacebookTextLabel;
        [SerializeField] private Text withEmailTextLabel;
        [SerializeField] private Text withPhoneTextLabel;
        [SerializeField] private Text registerOrLoginTextLabel;
        [SerializeField] private Text registerOrLoginActionTextLabel;
        [SerializeField] private Text landingWindowDescriptionText;
        [SerializeField] private GameObject smsRegistrationOption;

        public bool IsLogin = false;

        private void OnEnable() {
            updateUIForLoginOrSignUp();
            //if (!Megafans.Instance.IsUserLoggedIn)
            //{
            //    MegafansWebService.Instance.RegisterNewUser(OnRegisterNewUserResponse, OnRegisterNewUserFailure);
            //}
        }

        public void LoginWithFacebookBtn_OnClick() {
            if (IsLogin)
            {
                MegafansWebService.Instance.LoginFB(OnLoginFBResponse, OnLoginFBFailure);
            } else {
                MegafansUI.Instance.ShowLoadingBar();
                MegafansWebService.Instance.RegisterFB(OnRegisterFBResponse, OnRegisterFBFailure);
            }
        }

        public void CloseBtn_OnClick(){
            MegafansUI.Instance.HideLandingWindow();
        }

        public void LoginWithEmailBtn_OnClick() {
            if (IsLogin) {
                MegafansUI.Instance.ShowLoginWindow(true);
            } else {
                MegafansUI.Instance.ShowRegistrationWindow(true);
            }
        }

        public void LoginWithPhoneBtn_OnClick()
        {
            if (IsLogin) {
                MegafansUI.Instance.ShowLoginWindow(false);
            } else {
                MegafansUI.Instance.ShowRegistrationWindow(false);
            }
        }

        public void ToggleLoginSignUpBtn_OnClick() {
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
                if (!string.IsNullOrEmpty(MegafansPrefs.ProfilePic)) {
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

        private void updateUIForLoginOrSignUp(){
            smsRegistrationOption.SetActive(MegafansPrefs.SMSAvailable);
            if (IsLogin)
            {
                withFacebookTextLabel.text = "Log in with FB";
                withEmailTextLabel.text = "Log in with email";
                withPhoneTextLabel.text = "Log in with phone";
                registerOrLoginTextLabel.text = "Need an account?";
                registerOrLoginActionTextLabel.text = "REGISTER";
                landingWindowDescriptionText.text = "Log in to your MegaFans account to get back into the tournament arena";
            }
            else
            {
                withFacebookTextLabel.text = "Use FB";
                withEmailTextLabel.text = "Use email";
                withPhoneTextLabel.text = "Use phone";
                registerOrLoginTextLabel.text = "Have an account?";
                registerOrLoginActionTextLabel.text = "LOG IN";
                landingWindowDescriptionText.text = "Choose the most convenient way to save your progress and create your MegaFans account";
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
                //OneSignal.SetExternalUserId(response.data.userId.ToString());
#if UNITY_EDITOR
                Debug.Log("Unity Editor");
#elif UNITY_IOS
                Debug.Log("IOS");
                IntercomWrapperiOS.RegisterIntercomUserWithID(MegafansPrefs.UserId.ToString(), Megafans.Instance.GameUID, Application.productName);
#elif UNITY_ANDROID
                Debug.Log("ANDROID");
                IntercomWrapperAndroid.RegisterUserWithUserId(MegafansPrefs.UserId.ToString(), Megafans.Instance.GameUID, Application.productName);
#endif
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
    }
}