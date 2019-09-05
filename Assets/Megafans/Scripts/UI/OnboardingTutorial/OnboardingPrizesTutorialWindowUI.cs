using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class OnboardingPrizesTutorialWindowUI : MonoBehaviour
    {
        float currCountdownValue;

        void OnEnable()
        {
            StartCoroutine(StartCountdown(7));
        }

        public void NextBtn_OnClick()
        {
            MegafansUI.Instance.ShowOnboardingDetailsWindow();
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
            this.NextBtn_OnClick();
        }
    }
}