#pragma warning disable 649
using UnityEngine;
using UnityEngine.UI;
using MegafansSDK.Utils;

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
            imagePicker = GetComponent<ImagePicker>();

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

            logoutBtn.gameObject.SetActive(true);
            loginBtn.gameObject.SetActive(false);
        }

        public void UpdateCreditUI()
        {
            userTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        public void BackBtn_OnClick()
        {
            MegafansUI.Instance.ShowTournamentLobby();
        }

        public void LinkAccountBtn_OnClick()
        {
            if (!string.IsNullOrEmpty(MegafansPrefs.Email) && MegafansPrefs.IsPhoneVerified && !string.IsNullOrEmpty(MegafansPrefs.FacebookID))
            {
                MegafansUI.Instance.ShowPopup("Already Linked", "Your account is already linked.");
            }
            else
            {
                MegafansUI.Instance.ShowLandingWindow(false, MegafansPrefs.IsRegisteredMegaFansUser);
            }
        }

        public void UpdatePassword_OnClick()
        {
            if (!MegafansPrefs.IsRegisteredMegaFansUser)
            {
                MegafansUI.Instance.ShowUpdateProfileWindow();
            }
            else if (string.IsNullOrEmpty(MegafansPrefs.Email))
            {
                string msg = "Please link your email address before you are able to add or update your password. Would you like to link your email now?";

                MegafansUI.Instance.ShowAlertDialog(warningIcon, "Link Email",
                  msg, "Link Now", "Link Later",
                  () =>
                  {
                      MegafansUI.Instance.HideAlertDialog();
                      MegafansUI.Instance.ShowLandingWindow(false, MegafansPrefs.IsRegisteredMegaFansUser);
                  },
                  () =>
                  {
                      MegafansUI.Instance.HideAlertDialog();
                  });
            }
            else
            {
                MegafansUI.Instance.ShowUpdateProfileWindow();
            }
        }

        public void TermsOfUseAndPrivacyBtn_OnClick()
        {
            MegafansUI.Instance.ShowTermsOfUseOrPrivacyWindow(MegafansConstants.TERMS_OF_USE, false);
        }

        public void ProfilePic_OnClick()
        {
            Debug.Log("working here 0");
            imagePicker.PickImage((Texture2D tex, string imagePath) =>
            {
                Debug.Log("working here 3");
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
            Debug.Log("working here 4");
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
            if (MegafansPrefs.IsRegisteredMegaFansUser)
            {
                MegafansWebService.Instance.Logout(OnLogoutResponse, OnLogoutFailure);
                MegafansPrefs.ClearPrefs();
                MegafansPrefs.DeviceTokens = DeviceInfo.DeviceToken;
                MegafansUI.Instance.ShowOnboardingStartWindow();
            }
            else
            {
                string msg = "You are about to logout without saving your progress.  If you don't save your account, all progress, tokens, and history will be lost. \n<color=#FF4500>Are you sure you want to logout?</color>";

                MegafansUI.Instance.ShowAlertDialog(warningIcon, "Confirmation",
                  msg, "Log Out", "Cancel",
                  () =>
                  {
                      MegafansUI.Instance.HideAlertDialog();
                      MegafansWebService.Instance.Logout(OnLogoutResponse, OnLogoutFailure);
                      MegafansPrefs.ClearPrefs();
                      MegafansPrefs.DeviceTokens = DeviceInfo.DeviceToken;
                      MegafansUI.Instance.ShowOnboardingStartWindow();
                  },
                  () =>
                  {
                      MegafansUI.Instance.HideAlertDialog();
                  });
            }
        }

        public void DeleteAccount_OnClick()
        {
            if (MegafansPrefs.IsRegisteredMegaFansUser)
            {
                string msg = "You are about to Delete your account. If you do, all progress, tokens, and history will be lost. \n<color=#FF4500>Are you sure you want to delete account?</color>";

                MegafansUI.Instance.ShowAlertDialog(warningIcon, "Confirmation",
                  msg, "Delete Account", "Cancel",
                  () =>
                  {
                      MegafansWebService.Instance.deleteAccount(OnDeleteAccountResponse, OnLogoutFailure);
                      MegafansPrefs.ClearPrefs();
                      MegafansPrefs.DeviceTokens = DeviceInfo.DeviceToken;
                      MegafansUI.Instance.ShowOnboardingStartWindow();
                      MegafansUI.Instance.HideAlertDialog();
                  },
                  () =>
                  {
                      MegafansUI.Instance.HideAlertDialog();
                  });
            }
            else
            {
                string msg = "You are about to Delete your account. If you do, all progress, tokens, and history will be lost. \n<color=#FF4500>Are you sure you want to Delete?</color>";

                MegafansUI.Instance.ShowAlertDialog(warningIcon, "Confirmation",
                  msg, "Delete Account", "Cancel",
                  () =>
                  {
                      MegafansUI.Instance.HideAlertDialog();
                      MegafansWebService.Instance.Logout(OnLogoutResponse, OnLogoutFailure);
                      MegafansPrefs.ClearPrefs();
                      MegafansPrefs.DeviceTokens = DeviceInfo.DeviceToken;
                      MegafansUI.Instance.ShowOnboardingStartWindow();
                  },
                  () =>
                  {
                      MegafansUI.Instance.HideAlertDialog();
                  });
            }
        }

        void OnDeleteAccountResponse(DeleteAccountResponse response)
        {
            MegafansPrefs.UserId = 0;
            MegafansPrefs.ProfilePicUrl = "";
            MegafansPrefs.ClearPrefs();
            //MegafansWebService.Instance.FBLogout();
            MegafansUI.Instance.ShowOnboardingStartWindow();
            //Megafans.NativeWrapper.MegafanNativeWrapper.LogoutFromIntercom();
        }

        private void OnViewProfileResponse(ViewProfileResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                userNameLabel.text = response.data.username;
                MegafansPrefs.ProfilePicUrl = response.data.image;
                MegafansPrefs.Username = response.data.username;
                MegafansPrefs.UserStatus = response.data.status ?? 7;
                MegafansPrefs.CurrentTokenBalance = response.data.clientBalance;
                MegafansPrefs.FacebookID = response.data.facebookLoginId;

                MegafansWebService.Instance.FetchImage(response.data.image, OnFetchPicSuccess, OnFetchPicFailure);
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

        void OnFetchPicFailure(string error)
        {
            Debug.LogError(error);
        }

        void OnLogoutResponse(LogoutResponse response)
        {
            MegafansPrefs.UserId = 0;
            MegafansPrefs.ProfilePicUrl = "";
            MegafansPrefs.ClearPrefs();
            //MegafansWebService.Instance.FBLogout();
            MegafansUI.Instance.ShowOnboardingStartWindow();
            Megafan.NativeWrapper.MegafanNativeWrapper.LogoutFromIntercom();
        }

        private void OnLogoutFailure(string error)
        {
            Debug.LogError(error.ToString());
        }

        public void WithdrawBtnClicked()
        {
            Application.OpenURL(Megafans.Instance.WithdrawURL);
        }
    }

}