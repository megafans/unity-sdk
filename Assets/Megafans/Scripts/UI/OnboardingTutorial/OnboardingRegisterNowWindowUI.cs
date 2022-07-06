#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MegafansSDK.UI
{
    public class OnboardingRegisterNowWindowUI : MonoBehaviour
    {

        [SerializeField] private Text gameDescriptionLabel;

        private void Awake()
        {
            string gameName = Application.productName;
            gameDescriptionLabel.text = "You can now play " + gameName + " and win prizes powered by MegaFans!";
        }

        public void SignUpBtn_OnClick()
        {
            gameObject.SetActive(false);
            MegafansUI.Instance.ShowLandingWindow(false);
        }
    }
}