﻿#pragma warning disable 649

using UnityEngine;
using System;
using UnityEngine.UI;

using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class LoginPhoneUI : MonoBehaviour
    {

        [SerializeField] private InputField phoneNumberField;
        [SerializeField] private Button loginBtn;

        private bool IsFormValid
        {
            get
            {
                bool isValidNumber = MegafansUtils.IsPhoneNumberValid(phoneNumberField.text);
                //return MegafansUtils.IsPhoneNumberPrefixValid(phoneNumberPrefixField.text) && MegafansUtils.IsPhoneNumberValid (phoneNumberField.text);
                return isValidNumber;
                //return MegafansUtils.IsPhoneNumberValid(phoneNumberField.text);
            }
        }

        void Awake()
        {
            loginBtn.interactable = false;
            //phoneNumberField.text = "9709858807";
        }

        private void OnEnable()
        {
            phoneNumberField.text = "";
        }

        public void PhoneNumberPrefixField_OnValueChanged()
        {
            loginBtn.interactable = IsFormValid;
        }

        public void PhoneNumberField_OnValueChanged()
        {
            loginBtn.interactable = IsFormValid;
        }

        public void ContinueBtn_OnClick()
        {
            if (!MegafansUtils.IsPhoneNumberValid(phoneNumberField.text))
            {
                string msg = "Please enter your phone number.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);
                return;
            }
            MegafansPrefs.PhoneNumber = "+1" + phoneNumberField.text;
            MegafansWebService.Instance.LoginPhone(MegafansPrefs.PhoneNumber, OnLoginPhoneResponse,
                OnLoginPhoneFailure);
        }

        private void OnLoginPhoneResponse(LoginResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansUI.Instance.ShowVerifyPhoneWindow(MegafansPrefs.PhoneNumber, false, () =>
                {
                    MegafansUI.Instance.ShowLoginWindow(true);
                });
            }
            else
            {
                MegafansUI.Instance.ShowPopup("ERROR", response.message);
            }
        }

        private void OnLoginPhoneFailure(string error)
        {
            Debug.LogError(error);
        }
    }
}