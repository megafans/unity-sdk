#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class OnboardingTutorialScreenUI : MonoBehaviour
    {
        [SerializeField] private GameObject onboardingTutorialStartWindow;
        [SerializeField] private GameObject howMegafansTutorialWindow;
        [SerializeField] private GameObject prizesMegafansTutorialWindow;
        [SerializeField] private GameObject detailsMegafansTutorialWindow;
        [SerializeField] private GameObject boosterMegafansTutorialWindow;
        [SerializeField] private GameObject leaderboardMegafansTutorialWindow;
        [SerializeField] private GameObject freeTokensMegafansTutorialWindow;
        [SerializeField] private GameObject registerNowMegafansTutorialWindow;


        private List<GameObject> windows;

        void Awake()
        {
            FillWindows();
        }

        public void ShowStartWindow()
        {
            ShowWindow(onboardingTutorialStartWindow);
        }

        public void ShowHowMegafansWindow()
        {
            ShowWindow(howMegafansTutorialWindow);
        }

        public void ShowPrizesMegafansWindow()
        {
            ShowWindow(prizesMegafansTutorialWindow);
        }

        public void ShowDetailsMegafansWindow()
        {
            ShowWindow(detailsMegafansTutorialWindow);
        }

        public void ShowBoosterMegafansWindow()
        {
            ShowWindow(boosterMegafansTutorialWindow);
        }

        public void ShowLeaderboardMegafansWindow()
        {
            ShowWindow(leaderboardMegafansTutorialWindow);
        }

        public void ShowTokensMegafansWindow()
        {
            ShowWindow(freeTokensMegafansTutorialWindow);
        }

        public void ShowRegisterNowMegaFansWindow()
        {
            registerNowMegafansTutorialWindow.SetActive(true);
        }

        private void FillWindows()
        {
            if (windows == null)
            {
                windows = new List<GameObject>();
            }
            else
            {
                windows.Clear();
            }

            windows.Add(onboardingTutorialStartWindow);
            windows.Add(howMegafansTutorialWindow);
            windows.Add(prizesMegafansTutorialWindow);
            windows.Add(boosterMegafansTutorialWindow);
            windows.Add(leaderboardMegafansTutorialWindow);
            windows.Add(freeTokensMegafansTutorialWindow);
            windows.Add(detailsMegafansTutorialWindow);
            windows.Add(registerNowMegafansTutorialWindow);
        }

        private void ShowWindow(GameObject windowToShow)
        {
            if (windows != null)
            {
                foreach (GameObject window in windows)
                {
                    if (window == windowToShow)
                    {
                        window.SetActive(true);
                    }
                    else
                    {
                        window.SetActive(false);
                    }
                }
            }
        }

        public void HideAllWindows()
        {
            if (windows != null)
            {

                foreach (GameObject window in windows)
                {
                    window.SetActive(false);
                }
            }
        }

        public void LoginToAccount()
        {
            MegafansUI.Instance.ShowLandingWindow(true);
        }

        public void CloseOnboardingBtn_OnClick()
        {
            MegafansUI.Instance.EnableUI(false);
        }

        public void CloseOnboardingRegisterNowBtn_OnClick()
        {
            registerNowMegafansTutorialWindow.SetActive(false);
        }
    }

}