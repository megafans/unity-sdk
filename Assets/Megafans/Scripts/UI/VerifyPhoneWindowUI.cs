﻿#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class VerifyPhoneWindowUI : MonoBehaviour
    {

        [SerializeField] private InputField otpField;
        [SerializeField] private Button backBtn;

        private void Awake()
        {
        }

        public bool IsRegistering
        {
            get;
            set;
        }

        public bool IsLinking
        {
            get;
            set;
        }

        public string PhoneNumberToVerify
        {
            get;
            set;
        }

        void OnEnable()
        {
            otpField.text = "";
        }

        public void VerifyBtn_OnClick()
        {
            if (otpField.text == "")
            {
                string msg = "Please enter your OTP.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);

                return;
            }

            MegafansWebService.Instance.VerifyOtp(otpField.text, PhoneNumberToVerify, OnVerifyPhoneResponse,
                OnVerifyPhoneFailure, IsRegistering);
        }

        public void ResendBtn_OnClick()
        {
            MegafansWebService.Instance.LoginPhone(PhoneNumberToVerify, OnResendOTPResponse,
                OnResendOTPFailure);
        }

        public void SetBackBtnAction(UnityAction action)
        {
            backBtn.onClick.RemoveAllListeners();
            backBtn.onClick.AddListener(action);
        }

        private void OnVerifyPhoneResponse(LoginResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                if (IsRegistering)
                {
                    MegafansUI.Instance.ShowRegistrationSuccessWindow(false);
                    Megafans.Instance.ReportUserRegistered(MegafansPrefs.UserId.ToString());
                }
                else if (IsLinking)
                {
                    MegafansPrefs.UserId = response.data.id;
                    MegafansPrefs.ProfilePicUrl = response.data.image;
                    MegafansPrefs.AccessToken = response.data.token;
                    MegafansPrefs.RefreshToken = response.data.refresh;
                    MegafansUI.Instance.BackFromVerifyOTP();
                }
                else
                {
                    MegafansPrefs.UserId = response.data.id;
                    MegafansPrefs.ProfilePicUrl = response.data.image;
                    MegafansPrefs.AccessToken = response.data.token;
                    MegafansPrefs.RefreshToken = response.data.refresh;
                    Megafans.Instance.ReportUserLoggedIn(MegafansPrefs.UserId.ToString());
                }
            }
            else
            {
                string msg = "Invalid OTP.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);
            }
        }

        private void OnVerifyPhoneFailure(string error)
        {
            Debug.LogError(error);
        }

        private void OnResendOTPResponse(LoginResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                string msg = "OTP has been resent successfully.";
                MegafansUI.Instance.ShowPopup("OTP RESENT", msg);
            }
        }

        private void OnResendOTPFailure(string error)
        {
            Debug.LogError(error);
            MegafansUI.Instance.ShowPopup("OTP RESENT", error);
        }
    }
}