#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class RegistrationPhoneUI : MonoBehaviour
    {

        [SerializeField] private Text saveBtnText;
        [SerializeField] private GameObject termsAndConditionsView;
        [SerializeField] private InputField phoneNumberField;
        [SerializeField] private InputField phoneNumberPrefixField;
        [SerializeField] private RegistrationWindowUI mainRegistrationUI;
        [SerializeField] private Image isValidPrefixImage;
        [SerializeField] private Image isValidPhoneNumberImage;

        private void Awake()
        {
        }

        string combinedCountryCodeAndPhoneNumber;
        private bool IsFormValid
        {
            get
            {
                bool isValidNumber = MegafansUtils.IsPhoneNumberValid(phoneNumberField.text);
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

        public void ContinueBtn_OnClick()
        {
            if (!MegafansUtils.IsPhoneNumberValid(phoneNumberField.text))
            {
                string msg = "Please enter your phone number.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);
                return;
            }

            combinedCountryCodeAndPhoneNumber = "+1" + phoneNumberField.text;
            MegafansPrefs.PhoneNumber = combinedCountryCodeAndPhoneNumber;

            if (!mainRegistrationUI.IsLinking && !mainRegistrationUI.IsAgreementAcceptedPhone)
            {
                string msg = "Please accept our Terms of Use and Privacy Policy.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);

                return;
            }
            MegafansWebService.Instance.EditProfile(MegafansPrefs.Username, combinedCountryCodeAndPhoneNumber, null,
                OnEditProfileResponse, OnEditProfileFailure);
        }

        private void OnEditProfileResponse(EditProfileResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansUI.Instance.ShowVerifyPhoneWindow(combinedCountryCodeAndPhoneNumber, true, () =>
                {
                    phoneNumberField.text = "";
                    MegafansPrefs.PhoneNumber = "";
                    MegafansUI.Instance.ShowRegistrationWindow(false, true);
                });
            }
            else
            {
                string msg = "Failed to save changes.";
                if (!string.IsNullOrEmpty(response.message))
                {
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
    }
}