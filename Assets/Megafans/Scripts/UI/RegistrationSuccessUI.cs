using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{
    public class RegistrationSuccessUI : MonoBehaviour
    {
        [SerializeField] private Button resendEmailButton;
        [SerializeField] private Text emailInstructions;
        public bool IsEmail = true;

        void Awake() { 
            if (IsEmail) {
                resendEmailButton.gameObject.SetActive(true);
                emailInstructions.gameObject.SetActive(true);
            } else {
                resendEmailButton.gameObject.SetActive(false);
                emailInstructions.gameObject.SetActive(false);
            }
        }

        private void OnEnable() {
            if (IsEmail)
            {
                resendEmailButton.gameObject.SetActive(true);
                emailInstructions.gameObject.SetActive(true);
            }
            else
            {
                resendEmailButton.gameObject.SetActive(false);
                emailInstructions.gameObject.SetActive(false);
            }
        }

        public void ResendEmailBtn_OnClick() {
            MegafansUI.Instance.ShowLoadingBar();
            MegafansWebService.Instance.ResendEmailVerification(OnResendVerificationResponse, OnResendVerificationFailure);
        }

        private void OnResendVerificationResponse(Response response)
        {
            MegafansUI.Instance.HideLoadingBar();
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                string msg = "Please check your email for a new verification link.";
                MegafansUI.Instance.ShowPopup ("Success", msg);
            }
            else
            {
                MegafansUI.Instance.ShowPopup("ERROR", response.message);
            }
        }

        private void OnResendVerificationFailure(string error)
        {
            MegafansUI.Instance.HideLoadingBar();
            Debug.LogError(error);
            MegafansUI.Instance.ShowPopup("ERROR", error);
        }


        public void ContinueBtn_OnClick() {
            MegafansUI.Instance.ShowTournamentLobby();
        }
    }
}