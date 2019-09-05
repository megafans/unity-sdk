using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MegafansSDK.UI {

	public class LandingScreenUI : MonoBehaviour {

		[SerializeField] private GameObject background;
		[SerializeField] private GameObject landingWindow;
		[SerializeField] private GameObject registrationWindow;
		[SerializeField] private GameObject verifyPhoneWindow;
		[SerializeField] private GameObject loginWindow;
        [SerializeField] private GameObject registrationSuccessWindow;
        [SerializeField] private GameObject termsOfUseAndRulesWindow;

        private List<GameObject> windows;

		void Awake() {
			FillWindows ();
		}

		public void ShowLandingWindow(bool isLogin = false) {
            this.HideAllWindows();
            LandingWindowUI handler = landingWindow.GetComponent<LandingWindowUI>();
            if (handler != null)
            {
                handler.IsLogin = isLogin;
            }
            ShowWindow (landingWindow);
		}

		public void ShowRegistrationWindowPhone() {
            RegistrationWindowUI handler = registrationWindow.GetComponent<RegistrationWindowUI>();
            if (handler != null)
            {
                handler.IsEmail = false;
            }
            ShowWindow(registrationWindow);
        }

        public void ShowRegistrationWindowEmail() {
            RegistrationWindowUI handler = registrationWindow.GetComponent<RegistrationWindowUI>();
            if (handler != null)
            {
                handler.IsEmail = true;
            }
            ShowWindow(registrationWindow);
        }

        public void ShowLoginWindowEmail()
        {
            LoginWindowUI handler = loginWindow.GetComponent<LoginWindowUI>();
            if (handler != null)
            {
                handler.IsEmail = true;
            }
            ShowWindow(loginWindow);
        }

        public void ShowLoginWindowPhone() {
            LoginWindowUI handler = loginWindow.GetComponent<LoginWindowUI>();
            if (handler != null)
            {
                handler.IsEmail = false;
            }
            ShowWindow(loginWindow);
        }

        public void ShowVerifyPhoneWindow(string phoneNumberToVerify, bool isRegistering, UnityAction backBtnAction) {
			ShowWindow (verifyPhoneWindow);

			VerifyPhoneWindowUI handler = verifyPhoneWindow.GetComponent<VerifyPhoneWindowUI> ();
			if(handler != null) {
				handler.PhoneNumberToVerify = phoneNumberToVerify;
				handler.IsRegistering = isRegistering;
				handler.SetBackBtnAction (backBtnAction);
			}
		}

        public void ShowRegistrationSuccessWindow(bool isEmail) {        
            RegistrationSuccessUI handler = registrationSuccessWindow.GetComponent<RegistrationSuccessUI>();
            if (handler != null)
            {
                handler.IsEmail = isEmail;
            }
            ShowWindow(registrationSuccessWindow);
        }

        public void ShowTermsOfUseOrPrivacyWindow(string informationType)
        {
            TermsOfUseAndRulesWindow handler = termsOfUseAndRulesWindow.GetComponent<TermsOfUseAndRulesWindow>();
            if (handler != null)
            {
                handler.Init(informationType, true);
            }
            termsOfUseAndRulesWindow.gameObject.SetActive(true);
        }

        public void HideTermsOfUseOrPrivacyWindow() {
            termsOfUseAndRulesWindow.gameObject.SetActive(false);
        }

        private void FillWindows() {
			if (windows == null) {
				windows = new List<GameObject> ();
			}
			else {
				windows.Clear ();
			}

			windows.Add (background);
			windows.Add (landingWindow);
			windows.Add (registrationWindow);
			windows.Add (verifyPhoneWindow);
			windows.Add (loginWindow);
            windows.Add(registrationSuccessWindow);
		}

		private void ShowWindow(GameObject windowToShow) {
			foreach (GameObject window in windows) {
				if (window == windowToShow || window == background) {
					window.SetActive (true);
				}
				else {
					window.SetActive (false);
				}
			}
		}

		public void HideAllWindows() {
            if (windows != null) {
                foreach (GameObject window in windows)
                {
                    window.SetActive(false);
                }
            }
		}

	}

}