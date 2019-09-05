using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.Utils;
namespace MegafansSDK.UI {

    public class UpdatePasswordWindowUI : MonoBehaviour {

        [SerializeField] private Text userTokensTxt;
        [SerializeField] private InputField oldPasswordField;
        [SerializeField] private InputField newPasswordField;
        [SerializeField] private InputField newPasswordConfirmField;
        [SerializeField] private Button showCurrentPasswordBtn;
        [SerializeField] private Button showNewPasswordBtn;
        [SerializeField] private Button showConfirmPasswordBtn;
        [SerializeField] private Button savePasswordBtn;
        [SerializeField] private RawImage inputFieldHighlightedImage;

        private Sprite activeInputBackground;
        private Sprite inactiveInputBackground;

        private bool IsFormValid
        {
            get
            {
                return MegafansUtils.IsPasswordValid(oldPasswordField.text) &&
                       MegafansUtils.IsPasswordValid(newPasswordField.text) &&
                       MegafansUtils.IsPasswordValid(newPasswordConfirmField.text) &&
                       newPasswordField.text == newPasswordConfirmField.text;
            }
        }

        void Start()
        {
         
        }

        void Awake()
        {
            Texture2D spriteTexture = (Texture2D)inputFieldHighlightedImage.texture;
            inactiveInputBackground = oldPasswordField.image.sprite;
            activeInputBackground = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0.5f));
        }

        void OnEnable()
        {
            userTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
            oldPasswordField.text = "";
            newPasswordField.text = "";
            newPasswordConfirmField.text = "";
            oldPasswordField.image.sprite = inactiveInputBackground;
            newPasswordField.image.sprite = inactiveInputBackground;
            newPasswordConfirmField.image.sprite = inactiveInputBackground;
        }


        public void PasswordField_OnValueChanged()
        {
            savePasswordBtn.interactable = IsFormValid;
        }

        public void BackBtn_OnClick()
        {
            MegafansUI.Instance.ShowMyAccountWindow();
        }

        public void ShowCurrentPasswordBtn_OnClick()
        {
            if (oldPasswordField.contentType.Equals(InputField.ContentType.Password))
            {
                oldPasswordField.contentType = InputField.ContentType.Standard;
                showCurrentPasswordBtn.GetComponentInChildren<Text>().text = "HIDE";
            }
            else
            {
                oldPasswordField.contentType = InputField.ContentType.Password;
                showCurrentPasswordBtn.GetComponentInChildren<Text>().text = "SHOW";
            }
            oldPasswordField.ForceLabelUpdate();
        }

        public void ShowNewPasswordBtn_OnClick()
        {
            if (newPasswordField.contentType.Equals(InputField.ContentType.Password))
            {
                newPasswordField.contentType = InputField.ContentType.Standard;
                showNewPasswordBtn.GetComponentInChildren<Text>().text = "HIDE";
            }
            else
            {
                newPasswordField.contentType = InputField.ContentType.Password;
                showNewPasswordBtn.GetComponentInChildren<Text>().text = "SHOW";
            }
            newPasswordField.ForceLabelUpdate();
        }

        public void ShowConfirmPasswordBtn_OnClick()
        {
            if (newPasswordConfirmField.contentType.Equals(InputField.ContentType.Password))
            {
                newPasswordConfirmField.contentType = InputField.ContentType.Standard;
                showConfirmPasswordBtn.GetComponentInChildren<Text>().text = "HIDE";
            }
            else
            {
                newPasswordConfirmField.contentType = InputField.ContentType.Password;
                showConfirmPasswordBtn.GetComponentInChildren<Text>().text = "SHOW";
            }
            newPasswordConfirmField.ForceLabelUpdate();
        }

        public void UpdateBtn_OnClick()
        {
            if (!MegafansUtils.IsPasswordValid(oldPasswordField.text))
            {
                string msg = "Must enter your last password";
                MegafansUI.Instance.ShowPopup("ERROR", msg);

                return;
            }

            if (!MegafansUtils.IsPasswordValid(newPasswordField.text))
            {
                string msg = "Please enter a new password.  New password must be at least 8 characters long.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);

                return;
            }

            if (newPasswordField.text != newPasswordConfirmField.text)
            {
                string msg = "Password and confirm password do not match.";
                MegafansUI.Instance.ShowPopup("ERROR", msg);

                return;
            }

            MegafansWebService.Instance.UpdatePassword(oldPasswordField.text, newPasswordField.text, OnUpdatePasswordResponse, OnUpdatePasswordFailure);
        }

        private void OnUpdatePasswordResponse(UpdatePasswordResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                Debug.Log("Password Updated Successfully");
                MegafansUI.Instance.ShowPopup("SUCCESS", "Successfully updated your password.");
                MegafansUI.Instance.ShowMyAccountWindow();
            }
            else
            {
                string msg = "Failed to update password.  Please try again.";
                if (!string.IsNullOrEmpty(response.message)) {
                    msg = response.message;
                }
                MegafansUI.Instance.ShowPopup("ERROR", msg);
            }
        }

        private void OnUpdatePasswordFailure(string error)
        {
            Debug.LogError(error);
        }

        void Update()
        {
            if (oldPasswordField.GetComponent<InputField>().isFocused == true && oldPasswordField.image.sprite != activeInputBackground)
            {
                oldPasswordField.image.sprite = activeInputBackground;
            }

            if (newPasswordField.GetComponent<InputField>().isFocused == true && newPasswordField.image.sprite != activeInputBackground)
            {
                newPasswordField.image.sprite = activeInputBackground;
            }

            if (newPasswordConfirmField.GetComponent<InputField>().isFocused == true && newPasswordConfirmField.image.sprite != activeInputBackground)
            {
                newPasswordConfirmField.image.sprite = activeInputBackground;
            }
        }
    }

}