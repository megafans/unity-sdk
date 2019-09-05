using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.Utils;

namespace MegafansSDK.UI {

	public class RegistrationEmailUI : MonoBehaviour {
    
		[SerializeField] private InputField emailField;
        [SerializeField] private InputField passwordField;
        [SerializeField] private RawImage inputFieldHighlightedImage;
        [SerializeField] private Image isValidEmail;
        [SerializeField] private Image isValidPassword;

        private Sprite activeInputBackground;
        [SerializeField] private RegistrationWindowUI mainRegistrationUI;

        private void Awake()
        {
            Texture2D spriteTexture = (Texture2D)inputFieldHighlightedImage.texture;
            activeInputBackground = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0.5f));
        }

        void OnEnable() {
            emailField.text = "";
			passwordField.text = "";
        }

		public void ContinueBtn_OnClick() {
			if (!MegafansUtils.IsEmailValid(emailField.text)) {
				string msg = "Please enter a valid email address.";
				MegafansUI.Instance.ShowPopup ("ERROR", msg);

				return;
			}

			if (!MegafansUtils.IsPasswordValid(passwordField.text)) {
				string msg = "Password must be at least 8 characters long.";
				MegafansUI.Instance.ShowPopup ("ERROR", msg);

				return;
			}

			if (!mainRegistrationUI.IsAgreementAcceptedEmail) {
				string msg = "Please accept our Terms of Use and Privacy Policy.";
				MegafansUI.Instance.ShowPopup ("ERROR", msg);

				return;
			}
            MegafansUI.Instance.ShowLoadingBar();
            MegafansWebService.Instance.RegisterEmail (emailField.text, passwordField.text,
				OnRegisterEmailResponse, OnRegisterEmailFailure);
		}

		private void OnRegisterEmailResponse(RegisterResponse response) {
            MegafansUI.Instance.HideLoadingBar();
            if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
                MegafansPrefs.Email = emailField.text;
                Megafans.Instance.ReportUserRegistered(MegafansPrefs.UserId.ToString());
                MegafansUI.Instance.ShowRegistrationSuccessWindow(true);
			}
			else {
                string errorString = "Sorry there was an issue with your email or password.  Please try again.";
                MegafansUI.Instance.ShowPopup ("ERROR", errorString);
			}
		}

		private void OnRegisterEmailFailure(string error) {
            MegafansUI.Instance.HideLoadingBar();
            Debug.LogError (error);
            MegafansUI.Instance.ShowPopup("ERROR", error);
        }

        public void onEmailInputChange()
        {

            if (MegafansUtils.IsEmailValid(emailField.text))
            {
                isValidEmail.gameObject.SetActive(true);
            } else {
                isValidEmail.gameObject.SetActive(false);
            }
        }

        public void onPasswordInputChange()
        {

            if (MegafansUtils.IsPasswordValid(passwordField.text))
            {

                isValidPassword.gameObject.SetActive(true);
            }
            else
            {
                isValidPassword.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if (emailField.GetComponent<InputField>().isFocused == true && emailField.image.sprite != activeInputBackground)
            {
                emailField.image.sprite = activeInputBackground;
            }

            if (passwordField.GetComponent<InputField>().isFocused == true && passwordField.image.sprite != activeInputBackground)
            {
                passwordField.image.sprite = activeInputBackground;
            }
        }

    }

}