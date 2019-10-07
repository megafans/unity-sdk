#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.Utils;

namespace MegafansSDK.UI {

	public class LoginEmailUI : MonoBehaviour {
		
		[SerializeField] private InputField emailField;
		[SerializeField] private InputField passwordField;
		[SerializeField] private Button loginBtn;
        [SerializeField] private Button showPasswordBtn;

        [SerializeField] private RawImage inputFieldHighlightedImage;
        private Sprite activeInputBackground;

        private bool IsFormValid {
			get {
				return MegafansUtils.IsEmailValid (emailField.text) &&
				MegafansUtils.IsPasswordValid (passwordField.text);
			}
		}

		void Awake() {
            Texture2D spriteTexture = (Texture2D)inputFieldHighlightedImage.texture;
            activeInputBackground = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0.5f));
            loginBtn.interactable = false;
        }

		void OnEnable() {
            emailField.text = "";
            passwordField.text = "";

            //emailField.text = "mark.hoyt970@gmail.com";
            //passwordField.text = "Password1";
        }

		public void EmailField_OnValueChanged() {
			loginBtn.interactable = IsFormValid;
		}

		public void PasswordField_OnValueChanged() {
			loginBtn.interactable = IsFormValid;
		}

        public void ShowPasswordBtn_OnClick() {
            if (passwordField.contentType.Equals(InputField.ContentType.Password)) {
                passwordField.contentType = InputField.ContentType.Standard;
                showPasswordBtn.GetComponentInChildren<Text>().text = "HIDE";
            } else {
                passwordField.contentType = InputField.ContentType.Password;
                showPasswordBtn.GetComponentInChildren<Text>().text = "SHOW";
            }
            passwordField.ForceLabelUpdate();
        }

		public void LoginBtn_OnClick() {
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
            MegafansPrefs.Email = emailField.text;
            MegafansWebService.Instance.LoginEmail (emailField.text, passwordField.text, OnLoginEmailResponse,
				OnLoginEmailFailure);
		}

		public void ResetPasswordBtn_OnClick() {
			if (!MegafansUtils.IsEmailValid(emailField.text)) {
				string msg = "Please enter a valid email address.";
				MegafansUI.Instance.ShowPopup ("ERROR", msg);

				return;
			}

			MegafansWebService.Instance.ResetPassword (emailField.text, OnResetPasswordResponse,
				OnResetPasswordFailure);
		}

		private void OnLoginEmailResponse(LoginResponse response) {
			if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
                MegafansPrefs.AccessToken = response.data.token;
                MegafansPrefs.RefreshToken = response.data.refresh;
                //MegafansPrefs.Email = response.data.email;
                MegafansPrefs.ProfilePicUrl = response.data.image;
                Debug.Log("REGISTER USER With ID");
                Debug.Log(MegafansPrefs.UserId.ToString());
                Debug.Log(MegafansPrefs.UserId);
                Megafans.Instance.ReportUserLoggedIn (MegafansPrefs.UserId.ToString());
            }
            else {
				MegafansUI.Instance.ShowPopup ("ERROR", response.message);
			}
		}

		private void OnLoginEmailFailure(string error) {
            Debug.LogError (error.ToString());

			if (error.ToLower().Contains (MegafansConstants.UNAUTHORIZED_CODE.ToLower())) {
				string msg = "Invalid email/password.";
				MegafansUI.Instance.ShowPopup ("ERROR", msg);
            } else {
                MegafansUI.Instance.ShowPopup("ERROR", error);
            }
		}

		private void OnResetPasswordResponse(Response response) {
			if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
				MegafansUI.Instance.ShowPopup ("SUCCESS", response.message);
			}
			else {
				MegafansUI.Instance.ShowPopup ("ERROR", response.message);
			}
		}

		private void OnResetPasswordFailure(string error) {
			Debug.LogError (error);
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