using System;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.Utils;
using MegaFans.Unity.iOS;
using MegaFans.Unity.Android;

namespace MegafansSDK.UI
{

    public class MyAccountWindowUI : MonoBehaviour
    {

        [SerializeField] private Text userTokensTxt;
        [SerializeField] private Text userNameLabel;
        [SerializeField] private RawImage profilePicImg;
        [SerializeField] private Button loginBtn;
        [SerializeField] private Button logoutBtn;
        [SerializeField] private Sprite warningIcon;

        private ImagePicker imagePicker;
        private Texture2D picPlaceholder;

        private bool isPhotoRemoved = false;

        void Awake()
        {
            imagePicker = this.gameObject.AddComponent<ImagePicker>();

            picPlaceholder = (Texture2D)profilePicImg.texture;
        }

        void OnEnable()
        {
            Debug.Log("User current token balance - " + MegafansPrefs.CurrentTokenBalance.ToString());
            userTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
            userNameLabel.text = MegafansPrefs.Username;
            profilePicImg.texture = picPlaceholder;
            isPhotoRemoved = false;

            MegafansWebService.Instance.ViewProfile("", OnViewProfileResponse, OnViewProfileFailure);
            int status = MegafansPrefs.UserStatus;
            //if (MegafansPrefs.IsRegisteredMegaFansUser)
            //{
                logoutBtn.gameObject.SetActive(true);
                loginBtn.gameObject.SetActive(false);
            //}
            //else
            //{
            //    //saveAccountBtn.gameObject.SetActive(true);
            //    //loginToAccountBtn.gameObject.SetActive(true);
            //    logoutBtn.gameObject.SetActive(false);
            //    loginBtn.gameObject.SetActive(true);
            //}
        }

        void Start()
        {

        }

        public void BackBtn_OnClick()
        {
            MegafansUI.Instance.ShowTournamentLobby();
        }

        public void LinkAccountBtn_OnClick()
        {
            if (!string.IsNullOrEmpty(MegafansPrefs.Email) && !string.IsNullOrEmpty(MegafansPrefs.PhoneNumber) && !string.IsNullOrEmpty(MegafansPrefs.FacebookID)) {
                MegafansUI.Instance.ShowPopup("Already Linked", "Your account is already linked.");
            } else {
                MegafansUI.Instance.ShowLandingWindow(false, MegafansPrefs.IsRegisteredMegaFansUser);
            }
        }

        public void UpdatePassword_OnClick()
        {
            if (!MegafansPrefs.IsRegisteredMegaFansUser) {
                MegafansUI.Instance.ShowUpdateProfileWindow();
            } else if (string.IsNullOrEmpty(MegafansPrefs.Email)) {
                string msg = "Please link your email address before you are able to add or update your password. Would you like to link your email now?";

                MegafansUI.Instance.ShowAlertDialog(warningIcon, "Link Email",
                  msg, "Link Now", "Link Later",
                  () => {
                      MegafansUI.Instance.HideAlertDialog();
                      MegafansUI.Instance.ShowLandingWindow(false, MegafansPrefs.IsRegisteredMegaFansUser);                      
                  },
                  () => {
                      MegafansUI.Instance.HideAlertDialog();
                  });
            } else {
                MegafansUI.Instance.ShowUpdateProfileWindow();
            }
        }

        public void TermsOfUseAndPrivacyBtn_OnClick()
        {
            MegafansUI.Instance.ShowTermsOfUseOrPrivacyWindow(MegafansConstants.TERMS_OF_USE, false);
        }

        public void ProfilePic_OnClick()
        {
#if UNITY_EDITOR
            Debug.Log("Unity Editor");

            ////if (tex != null)
            ////{
            //    //isPhotoRemoved = false;
            //    //profilePicImg.texture = tex;               
            //    StartCoroutine(MegafansWebService.Instance.UploadProfilePic("/Users/markhoyt/Downloads/IMG_3956.PNG", (obj) =>
            //    {
            //        string msg = "Successfully uploaded profile picture.";
            //        MegafansUI.Instance.ShowPopup("SUCCESS", msg);
            //    }, (err) =>
            //    {
            //        string msg = "Error uploading profile image.  Please try again.";
            //        MegafansUI.Instance.ShowPopup("ERROR", msg);
            //    }));
            //return; 
#elif UNITY_IOS
            Debug.Log("IOS");
            IntercomWrapperiOS.HideIntercom();
#elif UNITY_ANDROID
            Debug.Log("ANDROID");
            IntercomWrapperAndroid.HideIntercom();
#endif
            imagePicker.PickImage((Texture2D tex, string imagePath) => {
                if (tex != null)
                {
                    isPhotoRemoved = false;
                    profilePicImg.texture = tex;
                    string newImageToUpload = MegafansUtils.TextureToString((Texture2D)profilePicImg.texture);
                    MegafansPrefs.ProfilePic = newImageToUpload;
                    StartCoroutine(MegafansWebService.Instance.UploadProfilePic(tex, (obj) =>
                    {
                        string msg = "Successfully uploaded profile picture.";
                        MegafansUI.Instance.ShowPopup("SUCCESS", msg);
                    }, (err) =>
                    {
                        string msg = "Error uploading profile image.  Please try again.";
                        MegafansUI.Instance.ShowPopup("ERROR", msg);
                    }));
                }
                else
                {
                    string msg = "Error uploading profile image.  Please try again.";
                    MegafansUI.Instance.ShowPopup("ERROR", msg);
                }
            }, 750, "Select your profile photo");
        }

        public void RemovePicBtn_OnClick()
        {
            isPhotoRemoved = true;
            profilePicImg.texture = picPlaceholder;
        }

        public void EditAccountBtn_OnClick()
        {
            MegafansUI.Instance.ShowEditProfileWindow();
        }

        public void SaveAcctBtn_OnClick()
        {
            MegafansUI.Instance.ShowLandingWindow(false);
        }

        public void LoginBtn_OnClick()
        {
            MegafansUI.Instance.ShowLandingWindow(true);
        }

        public void LogoutBtn_OnClick()
        {
            if (MegafansPrefs.IsRegisteredMegaFansUser) {
                MegafansWebService.Instance.Logout(OnLogoutResponse, OnLogoutFailure);
                MegafansPrefs.ClearPrefs();
                MegafansPrefs.DeviceTokens = DeviceInfo.DeviceToken;
                MegafansUI.Instance.ShowOnboardingStartWindow();
            } else {
                string msg = "You are about to logout without saving your progress.  If you don't save your account, all progress, tokens, and history will be lost.  Are you sure you want to logout?" ;

                MegafansUI.Instance.ShowAlertDialog (warningIcon, "Confirmation",
                  msg, "Log Out", "Cancel",
                  () => {
                      MegafansUI.Instance.HideAlertDialog();
                      MegafansWebService.Instance.Logout(OnLogoutResponse, OnLogoutFailure);
                      MegafansPrefs.ClearPrefs();
                      MegafansPrefs.DeviceTokens = DeviceInfo.DeviceToken;
                      MegafansUI.Instance.ShowOnboardingStartWindow();
                  },
                  () => {
                       MegafansUI.Instance.HideAlertDialog();
                });
            }
        }

        private void OnViewProfileResponse(ViewProfileResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                userNameLabel.text = response.data.username;

                MegafansWebService.Instance.FetchImage(response.data.image, OnFetchPicSuccess, OnFetchPicFailure);

                //if (MegafansPrefs.IsRegisteredMegaFansUser)
                //{
                //    if (System.String.IsNullOrEmpty(response.data.email))
                //    {
                //        emailAddressField.gameObject.SetActive(true);
                //        phoneNumberField.gameObject.SetActive(false);
                //        phoneNumberPrefixField.gameObject.SetActive(false);
                //        isEmailProfile = false;
                //    }
                //    else
                //    {
                //        if (System.String.IsNullOrEmpty(response.data.phoneNumber))
                //        {
                //            emailAddressField.gameObject.SetActive(false);
                //            phoneNumberField.gameObject.SetActive(true);
                //            phoneNumberPrefixField.gameObject.SetActive(true);
                //        }
                //        else
                //        {
                //            emailAddressField.gameObject.SetActive(false);
                //            phoneNumberField.gameObject.SetActive(false);
                //            phoneNumberPrefixField.gameObject.SetActive(false);
                //        }
                //        updatePasswordBtn.gameObject.SetActive(true);
                //        isEmailProfile = true;
                //    }
                //}
                //else
                //{
                //    emailAddressField.gameObject.SetActive(false);
                //    phoneNumberField.gameObject.SetActive(false);
                //    phoneNumberPrefixField.gameObject.SetActive(false);
                //    updatePasswordBtn.gameObject.SetActive(false);
                //}
            }
        }

        private void OnViewProfileFailure(string error)
        {
            Debug.LogError(error);
        }                

        private void OnFetchPicSuccess(Texture2D tex)
        {
            if (tex != null)
            {
                if (!isPhotoRemoved)
                {
                    profilePicImg.texture = tex;
                }
            }
        }

        private void OnFetchPicFailure(string error)
        {
            Debug.LogError(error);
        }

        private void OnLogoutResponse(LogoutResponse response)
        {
            //if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
            MegafansPrefs.UserId = 0;
            MegafansPrefs.ProfilePicUrl = "";
            MegafansPrefs.ClearPrefs();

            MegafansWebService.Instance.FBLogout();

#if UNITY_EDITOR
            Debug.Log("Unity Editor");
#elif UNITY_IOS
                Debug.Log("Logging Out iOS");
                IntercomWrapperiOS.LogoutFromIntercom();
#elif UNITY_ANDROID
                Debug.Log("Logging Out Android");
                IntercomWrapperAndroid.LogoutFromIntercom();         
#endif           
            MegafansUI.Instance.ShowOnboardingStartWindow();
            //}
        }

        private void OnLogoutFailure(string error)
        {
            Debug.LogError(error.ToString());
        }
    }

}