#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using MegafansSDK.Utils;

namespace MegafansSDK.UI {

	public class EditProfileWindowUI : MonoBehaviour {

        [SerializeField] private Text userTokensTxt;
        [SerializeField] private InputField nameField;
        [SerializeField] private InputField phoneNumberField;
        [SerializeField] private InputField phoneNumberPrefixField;

        [SerializeField] private InputField emailAddressField;
        [SerializeField] private InputField confirmEmailAddressField;
        [SerializeField] private RawImage inputFieldHighlightedImage;
        [SerializeField] private Button saveAccountBtn;
        private Sprite activeInputBackground;
        private Sprite inactiveInputBackground;

        private string addedEmailAddress;
        private string addedPhoneNumber;
        private string updatedUserName;

        void Awake()
        {
            Texture2D activeSpriteTexture = (Texture2D)inputFieldHighlightedImage.texture;
            inactiveInputBackground = emailAddressField.image.sprite;
            activeInputBackground = Sprite.Create(activeSpriteTexture, new Rect(0, 0, activeSpriteTexture.width, activeSpriteTexture.height), new Vector2(0.5f, 0.5f));
        }

        void OnEnable() {
            userTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
            nameField.text = "";
            nameField.image.sprite = inactiveInputBackground;
            emailAddressField.image.sprite = inactiveInputBackground;
            confirmEmailAddressField.image.sprite = inactiveInputBackground;
            phoneNumberPrefixField.image.sprite = inactiveInputBackground;
            phoneNumberField.image.sprite = inactiveInputBackground;

            MegafansWebService.Instance.ViewProfile ("", OnViewProfileResponse, OnViewProfileFailure);
            int status = MegafansPrefs.UserStatus;
            if (!MegafansPrefs.IsRegisteredMegaFansUser) {           
                emailAddressField.gameObject.SetActive(false);
                confirmEmailAddressField.gameObject.SetActive(false);
                phoneNumberField.gameObject.SetActive(false);
                phoneNumberPrefixField.gameObject.SetActive(false);
            }

            //phoneNumberField.text = "9709858807";
		}

		void Start() {
			
		}

		//public void BackBtn_OnClick() {
		//	MegafansUI.Instance.ShowTournamentLobby ();
		//}

        public void UpdatePassword_OnClick() {
            MegafansUI.Instance.ShowUpdateProfileWindow();
        }

//        public void ProfilePic_OnClick() {
//#if UNITY_EDITOR
//            Debug.Log("Unity Editor");

//            ////if (tex != null)
//            ////{
//            //    //isPhotoRemoved = false;
//            //    //profilePicImg.texture = tex;               
//            //    StartCoroutine(MegafansWebService.Instance.UploadProfilePic("/Users/markhoyt/Downloads/IMG_3956.PNG", (obj) =>
//            //    {
//            //        string msg = "Successfully uploaded profile picture.";
//            //        MegafansUI.Instance.ShowPopup("SUCCESS", msg);
//            //    }, (err) =>
//            //    {
//            //        string msg = "Error uploading profile image.  Please try again.";
//            //        MegafansUI.Instance.ShowPopup("ERROR", msg);
//            //    }));
//            //return;
		//          imagePicker.PickImage ((Texture2D tex, string imagePath) => {
		//		if(tex != null) {
		//			isPhotoRemoved = false;
		//			profilePicImg.texture = tex;
  //                  string newImageToUpload = MegafansUtils.TextureToString((Texture2D)profilePicImg.texture);
  //                  MegafansPrefs.ProfilePic = newImageToUpload;
  //                  StartCoroutine(MegafansWebService.Instance.UploadProfilePic(tex, (obj) =>
  //                  {
  //                      string msg = "Successfully uploaded profile picture.";
  //                      MegafansUI.Instance.ShowPopup("SUCCESS", msg);
  //                  }, (err) =>
  //                  {
  //                      string msg = "Error uploading profile image.  Please try again.";
  //                      MegafansUI.Instance.ShowPopup("ERROR", msg);
  //                  }));                    
  //              } else {
  //                  string msg = "Error uploading profile image.  Please try again.";
  //                  MegafansUI.Instance.ShowPopup("ERROR", msg);
  //              }
		//	}, 128, "Select your profile photo");
		//}

		//public void RemovePicBtn_OnClick() {
		//	isPhotoRemoved = true;
		//	profilePicImg.texture = picPlaceholder;
		//}

		public void SaveBtn_OnClick() {
			if (!MegafansUtils.IsUsernameValid(nameField.text)) {
				string msg = "Please enter your name.";
				MegafansUI.Instance.ShowPopup ("ERROR", msg);
				return;
			}
            if (nameField.text != MegafansPrefs.Username) {
                updatedUserName = nameField.text;
            }

            if (emailAddressField.gameObject.activeInHierarchy) {
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
                    //else if (!MegafansUtils.IsPhoneNumberValid(phoneNumberPrefixField.text))
                    //{
                    //    string msg = "Please enter your country code.";
                    //    MegafansUI.Instance.ShowPopup("ERROR", msg);
                    //    return;
                    //}
                    //string prefixText = phoneNumberPrefixField.text;
                    //if (prefixText.StartsWith("+", StringComparison.Ordinal))
                    //{
                    //addedPhoneNumber = phoneNumberPrefixField.text + phoneNumberField.text;
                    //}
                    //else
                    //{
                    //addedPhoneNumber = "+" + phoneNumberPrefixField.text + phoneNumberField.text;
                    //}
                    addedPhoneNumber = "+1" + phoneNumberField.text;
                }               
            }

            MegafansWebService.Instance.EditProfile (updatedUserName, addedPhoneNumber, addedEmailAddress,
				OnEditProfileResponse, OnEditProfileFailure);
		}

        public void SaveAcctBtn_OnClick() {
            MegafansUI.Instance.ShowLandingWindow(false);
        }

        public void BackBtn_OnClick() {
            MegafansUI.Instance.ShowMyAccountWindow();
        }

        public void LinkFBBtn_OnClick()
        {
            MegafansWebService.Instance.LinkFB(OnEditProfileResponse, OnEditProfileFailure);
        }

        public void LogoutBtn_OnClick() {
            MegafansWebService.Instance.Logout (OnLogoutResponse, OnLogoutFailure);
            MegafansPrefs.ClearPrefs();
            MegafansUI.Instance.ShowOnboardingStartWindow();
        }

		private void OnViewProfileResponse(ViewProfileResponse response) {
			if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
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
                } else {
                    emailAddressField.gameObject.SetActive(false);
                    confirmEmailAddressField.gameObject.SetActive(false);
                    phoneNumberField.gameObject.SetActive(false);
                    phoneNumberPrefixField.gameObject.SetActive(false);
                }
			}
		}

		private void OnViewProfileFailure(string error) {
			Debug.LogError (error);
		}

		private void OnEditProfileResponse(EditProfileResponse response) {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE)) {
                if (!string.IsNullOrEmpty(updatedUserName)) {
                    MegafansPrefs.Username = updatedUserName;
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
			else {
				string msg = "Failed to save changes.";
				MegafansUI.Instance.ShowPopup ("ERROR", msg);
			}
		}

		private void OnEditProfileFailure(string error) {
			Debug.LogError (error);
            string errorString = "Error updating profile.  Please try again.";
            MegafansUI.Instance.ShowPopup("Error", errorString);
        }

		//private void OnFetchPicSuccess(Texture2D tex) {
		//	if (tex != null) {
		//		if (!isPhotoRemoved) {
		//			profilePicImg.texture = tex;
		//		}
		//	}
		//}

		//private void OnFetchPicFailure(string error) {
		//	Debug.LogError (error);
		//}

        private void OnLogoutResponse(LogoutResponse response) {
			//if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
			MegafansPrefs.UserId = 0;
			MegafansPrefs.ProfilePicUrl = "";
			MegafansPrefs.ClearPrefs ();
			MegafansWebService.Instance.FBLogout ();
			MegafansUI.Instance.ShowOnboardingStartWindow();
			//}
		}

		private void OnLogoutFailure(string error) {
            Debug.LogError (error.ToString());
		}

        void Update()
        {
            if (nameField.GetComponent<InputField>().isFocused == true && nameField.image.sprite != activeInputBackground)
            {
                nameField.image.sprite = activeInputBackground;
            }

            if (emailAddressField.GetComponent<InputField>().isFocused == true && emailAddressField.image.sprite != activeInputBackground)
            {
                emailAddressField.image.sprite = activeInputBackground;
            }

            if (confirmEmailAddressField.GetComponent<InputField>().isFocused == true && confirmEmailAddressField.image.sprite != activeInputBackground)
            {
                confirmEmailAddressField.image.sprite = activeInputBackground;
            }

            if (phoneNumberField.GetComponent<InputField>().isFocused == true && phoneNumberField.image.sprite != activeInputBackground)
            {
                phoneNumberField.image.sprite = activeInputBackground;
            }

            if (phoneNumberPrefixField.GetComponent<InputField>().isFocused == true && phoneNumberPrefixField.image.sprite != activeInputBackground)
            {
                phoneNumberPrefixField.image.sprite = activeInputBackground;
            }
        }
    }

}