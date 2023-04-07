#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class EditProfileWindowUI : MonoBehaviour
    {
        [SerializeField] GameObject Container;
        [SerializeField] private Text userTournamentTokensTxt;
        [SerializeField] private Text userClientTokensTxt;
        [SerializeField] private InputField nameField;
        [SerializeField] private InputField phoneNumberField;
        [SerializeField] private InputField phoneNumberPrefixField;

        [SerializeField] private InputField emailAddressField;
        [SerializeField] private InputField confirmEmailAddressField;
        [SerializeField] private Button saveAccountBtn;

        private string addedEmailAddress;
        private string addedPhoneNumber;
        private string updatedUserName;

        void Awake()
        {
            if (Screen.orientation != ScreenOrientation.Portrait)
            {
                Container.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                Container.transform.position = new Vector3(Container.transform.position.x, Container.transform.position.y + 34, Container.transform.position.z);
            }
        }

        void OnEnable()
        {
            userTournamentTokensTxt.text = MegafansPrefs.TournamentEntryTokens.ToString();
            userClientTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
            nameField.text = "";

            MegafansWebService.Instance.ViewProfile("", OnViewProfileResponse, OnViewProfileFailure);
            int status = MegafansPrefs.UserStatus;
            if (!MegafansPrefs.IsRegisteredMegaFansUser)
            {
                emailAddressField.gameObject.SetActive(false);
                confirmEmailAddressField.gameObject.SetActive(false);
                phoneNumberField.gameObject.SetActive(false);
                phoneNumberPrefixField.gameObject.SetActive(false);
            }
        }

        public void UpdateCreditUI()
        {
            userTournamentTokensTxt.text = MegafansPrefs.TournamentEntryTokens.ToString();
            userClientTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }


        public void UpdatePassword_OnClick()
        {
            MegafansUI.Instance.ShowUpdateProfileWindow();
        }


        public void SaveBtn_OnClick()
        {
            if (!MegafansUtils.IsUsernameValid(nameField.text))
            {
                string msg = "Please enter your name.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);
                return;
            }
            if (nameField.text != MegafansPrefs.Username)
            {
                updatedUserName = nameField.text;
            }

            if (emailAddressField.gameObject.activeInHierarchy)
            {
                if (!string.IsNullOrEmpty(emailAddressField.text))
                {
                    addedEmailAddress = emailAddressField.text;
                }
            }

            if (phoneNumberField.gameObject.activeInHierarchy)
            {
                //if (!string.IsNullOrEmpty(phoneNumberField.text) && !string.IsNullOrEmpty(phoneNumberField.text)) {
                if (!string.IsNullOrEmpty(phoneNumberField.text))
                {
                    if (!MegafansUtils.IsPhoneNumberValid(phoneNumberField.text))
                    {
                        string msg = "Please enter your phone number.";
                        MegafansUI.Instance.ShowPopup("ERROR", msg);
                        return;
                    }
                   
                    addedPhoneNumber = "+1" + phoneNumberField.text;
                }
            }

            MegafansWebService.Instance.EditProfile(updatedUserName, addedPhoneNumber, addedEmailAddress,
                OnEditProfileResponse, OnEditProfileFailure);
        }

        public void SaveAcctBtn_OnClick()
        {
            MegafansUI.Instance.ShowLandingWindow(false);
        }

        public void BackBtn_OnClick()
        {
            MegafansUI.Instance.ShowMyAccountWindow();
        }


        public void LogoutBtn_OnClick()
        {
            MegafansUI.Instance.LogOutCurrentUser();
        }

        private void OnViewProfileResponse(ViewProfileResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                nameField.text = response.data.username;
                //MegafansWebService.Instance.FetchImage (response.data.image, OnFetchPicSuccess, OnFetchPicFailure);

                if (MegafansPrefs.IsRegisteredMegaFansUser)
                {
                    if (System.String.IsNullOrEmpty(response.data.email))
                    {
                        emailAddressField.gameObject.SetActive(true);
                        confirmEmailAddressField.gameObject.SetActive(true);
                        phoneNumberField.gameObject.SetActive(false);
                        phoneNumberPrefixField.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (System.String.IsNullOrEmpty(response.data.phoneNumber) && MegafansPrefs.SMSAvailable)
                        {
                            emailAddressField.gameObject.SetActive(false);
                            confirmEmailAddressField.gameObject.SetActive(false);
                            phoneNumberField.gameObject.SetActive(true);
                            phoneNumberPrefixField.gameObject.SetActive(true);
                        }
                        else
                        {
                            confirmEmailAddressField.gameObject.SetActive(false);
                            emailAddressField.gameObject.SetActive(false);
                            phoneNumberField.gameObject.SetActive(false);
                            phoneNumberPrefixField.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    emailAddressField.gameObject.SetActive(false);
                    confirmEmailAddressField.gameObject.SetActive(false);
                    phoneNumberField.gameObject.SetActive(false);
                    phoneNumberPrefixField.gameObject.SetActive(false);
                }
            }
        }

        private void OnViewProfileFailure(string error)
        {
            Debug.LogError(error);
        }

        private void OnEditProfileResponse(EditProfileResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                if (!string.IsNullOrEmpty(updatedUserName))
                {
                    MegafansPrefs.Username = updatedUserName;
                    Megafan.NativeWrapper.MegafanNativeWrapper.UpdateUsernameToIntercom(updatedUserName);
                    updatedUserName = null;
                }

                if (!string.IsNullOrEmpty(addedEmailAddress))
                {
                    MegafansPrefs.Email = addedEmailAddress;
                    addedEmailAddress = null;
                }

                if (!string.IsNullOrEmpty(addedPhoneNumber))
                {
                    MegafansPrefs.PhoneNumber = addedPhoneNumber;
                    MegafansUI.Instance.ShowVerifyPhoneWindowFromEdit(addedPhoneNumber, () =>
                    {
                        MegafansUI.Instance.ShowEditProfileWindow();
                    });
                    addedPhoneNumber = null;
                    MegafansWebService.Instance.ViewProfile("", OnViewProfileResponse, OnViewProfileFailure);
                }
                else
                {
                    MegafansUI.Instance.ShowTournamentLobby();
                }
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

    }
}