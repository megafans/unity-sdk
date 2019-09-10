using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MegaFans.Unity.iOS;
using MegaFans.Unity.Android;

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
                //TODO: removed OneSignal //OneSignal.SetExternalUserId(response.data.userId.ToString());
#if UNITY_EDITOR
                Debug.Log("Unity Editor");
#elif UNITY_IOS
                Debug.Log("IOS");
                IntercomWrapperiOS.RegisterIntercomUserWithID(MegafansPrefs.UserId.ToString(), Megafans.Instance.GameUID, Application.productName);
#elif UNITY_ANDROID
                Debug.Log("ANDROID");
                IntercomWrapperAndroid.RegisterUserWithUserId(MegafansPrefs.UserId.ToString(), Megafans.Instance.GameUID, Application.productName);
#endif
            }
            else
            {
                MegafansUI.Instance.ShowPopup("ERROR", response.message);
            }
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