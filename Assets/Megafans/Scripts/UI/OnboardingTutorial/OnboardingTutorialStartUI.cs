#pragma warning disable 649

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class OnboardingTutorialStartUI : MonoBehaviour
    {
        float currCountdownValue;
        [SerializeField] private Text gameDescriptionLabel;

        //You can now play Jet Jack Tournament Edition and win prizes and real money powered by MegaFans

        private void Awake() {
            string gameName = Application.productName;
            gameDescriptionLabel.text = "You can now play " + gameName + " and win prizes powered by MegaFans!";
        }

        public void StartBtn_OnClick()
        {
            MegafansUI.Instance.ShowOnboardingHowWindow();
        }

        void OnEnable()
        {
            if (!Megafans.Instance.IsUserLoggedIn)
            {
                MegafansWebService.Instance.RegisterNewUser(OnRegisterNewUserResponse, OnRegisterNewUserFailure);
                StartCoroutine(StartCountdown(7));
            }
        }

        private void OnRegisterNewUserResponse(RegisterFirstTimeResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansPrefs.AccessToken = response.data.token;
                MegafansPrefs.RefreshToken = response.data.refresh;
                MegafansPrefs.Username = response.data.username;
                MegafansPrefs.UserId = response.data.userId;
                MegafansPrefs.SMSAvailable = response.data.sms;
             
                //OneSignal.SetExternalUserId(MegafansPrefs.UserId.ToString());

                Megafan.NativeWrapper.MegafanNativeWrapper.RegisterUserWithUserId(response.data.userId.ToString(),
                                                                  Megafans.Instance.GameUID,
                                                                  Application.productName);
                Megafans.Instance.m_AdsManager.initIronSourceWithUserId(MegafansPrefs.UserId.ToString());
            }
            else
            {
                MegafansUI.Instance.ShowPopup("ERROR", response.message);
            }
        }

        private void OneSignal_promptForPushNotificationsReponse(bool accepted)
        {
            Debug.Log("OneSignal_promptForPushNotificationsReponse: " + accepted);
        }

        private void OnRegisterNewUserFailure(string error)
        {
            Debug.LogError(error);
        }

        public IEnumerator StartCountdown(float countdownValue = 7)
        {
            currCountdownValue = countdownValue;
            while (currCountdownValue > 0)
            {
                Debug.Log("Countdown: " + currCountdownValue);
                yield return new WaitForSeconds(1.0f);
                currCountdownValue--;
            }
            this.StartBtn_OnClick();
        }
    }

}