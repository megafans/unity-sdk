﻿#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MegafansSDK.Utils;

namespace MegafansSDK.UI {

    public class UpdatePasswordWindowUI : MonoBehaviour {

        [SerializeField] Text userTokensTxt;
        [SerializeField] InputField oldPasswordField;
        [SerializeField] InputField newPasswordField;
        [SerializeField] InputField newPasswordConfirmField;
        [SerializeField] Button showCurrentPasswordBtn;
        [SerializeField] Button showNewPasswordBtn;
        [SerializeField] Button showConfirmPasswordBtn;
        [SerializeField] Button savePasswordBtn;

        bool IsFormValid
            => MegafansUtils.IsPasswordValid(oldPasswordField.text) &&
               MegafansUtils.IsPasswordValid(newPasswordField.text) &&
               MegafansUtils.IsPasswordValid(newPasswordConfirmField.text) &&
               newPasswordField.text == newPasswordConfirmField.text &&
               newPasswordField.text != oldPasswordField.text;


        void Awake()
        {
           
        }


        void OnEnable() {
            userTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
            oldPasswordField.text = "";
            newPasswordField.text = "";
            newPasswordConfirmField.text = "";
        }

        public void UpdateCreditUI()
        {
            userTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        void OnUpdatePasswordResponse(UpdatePasswordResponse response) {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE)) {
                Debug.Log("Password Updated Successfully");
                MegafansUI.Instance.ShowPopup("SUCCESS", "Successfully updated your password.");
                MegafansUI.Instance.ShowMyAccountWindow();
            }
            else {
                string msg = "Failed to update password.  Please try again.";
                if (!string.IsNullOrEmpty(response.message)) {
                    msg = response.message;
                }
                MegafansUI.Instance.ShowPopup("ERROR", msg);
            }
        }

        void OnUpdatePasswordFailure(string error) {
            Debug.LogError(error);
        }


        //Called from each password field
        public void PasswordField_OnValueChanged() {
            savePasswordBtn.interactable = IsFormValid;
        }

        public void BackBtn_OnClick() {
            MegafansUI.Instance.ShowMyAccountWindow();
        }

        public void ShowCurrentPasswordBtn_OnClick() {
            if (oldPasswordField.contentType == InputField.ContentType.Password) {
                oldPasswordField.contentType = InputField.ContentType.Standard;
                showCurrentPasswordBtn.GetComponentInChildren<Text>().text = "HIDE";
            }
            else {
                oldPasswordField.contentType = InputField.ContentType.Password;
                showCurrentPasswordBtn.GetComponentInChildren<Text>().text = "SHOW";
            }
            oldPasswordField.ForceLabelUpdate();
        }

        public void ShowNewPasswordBtn_OnClick() {
            if (newPasswordField.contentType == InputField.ContentType.Password) {
                newPasswordField.contentType = InputField.ContentType.Standard;
                showNewPasswordBtn.GetComponentInChildren<Text>().text = "HIDE";
            }
            else {
                newPasswordField.contentType = InputField.ContentType.Password;
                showNewPasswordBtn.GetComponentInChildren<Text>().text = "SHOW";
            }
            newPasswordField.ForceLabelUpdate();
        }

        public void ShowConfirmPasswordBtn_OnClick() {
            if (newPasswordConfirmField.contentType == InputField.ContentType.Password) {
                newPasswordConfirmField.contentType = InputField.ContentType.Standard;
                showConfirmPasswordBtn.GetComponentInChildren<Text>().text = "HIDE";
            }
            else {
                newPasswordConfirmField.contentType = InputField.ContentType.Password;
                showConfirmPasswordBtn.GetComponentInChildren<Text>().text = "SHOW";
            }
            newPasswordConfirmField.ForceLabelUpdate();
        }

        public void UpdateBtn_OnClick() {
            if (!MegafansUtils.IsPasswordValid(oldPasswordField.text)) {
                string msg = "Must enter your last password";
                MegafansUI.Instance.ShowPopup("ERROR", msg);
                return;
            }

            if (!MegafansUtils.IsPasswordValid(newPasswordField.text)) {
                string msg = "Please enter a new password.  New password must be at least 8 characters long.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);
                return;
            }

            if (newPasswordField.text != newPasswordConfirmField.text) {
                string msg = "Password and confirm password do not match.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);
                return;
            }

            MegafansWebService.Instance.UpdatePassword(oldPasswordField.text, newPasswordField.text, OnUpdatePasswordResponse, OnUpdatePasswordFailure);
        }
    }
}