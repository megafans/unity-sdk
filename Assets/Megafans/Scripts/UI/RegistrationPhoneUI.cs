#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

using MegafansSDK.Utils;

namespace MegafansSDK.UI {

	public class RegistrationPhoneUI : MonoBehaviour {

        [SerializeField] private Text saveBtnText;
        [SerializeField] private GameObject termsAndConditionsView;
        [SerializeField] private InputField phoneNumberField;
        [SerializeField] private InputField phoneNumberPrefixField;
        [SerializeField] private RegistrationWindowUI mainRegistrationUI;
        [SerializeField] private RawImage inputFieldHighlightedImage;
        [SerializeField] private RawImage prefixInputFieldHighlightedImage;
        [SerializeField] private Image isValidPrefixImage;
        [SerializeField] private Image isValidPhoneNumberImage;

        private Sprite activeInputBackground;
        private Sprite activePrefixInputBackground;

        private void Awake()
        {
            Texture2D spriteTexture = (Texture2D)inputFieldHighlightedImage.texture;
            activeInputBackground = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0.5f));

            Texture2D spriteTexture2 = (Texture2D)prefixInputFieldHighlightedImage.texture;
            activePrefixInputBackground = Sprite.Create(spriteTexture2, new Rect(0, 0, spriteTexture2.width, spriteTexture2.height), new Vector2(0.5f, 0.5f));
        }

        string combinedCountryCodeAndPhoneNumber;
        private bool IsFormValid
        {
            get
            {
                //bool isValidPrefix = MegafansUtils.IsPhoneNumberPrefixValid(phoneNumberPrefixField.text);
                bool isValidNumber = MegafansUtils.IsPhoneNumberValid(phoneNumberField.text);
                //return isValidPrefix && isValidNumber;
                return isValidNumber;
            }
        }

        private void OnEnable()
        {
            if (mainRegistrationUI.IsLinking)
            {
                saveBtnText.text = "LINK ACCOUNT";
                termsAndConditionsView.SetActive(false);
            }
            else
            {
                saveBtnText.text = "CREATE ACCOUNT";
                termsAndConditionsView.SetActive(true);
            }

        }

        public void ContinueBtn_OnClick() {
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
            //    combinedCountryCodeAndPhoneNumber = phoneNumberPrefixField.text + phoneNumberField.text;
            //}
            //else
            //{
            //    combinedCountryCodeAndPhoneNumber = "+" + phoneNumberPrefixField.text + phoneNumberField.text;
            //}
            combinedCountryCodeAndPhoneNumber = "+1" + phoneNumberField.text;
            MegafansPrefs.PhoneNumber = combinedCountryCodeAndPhoneNumber;

			if (!mainRegistrationUI.IsLinking && !mainRegistrationUI.IsAgreementAcceptedPhone) {
				string msg = "Please accept our Terms of Use and Privacy Policy.";
				MegafansUI.Instance.ShowPopup ("ERROR", msg);

				return;
			}
            MegafansWebService.Instance.EditProfile(MegafansPrefs.Username, combinedCountryCodeAndPhoneNumber, null,
                OnEditProfileResponse, OnEditProfileFailure);
		}

        //private void OnRegisterPhoneResponse(RegisterResponse response) {
        //	if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
        //		MegafansUI.Instance.ShowVerifyPhoneWindow (phoneNumberField.text, true, () => {
        //			//MegafansUI.Instance.ShowRegistrationWindow ();
        //		});
        //	}
        //	else {
        //		MegafansUI.Instance.ShowPopup ("ERROR", response.message);
        //	}
        //}

        //private void OnRegisterPhoneFailure(string error) {
        //	Debug.LogError (error);
        //}

        private void OnEditProfileResponse(EditProfileResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                //MegafansPrefs.PhoneNumber = "+" + phoneNumberPrefixField.text + phoneNumberField.text;
                MegafansUI.Instance.ShowVerifyPhoneWindow (combinedCountryCodeAndPhoneNumber, true, () => {
                    Megafans.Instance.ReportUserRegistered(MegafansPrefs.UserId.ToString());
                    MegafansUI.Instance.ShowRegistrationWindow(false);
                });

            }
            else
            {
                string msg = "Failed to save changes.";
                if (!string.IsNullOrEmpty(response.message)) {
                    msg = response.message;
                }
                MegafansUI.Instance.ShowPopup("ERROR", msg);
            }
        }

        private void OnEditProfileFailure(string error)
        {
            Debug.LogError(error);
            string errorString = "Error updating profile.  Please try again.";
            MegafansUI.Instance.ShowPopup("Error", errorString);
        }

        public void onPhoneNumberPrefixInputChange()
        {

            if (MegafansUtils.IsPhoneNumberPrefixValid(phoneNumberPrefixField.text))
            {
                isValidPrefixImage.gameObject.SetActive(true);
            }
            else
            {
                isValidPrefixImage.gameObject.SetActive(false);
            }
        }

        public void onPhoneNumberInputChange()
        {

            if (MegafansUtils.IsPhoneNumberValid(phoneNumberField.text))
            {

                isValidPhoneNumberImage.gameObject.SetActive(true);
            }
            else
            {
                isValidPhoneNumberImage.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if (phoneNumberField.GetComponent<InputField>().isFocused == true && phoneNumberField.image.sprite != activeInputBackground)
            {
                phoneNumberField.image.sprite = activeInputBackground;
            }

            if (phoneNumberPrefixField.GetComponent<InputField>().isFocused == true && phoneNumberPrefixField.image.sprite != activePrefixInputBackground)
            {
                phoneNumberPrefixField.image.sprite = activePrefixInputBackground;
            }
        }
    }

}