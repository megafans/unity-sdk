using UnityEngine;
using UnityEngine.UI;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{
    public class TournamentPasswordUI : MonoBehaviour
    {
        public InputField passwordInput;
        public GameObject wrongPassword;
        internal string currentTournamentID;

        private void OnEnable()
        {
            wrongPassword.SetActive(false);
            passwordInput.text = "";
        }

        public void ConfirmPassword()
        {
            MegafansWebService.Instance.GetCheckPassword(currentTournamentID, passwordInput.text, OnPasswordCheckResponse, OnPasswordCheckFailure);
        }

        private void OnPasswordCheckResponse(GetPasswordCheckResponse response)
        {
            if (response != null && response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                TournamentLobbyUI _TLUI = FindObjectOfType<TournamentLobbyUI>();
                if (_TLUI != null)
                {
                    JoinMatchAssistant matchAssistant = _TLUI.GetComponent<JoinMatchAssistant>();
                    matchAssistant.JoinTournamentMatch(currentTournamentID);
                    gameObject.SetActive(false);
                }

                return;
            }

            ShowWrongPassword();
        }

        void ShowWrongPassword()
        {
            wrongPassword.SetActive(true);
            Invoke("HideWrongPassword", 1.1f);
        }

        void HideWrongPassword()
        {
            wrongPassword.SetActive(false);
        }

        private void OnPasswordCheckFailure(string error)
        {
            ShowWrongPassword();
            Debug.LogError(error);
        }

        public void Back()
        {
            gameObject.SetActive(false);
        }
    }
}
