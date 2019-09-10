using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.Utils;

namespace MegafansSDK.UI {

	public class RegistrationWindowUI : MonoBehaviour {

        [SerializeField] private Text titleText;
        [SerializeField] private GameObject emailSection;
		[SerializeField] private GameObject phoneSection;
		[SerializeField] private Toggle agreementCheckboxPhone;
		[SerializeField] private Toggle agreementCheckboxEmail;

        public bool IsAgreementAcceptedPhone {
			get {
				return agreementCheckboxPhone.isOn;
			}
		}

		public bool IsAgreementAcceptedEmail {
			get {
				return agreementCheckboxEmail.isOn;
			}
		}

        public bool IsEmail = true;
        public bool IsLinking = false;

        void Awake() {
            if (IsEmail)
            {
                phoneSection.SetActive(false);
                emailSection.SetActive(true);
            } else {
                phoneSection.SetActive(true);
                emailSection.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (IsEmail)
            {
                phoneSection.SetActive(false);
                emailSection.SetActive(true);
            }
            else
            {
                phoneSection.SetActive(true);
                emailSection.SetActive(false);
            }
            if (IsLinking)
            {
                titleText.text = "Link your account";
            }
            else
            {
                titleText.text = "Create your account";
            }
        }

        public void FBBtn_OnClick() {
			MegafansWebService.Instance.RegisterFB (OnRegisterFBResponse, OnRegisterFBFailure);
		}

		public void PhoneBtn_OnClick() {
			emailSection.SetActive (false);
			phoneSection.SetActive (true);
		}

		public void EmailBtn_OnClick() {
			phoneSection.SetActive (false);
			emailSection.SetActive (true);
		}

		public void TermsOfUseBtn_OnClick() {
            MegafansUI.Instance.ShowTermsOfUseOrPrivacyWindow(MegafansConstants.TERMS_OF_USE, true);
            //if (webViewScreen == null) {
            //    webViewScreen = (new GameObject("WebView")).AddComponent<WebView>();
            //} else {
            //    Destroy(webViewScreen);
            //    webViewScreen = (new GameObject("WebView")).AddComponent<WebView>();
            //}
            //webViewScreen.TitleTextValue = "Terms of use";
            //webViewScreen.Url = MegafansConstants.TERMS_OF_USE_URL;
        }

		public void PrivacyPolicyBtn_OnClick() {
            //if (webViewScreen == null) {
            //    webViewScreen = (new GameObject("WebView")).AddComponent<WebView>();
            //} else {
            //    Destroy(webViewScreen);
            //    webViewScreen = (new GameObject("WebView")).AddComponent<WebView>();
            //}
            //webViewScreen.TitleTextValue = "Privacy Policy";
            //webViewScreen.Url = MegafansConstants.PRIVACY_POLICY_URL;
            //MegafansUI.Instance.ShowTermsOfUseOrRulesWindow();
            MegafansUI.Instance.ShowTermsOfUseOrPrivacyWindow(MegafansConstants.PRIVACY_POLICY, true);
        }

		public void LearnMoreBtn_OnClick() {
			Application.OpenURL (MegafansConstants.LEARN_MORE_URL);
		}

		public void BackBtn_OnClick() {
            MegafansUI.Instance.ShowLandingWindow (false, IsLinking);
		}

        public void LoginBtn_OnClickEmail() {
            MegafansUI.Instance.ShowLoginWindow(true);
        }

        public void LoginBtn_OnClickPhone()
        {
            MegafansUI.Instance.ShowLoginWindow(false);
        }

        private void OnRegisterFBResponse(RegisterResponse response) {
			if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
                Debug.Log("***************FB LOGIN******************");
                Debug.Log(response.data);
                Debug.Log("*********************************");
                MegafansPrefs.UserId = response.data.id;
                string msg = "You have registered successfully.";
				MegafansUI.Instance.ShowPopup ("SUCCESS", msg);
                MegafansPrefs.Email = response.data.email;
                MegafansUI.Instance.ShowLoginWindow (true);

                Megafans.Instance.ReportUserRegistered(MegafansPrefs.UserId.ToString());
            }
			else {
				Debug.LogError (response.message);
				string error = "There was an error: " + response.message;
				MegafansUI.Instance.ShowPopup ("ERROR", error);
			}
		}

		private void OnRegisterFBFailure(string error) {
			Debug.LogError (error);
			MegafansUI.Instance.ShowPopup ("ERROR", error);
		}
	}
}